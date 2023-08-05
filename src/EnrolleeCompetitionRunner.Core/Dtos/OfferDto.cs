namespace EnrolleeCompetitionRunner.Core.Dtos;
public sealed record OfferDto
{
    public int BudgetPlaces { get; set; }
    public int Quote1Places { get; set; }
    public int Quote2Places { get; set; }

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string SpecialityCode { get; set; } = string.Empty;
    public string SpecialityName { get; set; } = string.Empty;

    public string? SpecializationCode { get; set; }
    public string? SpecializationName { get; set; }

    public string? FacultyName { get; set; }

    public string UniversityName { get; set; } = string.Empty;

    public int TotalPassedEnrollees { get; set; }

    public decimal Quote1PassingScore { get; set; }
    public decimal Quote2PassingScore { get; set; }
    public decimal GeneralPassingScore { get; set; }

    public decimal MinScholarshipScore { get; set; }

    public IEnumerable<OfferEnrolleeDto> PassedEnrollees { get; set; } = Array.Empty<OfferEnrolleeDto>();
}
