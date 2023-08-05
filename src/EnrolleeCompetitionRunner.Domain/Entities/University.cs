using EnrolleeCompetitionRunner.Domain.Abstractions;

namespace EnrolleeCompetitionRunner.Domain.Entities;
public class University : BaseEntity, IEquatable<University>
{
    public string Name { get; private set; }
    public string Code { get; private set; }

    public IReadOnlyList<Offer> Offers => _offers;
    private readonly List<Offer> _offers = new();

    private University()
    {
        Name = string.Empty;
        Code = string.Empty;
    }

    public University(string name, string code)
    {
        Name = name;
        Code = code;
    }

    public static bool operator ==(University left, University right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(University left, University right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as University);
    }

    public bool Equals(University? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other))
            return true;

        return Code == other.Code;
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }

    public void AddOffer(Offer offer)
    {
        _offers.Add(offer);
    }
}
