using System.Text.Json.Serialization;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Requests;
public sealed record SearchUniversitiesRequest
{
    [JsonPropertyName("speciality")]
    public string SpecialityCodeWithInternalSpecializationCode { get; init; } = string.Empty;

    [JsonPropertyName("education_base")]
    public string EnrollmentBasisCode { get; init; } = string.Empty;
    [JsonPropertyName("qualification")]
    public string EducationalStageCode { get; init; } = string.Empty;

    [JsonPropertyName("region")]
    public string Region { get; init; } = string.Empty;

    [JsonPropertyName("university")]
    public string University { get; init; } = string.Empty;

    [JsonPropertyName("study_program")]
    public string StudyProgram { get; init; } = string.Empty;

    [JsonPropertyName("education_form")]
    public string EducationForm { get; init; } = string.Empty;

    [JsonPropertyName("course")]
    public string Course { get; init; } = string.Empty;
}
