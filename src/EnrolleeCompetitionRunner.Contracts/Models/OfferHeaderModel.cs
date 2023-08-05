namespace EnrolleeCompetitionRunner.Contracts.Models;
public sealed record OfferHeaderModel
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string SpecialityCode { get; set; } = string.Empty;
    public string SpecialityName { get; set; } = string.Empty;

    public string? SpecializationCode { get; set; }
    public string? SpecializationName { get; set; }

    public string? FacultyName { get; set; }

    public string UniversityName { get; set; } = string.Empty;
}
