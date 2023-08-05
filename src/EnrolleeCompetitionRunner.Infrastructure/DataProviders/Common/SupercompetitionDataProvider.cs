using EnrolleeCompetitionRunner.Core.Abstractions;
using EnrolleeCompetitionRunner.Domain.Abstractions;
using EnrolleeCompetitionRunner.Domain.Entities;
using EnrolleeCompetitionRunner.Domain.Enums;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.AbitPoisk.Abstractions;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.AbitPoisk.Clients;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.AbitPoisk.ScrapedResultModels;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Abstractions;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Clients;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Requests;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using static EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Constants.EducationalStageConstants;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Common;
public sealed class SupercompetitionDataProvider : ISupercompetitionDataProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITypedHttpClientFactory<AbitPoiskHttpClient> _abitPoiskHttpClientFactory;
    private AbitPoiskHttpClient AbitPoiskHttpClient => _abitPoiskHttpClientFactory.CreateClient(_httpClientFactory.CreateClient(nameof(AbitPoiskHttpClient)));
    private readonly IAbitPoiskScraper _abitPoiskScraper;

    private readonly ITypedHttpClientFactory<EdboHttpClient> _edboHttpClientFactory;
    private EdboHttpClient EdboHttpClient => _edboHttpClientFactory.CreateClient(_httpClientFactory.CreateClient(nameof(EdboHttpClient)));
    private readonly IEdboScraper _edboScraper;

    private readonly IDefaultQuotePlacesCalculator _defaultQuotePlacesCalculator;

    private readonly IDateTimeProvider _dateTimeProvider;

    private readonly ILogger<SupercompetitionDataProvider> _logger;

    private readonly IEnumerable<string> _enrollmentBases = new string[]
    {
        EnrollmentBasis.SecondaryEducationCode,
        EnrollmentBasis.ProfessionalJuniorBachelorCode,
        EnrollmentBasis.JuniorBachelorCode,
        EnrollmentBasis.JuniorSpecialistCode,
    };

    private ParallelOptions ParallelOptions => new() { MaxDegreeOfParallelism = 16 };

    public SupercompetitionDataProvider(
        IHttpClientFactory httpClientFactory,
        ITypedHttpClientFactory<AbitPoiskHttpClient> abitPoiskHttpClientFactory,
        IAbitPoiskScraper abitPoiskScraper,
        ITypedHttpClientFactory<EdboHttpClient> edboHttpClientFactory,
        IEdboScraper edboScraper,
        IDefaultQuotePlacesCalculator defaultQuotePlacesCalculator,
        IDateTimeProvider dateTimeProvider,
        ILogger<SupercompetitionDataProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _abitPoiskHttpClientFactory = abitPoiskHttpClientFactory;
        _abitPoiskScraper = abitPoiskScraper;
        _edboHttpClientFactory = edboHttpClientFactory;
        _edboScraper = edboScraper;
        _defaultQuotePlacesCalculator = defaultQuotePlacesCalculator;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Supercompetition>> GetSupercompetitionsAsync()
    {
        _logger.LogInformation("Retrieving super amounts");
        var edboSuperCompetitionsPage = await EdboHttpClient.GetSupercompetitionInfoAsync();
        var edboSuperCompetitions = _edboScraper.ScrapSupercompetitionPage(edboSuperCompetitionsPage.Html);

        var superCompetitions = new ConcurrentBag<Supercompetition>();
        foreach (var edboSuperCompetition in edboSuperCompetitions)
        {
            superCompetitions.Add(new Supercompetition(
                totalPlaces: edboSuperCompetition.TotalPlaces,
                code: edboSuperCompetition.Code,
                enrollmentBasis: edboSuperCompetition.EnrollmentBasis,
                educationalStage: edboSuperCompetition.EducationalStage));
        }

        var specialities = new ConcurrentDictionary<Speciality, byte>();       // Used as a concurrent hashset.
        foreach (var edboSpeciality in edboSuperCompetitions.SelectMany(sc => sc.Specialities))
        {
            var speciality = new Speciality(
                code: edboSpeciality.Code,
                name: edboSpeciality.Name,
                specializationInternalCode: edboSpeciality.SpecializationInternalCode,
                specializationCode: edboSpeciality.SpecializationCode,
                specializationName: edboSpeciality.SpecializationName,
                supercompetition: superCompetitions.First(sc => sc.Code == edboSpeciality.Supercompetition.Code));
            specialities.TryAdd(speciality, default);

            speciality.Supercompetition.AddSpeciality(speciality);
        }

        _logger.LogInformation("Retrieving universities");
        var universities = new ConcurrentDictionary<University, byte>();       // Used as a concurrent hashset.
        var offerCodeBatches = new ConcurrentBag<(IEnumerable<string>, SearchUniversitiesRequest)>();
        await Parallel.ForEachAsync(specialities.Keys, ParallelOptions, async (speciality, cancellationToken) =>
        {
            foreach (var educationalBasis in _enrollmentBases)
            {
                var request = new SearchUniversitiesRequest
                {
                    SpecialityCodeWithInternalSpecializationCode = speciality.HasSpecialization() ? $"{speciality.Code}-{speciality.SpecializationInternalCode}" : speciality.Code,
                    EducationalStageCode = EducationalStageToEdboCode(speciality.Supercompetition.EducationalStage),
                    EnrollmentBasisCode = educationalBasis,
                };

                var foundUniversities = await EdboHttpClient.SearchUniversitiesAsync(request);

                foreach (var edboUniversity in foundUniversities.Universities)
                {
                    var university = new University(code: edboUniversity.Code.ToString(), name: edboUniversity.Name);
                    universities.TryAdd(university, default);
                }

                foreach (var offerCodeBatch in foundUniversities.Universities.Select(u => u.OfferCodes.Split(',')))
                {
                    offerCodeBatches.Add((offerCodeBatch, request));
                }
            }
        });

        _logger.LogInformation("Retrieving offers");
        var offersCount = offerCodeBatches.SelectMany(o => o.Item1).Count();
        var currentOfferCounter = 0;
        var enrollees = new ConcurrentDictionary<Enrollee, Enrollee>();            // Used in order not to add duplicates and keep references.

        await Parallel.ForEachAsync(offerCodeBatches.ToList(), ParallelOptions, async (offerCodeBatch, cancellationToken) =>    // ToList in order not to use thread-safe collection w/o any reason.
        {
            var edboOffersInfo = await EdboHttpClient.GetOffersInfoAsync(offerCodeBatch.Item1, offerCodeBatch.Item2);

            foreach (var edboOffer in edboOffersInfo.Offers)
            {
                _logger.LogInformation("Processing offer {offerCode}", edboOffer.Code);

                var abitPoiskOfferPage = await AbitPoiskHttpClient.GetOfferInfoAsync(edboOffer.Code.ToString(), _dateTimeProvider.UtcNow.Year);
                var abitPoiskOffer = new AbitPoiskResultOffer();
                if (abitPoiskOfferPage is not null)
                {
                    abitPoiskOffer = _abitPoiskScraper.ScrapOfferPage(abitPoiskOfferPage.Html, edboOffer.Code.ToString());
                }

                var offerSpeciality = specialities.Keys.FirstOrDefault(s => string.IsNullOrEmpty(edboOffer.SpecialityCodeAndSpecializationCode)
                        ? s.Code == edboOffer.SpecialityCode && !s.HasSpecialization()
                        : edboOffer.SpecialityCodeAndSpecializationCode == $"{s.Code}.{s.SpecializationCode}");
                if (offerSpeciality is null && !string.IsNullOrEmpty(edboOffer.SpecialityCodeAndSpecializationCode))        // For case like with speciality 223.01 Медсестринство that sometimes is written as 223.
                {
                    offerSpeciality = specialities.Keys.FirstOrDefault(s => s.Code == edboOffer.SpecialityCode && !s.HasSpecialization());
                    _logger.LogWarning("Speciality with code {SpecialityCodeAndSpecializationCode} {SpecialityCode} was not found. Offer code: {OfferCode}.", edboOffer.SpecialityCodeAndSpecializationCode, edboOffer.SpecialityCode, edboOffer.Code);
                }
                if (offerSpeciality is null)
                {
                    throw new InvalidOperationException($"Speciality with code {edboOffer.SpecialityCodeAndSpecializationCode} {edboOffer.SpecialityCode} was not found. Offer code: {edboOffer.Code}.");
                }

                var offerUniversity = universities.Keys.FirstOrDefault(u => u.Code == edboOffer.UniversityCode.ToString())
                    ?? throw new InvalidOperationException($"University with code {edboOffer.UniversityCode} was not found. Offer code: {edboOffer.Code}.");

                var offer = new Offer(
                    budgetPlaces: edboOffer.BudgetPlaces,
                    quote1Places: abitPoiskOffer.Quote1Places ?? _defaultQuotePlacesCalculator.CalculateQuote1Places(edboOffer.BudgetPlaces),
                    quote2Places: abitPoiskOffer.Quote2Places ?? _defaultQuotePlacesCalculator.CalculateQuote2Places(edboOffer.BudgetPlaces),
                    code: edboOffer.Code.ToString(),
                    name: edboOffer.Name,
                    facultyName: edboOffer.FacultyName,
                    enrollmentBasis: EdboCodeToEnrollmentBasis(edboOffer.EnrollmentBasisCode.ToString()),
                    educationalStage: EdboCodeToEducationalStage(edboOffer.EducationalStageCode),
                    speciality: offerSpeciality,
                    university: offerUniversity);
                offer.Speciality.AddOffer(offer);
                offer.University.AddOffer(offer);

                var offerUkrainianLanguageCodes = edboOffer.SubjectsInfo
                    .Where(s => s.Value.Name.Contains("Українська мова", StringComparison.OrdinalIgnoreCase))       // In order to support both "ЗНО" and "НМТ" codes.
                    .Select(s => s.Key)
                    .ToList();

                _logger.LogInformation("Retrieving enrollees for offer {offerCode}", offer.Code);

                var edboOfferEnrollees = new List<Edbo.Responses.OfferEnrollee>();
                var isLastPage = false;
                var lastEnrolleePosition = 0;

                while (!isLastPage)
                {
                    var newOfferEnrollees = await EdboHttpClient.GetOfferEnrolleesAsync(offer.Code, lastEnrolleePosition);
                    isLastPage = !newOfferEnrollees.Requests.Any();
                    lastEnrolleePosition = newOfferEnrollees.Requests.LastOrDefault()?.Position ?? 0;
                    edboOfferEnrollees.AddRange(newOfferEnrollees.Requests);
                }

                foreach (var edboOfferEnrollee in edboOfferEnrollees)
                {
                    var enrollee = new Enrollee(
                        name: edboOfferEnrollee.EnrolleeName.Any(c => char.IsNumber(c)) 
                            && edboOfferEnrollee.EnrolleeName.Any(c => c == '-') 
                            ? edboOfferEnrollee.EnrolleeName[(edboOfferEnrollee.EnrolleeName.IndexOf('-') + 1)..] 
                            : edboOfferEnrollee.EnrolleeName,
                        ukrainianLanguageExamScore: GetScoreFromFormula(edboOfferEnrollee.Coefficients.FirstOrDefault(c => offerUkrainianLanguageCodes.Contains(c.Code.ToString()))?.Formula));                    
                    enrollee = enrollees.GetOrAdd(enrollee, enrollee);    // Keeping references this way.

                    var offerEnrollee = new OfferEnrollee(
                        priority: edboOfferEnrollee.Priority == 0 ? null : edboOfferEnrollee.Priority,
                        isContractOnly: edboOfferEnrollee.Priority == 0,
                        score: edboOfferEnrollee.Score,
                        hasQuote1: edboOfferEnrollee.Coefficients.Any(c => c.Name?.Contains("Квота 1", StringComparison.OrdinalIgnoreCase) is true),
                        hasQuote2: edboOfferEnrollee.Coefficients.Any(c => c.Name?.Contains("Квота 2", StringComparison.OrdinalIgnoreCase) is true),
                        hasInterview: edboOfferEnrollee.Coefficients.Any(c => c.Name?.Contains("Індивідуальна усна співбесіда", StringComparison.OrdinalIgnoreCase) is true),
                        initialStatus: edboOfferEnrollee.Status,
                        code: edboOfferEnrollee.Code.ToString(),
                        offer: offer,
                        enrollee: enrollee);
                    offer.AddEnrollee(offerEnrollee);
                    enrollee.AddOffer(offerEnrollee);
                }

                currentOfferCounter++;
                _logger.LogInformation("Completed processing offer {offerCode}", edboOffer.Code);
                _logger.LogInformation("Offers processed: {currentCount} / {totalCount}", currentOfferCounter, offersCount);
            }
        });

        return superCompetitions.ToList();
    }

    private static string EducationalStageToEdboCode(EducationalStage educationalStage)
    {
        return educationalStage switch
        {
            EducationalStage.Bachelor => DesiredEducationalStage.BachelorCode,
            _ => throw new InvalidOperationException($"Unknown educational stage {educationalStage}")
        };
    }

    private static EducationalStage EdboCodeToEnrollmentBasis(string edboCode)
    {
        return edboCode switch
        {
            EnrollmentBasis.SecondaryEducationCode => EducationalStage.SecondaryEducation,
            EnrollmentBasis.ProfessionalJuniorBachelorCode => EducationalStage.ProfessionalJuniorBachelor,
            EnrollmentBasis.JuniorBachelorCode => EducationalStage.JuniorBachelor,
            EnrollmentBasis.JuniorSpecialistCode => EducationalStage.JuniorSpecialist,
            _ => throw new InvalidOperationException($"Unknown educational stage code {edboCode}")
        };
    }

    private static EducationalStage EdboCodeToEducationalStage(string edboCode)
    {
        return edboCode switch
        {
            DesiredEducationalStage.BachelorCode => EducationalStage.Bachelor,
            _ => throw new InvalidOperationException($"Unknown educational stage code {edboCode}")
        };
    }

    private static decimal GetScoreFromFormula(string? formula)
    {
        if (string.IsNullOrEmpty(formula)) 
            return 0;

        formula = formula.Replace(",", ".");
        return decimal.Parse(formula[..formula.IndexOf(' ')]);
    }
}
