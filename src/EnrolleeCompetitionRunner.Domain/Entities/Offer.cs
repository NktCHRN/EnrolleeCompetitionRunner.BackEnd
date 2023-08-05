using EnrolleeCompetitionRunner.Domain.Abstractions;
using EnrolleeCompetitionRunner.Domain.Constants;
using EnrolleeCompetitionRunner.Domain.Enums;
using EnrolleeCompetitionRunner.Domain.Extensions;
using EnrolleeCompetitionRunner.Domain.UtilityModels;

namespace EnrolleeCompetitionRunner.Domain.Entities;
public class Offer : BaseEntity
{
    public int BudgetPlaces { get; private set; }
    public int Quote1Places { get; private set; }
    public int Quote2Places { get; private set; }

    public string Code { get; private set; }
    public string Name { get; private set; }

    public EducationalStage EnrollmentBasis { get; private set; }
    public EducationalStage EducationalStage { get; private set; }

    public string? FacultyName { get; private set; }

    public Guid SpecialityId { get; set; }
    public Speciality Speciality { get; private set; }

    public Guid UniversityId { get; set; }
    public University University { get; private set; }

    public IReadOnlyList<OfferEnrollee> Enrollees => _enrollees;

    private readonly List<OfferEnrollee> _enrollees = new();

    private Offer()
    {
        Code = string.Empty;
        Name = string.Empty;
        Speciality = null!;
        University = null!;
    }

    public Offer(
        int budgetPlaces,
        int quote1Places,
        int quote2Places,
        string code,
        string name,
        string? facultyName,
        EducationalStage enrollmentBasis,
        EducationalStage educationalStage,
        Speciality speciality,
        University university)
    {
        BudgetPlaces = budgetPlaces;
        Quote1Places = quote1Places;
        Quote2Places = quote2Places;
        Code = code;
        Name = name;
        EnrollmentBasis = enrollmentBasis;
        EducationalStage = educationalStage;
        FacultyName = facultyName;
        SpecialityId = speciality.Id;
        Speciality = speciality;
        UniversityId = university.Id;
        University = university;
    }

    public void AddEnrollee(OfferEnrollee enrollee)
    {
        _enrollees.Add(enrollee);
    }

    public CompetitionResultDetails HandleCompetition()
    {
        foreach (var offerEnrollee in Enrollees)
        {
            offerEnrollee.SubcompetitionType = SubcompetitionType.None;
            offerEnrollee.UnmarkAsPassed();
        }

        var quote1Results = HandleQuote1();
        var quote2Results = HandleQuote2(quote1Results.PassedOfferEnrollees);
        var interviewResults = HandleInterview();

        var generalResults = HandleGeneral(
            BudgetPlaces - quote1Results.OccupiedPlaces - quote2Results.OccupiedPlaces - interviewResults.OccupiedPlaces, 
            quote1Results.NotPassedOfferEnrollees.Union(quote2Results.NotPassedOfferEnrollees));

        var waitingOfferEnrollees = new List<OfferEnrollee>();
        foreach (var offerEnrollee in generalResults.NotPassedOfferEnrollees)
        {
            var nextPriorityOfferResult = offerEnrollee.MarkAsNotPassed();
            if (nextPriorityOfferResult.NextPriorityOffer is not null)
            {
                waitingOfferEnrollees.Add(nextPriorityOfferResult.NextPriorityOffer);
            }
        }

        return new CompetitionResultDetails(waitingOfferEnrollees);

        SubcompetitionResultDetails HandleQuote1()
        {
            var quote1OfferEnrollees = GetAdmittedBudgetEnrollees().Where(e => e.HasQuote1);
            SetSubcompetitionTypes(quote1OfferEnrollees, SubcompetitionType.Quote1);
            return HandleSubcompetition(SubcompetitionType.Quote1, Quote1Places);
        }
        SubcompetitionResultDetails HandleQuote2(IEnumerable<OfferEnrollee> passedQuote1Enrollees)
        {
            var quote2OfferEnrollees = GetAdmittedBudgetEnrollees().Where(e => e.HasQuote2).Except(passedQuote1Enrollees).ToList();
            SetSubcompetitionTypes(quote2OfferEnrollees, SubcompetitionType.Quote2);
            return HandleSubcompetition(SubcompetitionType.Quote2, Quote2Places);
        }
        SubcompetitionResultDetails HandleInterview()
        {
            var interviewOfferPassedEnrollees = Enrollees.Where(e => !e.IsContractOnly
                    && e.HasInterview
                    && (e.InitialStatus is OfferEnrolleeConstants.Statuses.RecommendedBudget or OfferEnrolleeConstants.Statuses.IncludedToOrder))
                .ToList();
            SetSubcompetitionTypes(interviewOfferPassedEnrollees, SubcompetitionType.Interview);
            return HandleSubcompetition(SubcompetitionType.Interview, BudgetPlaces);
        }
        SubcompetitionResultDetails HandleGeneral(int currentBudgetPlaces, IEnumerable<OfferEnrollee> notPassedPreviousSubcompetitionsOfferEnrollees)
        {
            var generalEnrollees = GetAdmittedBudgetEnrollees()
                .Where(e => !e.HasQuote1 && !e.HasQuote2)
                .Concat(notPassedPreviousSubcompetitionsOfferEnrollees)
                .ToList();
            SetSubcompetitionTypes(generalEnrollees, SubcompetitionType.General);
            return HandleSubcompetition(SubcompetitionType.General, currentBudgetPlaces);
        }
    }

    private IEnumerable<OfferEnrollee> GetAdmittedBudgetEnrollees()
        => Enrollees
            .Where(e => !e.IsWaiting
                && !e.IsContractOnly
                && !e.HasInterview
                && (e.InitialStatus is OfferEnrolleeConstants.Statuses.Admitted
                                   or OfferEnrolleeConstants.Statuses.RecommendedBudget)         // Just for test with already processed data.
                && e.Equals(e.Enrollee.CurrentPriorityOffer));

    private static void SetSubcompetitionTypes(IEnumerable<OfferEnrollee> offerEnrollees, SubcompetitionType subcompetitionType)
    {
        foreach (var enrollee in offerEnrollees)
        {
            enrollee.SubcompetitionType = subcompetitionType;
        }
    }

    private SubcompetitionResultDetails HandleSubcompetition(SubcompetitionType type, int places)
    {
        var offerEnrollees = Enrollees
            .Where(e => e.SubcompetitionType == type)
            .OrderByScoreAndPriority();

        var passedEnrollees = offerEnrollees
            .Take(places)
            .ToList();

        foreach (var enrollee in passedEnrollees)
        {
            enrollee.MarkAsPassed();
        }

        var freePlaces = places - passedEnrollees.Count;

        var notPassedEnrolees = offerEnrollees.Except(passedEnrollees);

        return new SubcompetitionResultDetails(passedEnrollees, notPassedEnrolees, freePlaces, passedEnrollees.Count);
    }

    public void AwardBestEnrolleesWithScholarship()
    {
        var passedEnrollees = GetPotentiallyPassingEnrollees()
            .Where(e => e.FinalStatus == OfferEnrolleeConstants.Statuses.RecommendedBudget)
            .OrderByScoreAndPriority();

        var enrolleesWithScolarshipCount = (int)Math.Floor(passedEnrollees.Count() * OfferConstants.ScholarshipRate);

        var enrolleesWithScolarship = passedEnrollees
            .Take(enrolleesWithScolarshipCount);
        foreach (var offerEnrollee in enrolleesWithScolarship)
        {
            offerEnrollee.MarkAsAwardedWithScholarship();
        }
    }

    public decimal CalculateQuote1PassingScore()
    {
        var enrolleesByType = GetPotentiallyPassingEnrollees()
            .Where(e => e.HasQuote1);

        var passedEnrollees = Enrollees
            .Where(e => e.SubcompetitionType == SubcompetitionType.Quote1 && e.FinalStatus == OfferEnrolleeConstants.Statuses.RecommendedBudget);

        return passedEnrollees.Any() && passedEnrollees.Count() < enrolleesByType.Count() ? passedEnrollees.Min(e => e.Score) : 0;
    }

    public decimal CalculateQuote2PassingScore()
    {
        var enrolleesByType = GetPotentiallyPassingEnrollees()
            .Where(e => e.HasQuote2);

        var passedEnrollees = Enrollees
            .Where(e => e.SubcompetitionType == SubcompetitionType.Quote2 && e.FinalStatus == OfferEnrolleeConstants.Statuses.RecommendedBudget);

        return passedEnrollees.Any() && passedEnrollees.Count() < enrolleesByType.Count() ? passedEnrollees.Min(e => e.Score) : 0;
    }

    public decimal CalculateGeneralPassingScore()
    {
        var enrolleesByType = Enrollees
            .Where(e => !e.HasQuote1 && !e.HasQuote2);

        var passedEnrollees = Enrollees
            .Where(e => e.SubcompetitionType == SubcompetitionType.General && e.FinalStatus == OfferEnrolleeConstants.Statuses.RecommendedBudget);

        return passedEnrollees.Any() && passedEnrollees.Count() < enrolleesByType.Count() ? passedEnrollees.Min(e => e.Score) : 0;
    }

    private IEnumerable<OfferEnrollee> GetPotentiallyPassingEnrollees()
    {
        return Enrollees
            .Where(e => !e.IsContractOnly
                && !e.HasInterview
                && (e.InitialStatus is OfferEnrolleeConstants.Statuses.Admitted
                                   or OfferEnrolleeConstants.Statuses.RecommendedBudget));
    }

    public decimal CalculateMinScholarshipScore()
    {
        var enrollees = Enrollees
            .Where(e => e.HasScholarship);

        return enrollees.Any() ? enrollees.Min(e => e.Score) : 0;
    }

    public int GetPassedEnrolleesCount() => Enrollees.Where(e => e.FinalStatus is OfferEnrolleeConstants.Statuses.RecommendedBudget).Count();
}
