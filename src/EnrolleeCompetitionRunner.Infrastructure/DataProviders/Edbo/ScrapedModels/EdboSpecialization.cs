using System.Text.Json.Serialization;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.ScrapedModels;
public sealed record EdboSpecialization
{
    [JsonPropertyName("code")]
    public string SpecialityCodeAndSpecializationCode { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}
