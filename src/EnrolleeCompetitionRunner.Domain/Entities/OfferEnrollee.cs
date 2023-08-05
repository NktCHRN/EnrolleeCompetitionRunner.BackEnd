using EnrolleeCompetitionRunner.Domain.Abstractions;
using EnrolleeCompetitionRunner.Domain.Constants;
using EnrolleeCompetitionRunner.Domain.Enums;
using EnrolleeCompetitionRunner.Domain.UtilityModels;

namespace EnrolleeCompetitionRunner.Domain.Entities;
public class OfferEnrollee : BaseEntity
{
    public short? Priority { get; private set; }
    public bool IsContractOnly { get; private set; }

    public decimal Score { get; private set; }

    public bool HasQuote1 { get; private set; }
    public bool HasQuote2 { get; private set; }
    public bool HasInterview { get; private set; }

    public int InitialStatus { get; private set; }
    public int FinalStatus { get; private set; }

    public bool HasScholarship { get; private set; }

    public string Code { get; private set; }

    public bool IsWaiting { get; private set; }

    public SubcompetitionType SubcompetitionType { get; set; }

    public Guid OfferId { get; set; }
    public Offer Offer { get; private set; }

    public Guid EnrolleeId { get; set; }
    public Enrollee Enrollee { get; private set; }

    private OfferEnrollee() 
    { 
        Code = string.Empty;
        Offer = null!;
        Enrollee = null!;
    }

    public OfferEnrollee(
        short? priority,
        bool isContractOnly,
        decimal score,
        bool hasQuote1,
        bool hasQuote2,
        bool hasInterview,
        int initialStatus,
        string code,
        Offer offer,
        Enrollee enrollee)
    {
        Priority = priority;
        IsContractOnly = isContractOnly;
        Score = score;
        HasQuote1 = hasQuote1;
        HasQuote2 = hasQuote2;
        HasInterview = hasInterview;
        InitialStatus = initialStatus;
        FinalStatus = initialStatus;        // It will be equal to initial status before the competition.
        Code = code;
        OfferId = offer.Id;
        Offer = offer;
        EnrolleeId = enrollee.Id;
        Enrollee = enrollee;
    }

    public void MarkAsPassed() => FinalStatus = OfferEnrolleeConstants.Statuses.RecommendedBudget;
    public void UnmarkAsPassed() => FinalStatus = InitialStatus;

    public SwitchToNextPriorityResult MarkAsNotPassed()
    {
        FinalStatus = InitialStatus;
        var nextPriorityOfferResult = Enrollee.NextPriorityOffer();

        if (nextPriorityOfferResult.NextPriorityOffer is not null)
        {
            nextPriorityOfferResult.NextPriorityOffer.IsWaiting = true;
        }

        return nextPriorityOfferResult;
    }

    public void UnmarkAsWaiting() => IsWaiting = false;

    public void MarkAsAwardedWithScholarship() => HasScholarship = true;

    public void ResetInitialRecommendation()
    {
        if (!HasInterview && InitialStatus == OfferEnrolleeConstants.Statuses.RecommendedBudget)
        {
            InitialStatus = OfferEnrolleeConstants.Statuses.Admitted;
        }
        FinalStatus = InitialStatus;
        HasScholarship = false;
    }
}
