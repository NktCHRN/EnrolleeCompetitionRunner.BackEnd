using EnrolleeCompetitionRunner.Domain.Abstractions;
using EnrolleeCompetitionRunner.Domain.Constants;
using EnrolleeCompetitionRunner.Domain.UtilityModels;

namespace EnrolleeCompetitionRunner.Domain.Entities;
public class Enrollee : BaseEntity, IEquatable<Enrollee>
{
    public string Name { get; private set; }

    public decimal UkrainianLanguageExamScore { get; private set; }

    public IReadOnlyList<OfferEnrollee> Offers => _offers;
    private readonly List<OfferEnrollee> _offers = new();

    private int _currentPriority = 0;
    public OfferEnrollee? CurrentPriorityOffer { get; private set; }

    private Enrollee() 
    { 
        Name = string.Empty;
    }

    public Enrollee(string name, decimal ukrainianLanguageExamScore)
    {
        Name = name;
        UkrainianLanguageExamScore = ukrainianLanguageExamScore;
    }

    public static bool operator ==(Enrollee left, Enrollee right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Enrollee left, Enrollee right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Enrollee);
    }

    public bool Equals(Enrollee? other)
    {
        if (other is null) 
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Name == other.Name
            && UkrainianLanguageExamScore == other.UkrainianLanguageExamScore;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, UkrainianLanguageExamScore);
    }

    public void AddOffer(OfferEnrollee offer)
    {
        _offers.Add(offer);
    }

    public SwitchToNextPriorityResult NextPriorityOffer()
    {
        if (Offers.Any(o => o.HasInterview && (o.InitialStatus is OfferEnrolleeConstants.Statuses.RecommendedBudget or OfferEnrolleeConstants.Statuses.IncludedToOrder))
            || _currentPriority > OfferEnrolleeConstants.MaxPriority)
            return new(null);

        do
        {
            _currentPriority++;
            CurrentPriorityOffer = Offers.FirstOrDefault(o => o.Priority == _currentPriority);
        } while (CurrentPriorityOffer == null && _currentPriority <= OfferEnrolleeConstants.MaxPriority);

        return new SwitchToNextPriorityResult(CurrentPriorityOffer);
    }
}
