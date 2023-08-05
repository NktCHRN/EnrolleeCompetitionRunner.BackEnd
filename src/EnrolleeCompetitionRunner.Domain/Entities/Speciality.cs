using EnrolleeCompetitionRunner.Domain.Abstractions;

namespace EnrolleeCompetitionRunner.Domain.Entities;
public class Speciality : BaseEntity, IEquatable<Speciality>
{
    public string Code { get; private set; }
    public string Name { get; private set; }

    public string? SpecializationInternalCode { get; private set; }
    public string? SpecializationCode { get; private set; }
    public string? SpecializationName { get; private set; }

    public Guid SupercompetitionId { get; set; }
    public Supercompetition Supercompetition { get; private set; }

    public IReadOnlyList<Offer> Offers => _offers;
    private readonly List<Offer> _offers = new();

    private Speciality() 
    {
        Code = string.Empty;
        Name = string.Empty;
        Supercompetition = null!;
    }

    public Speciality(string code, string name, string? specializationInternalCode, string? specializationCode, string? specializationName, Supercompetition supercompetition)
    {
        Code = code;
        Name = name;
        SpecializationInternalCode = specializationInternalCode;
        SpecializationCode = specializationCode;
        SpecializationName = specializationName;
        SupercompetitionId = supercompetition.Id;
        Supercompetition = supercompetition;
    }

    public void AddOffer(Offer offer) => _offers.Add(offer);

    public static bool operator ==(Speciality left, Speciality right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Speciality left, Speciality right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Speciality);
    }

    public bool Equals(Speciality? other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other))
            return true;

        return Code == other.Code && SpecializationCode == other.SpecializationCode;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Code, SpecializationCode);
    }

    public bool HasSpecialization()
    {
        return !string.IsNullOrEmpty(SpecializationCode);
    }
}
