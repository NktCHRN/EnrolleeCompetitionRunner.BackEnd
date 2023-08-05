using EnrolleeCompetitionRunner.Domain.Abstractions;
using EnrolleeCompetitionRunner.Domain.Constants;
using EnrolleeCompetitionRunner.Domain.Enums;
using EnrolleeCompetitionRunner.Domain.Extensions;
using EnrolleeCompetitionRunner.Domain.UtilityModels;

namespace EnrolleeCompetitionRunner.Domain.Entities;
public class Supercompetition : BaseEntity
{
    public int TotalPlaces { get; private set; }

    public string Code { get; private set; }

    public EducationalStage EnrollmentBasis { get; private set; }
    public EducationalStage EducationalStage { get; private set; }

    public IReadOnlyList<Speciality> Specialities => _specialities;
    private readonly List<Speciality> _specialities = new();

    private Supercompetition()
    {
        Code = string.Empty;
    }

    public Supercompetition(
        int totalPlaces,
        string code,
        EducationalStage enrollmentBasis,
        EducationalStage educationalStage)
    {
        TotalPlaces = totalPlaces;
        Code = code;
        EnrollmentBasis = enrollmentBasis;
        EducationalStage = educationalStage;
    }

    public void AddSpeciality(Speciality speciality)
    {
        _specialities.Add(speciality);
    }

    public CompetitionResultDetails HandleCompetition()
    {
        var waitingOfferEnrollees = new List<OfferEnrollee>();

        var offers = Specialities.SelectMany(s => s.Offers);

        foreach (var offer in offers)
        {
            var offerResult = offer.HandleCompetition();
            waitingOfferEnrollees.AddRange(offerResult.WaitingOfferEnrollees);
        }

        var passedCompetitionsEnrollees = offers
            .SelectMany(o => o.Enrollees)
            .Where(e => e.FinalStatus is OfferEnrolleeConstants.Statuses.RecommendedBudget);
        var passedCompetitionsEnrolleesCount = passedCompetitionsEnrollees.Count();

        if (passedCompetitionsEnrolleesCount > TotalPlaces)
        {
            var notPassedSubcompetitionEnrollees = passedCompetitionsEnrollees
                .Where(e => e.SubcompetitionType == SubcompetitionType.General)
                .OrderByScoreAndPriority()
                .TakeLast(passedCompetitionsEnrolleesCount - TotalPlaces);

            foreach (var offerEnrollee in notPassedSubcompetitionEnrollees)
            {
                var nextPriorityOfferResult = offerEnrollee.MarkAsNotPassed();
                if (nextPriorityOfferResult.NextPriorityOffer is not null)
                {
                    waitingOfferEnrollees.Add(nextPriorityOfferResult.NextPriorityOffer);
                }
            }
        }

        return new CompetitionResultDetails(waitingOfferEnrollees);
    }
}
