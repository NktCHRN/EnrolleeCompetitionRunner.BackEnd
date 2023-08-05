using System.Text.Json.Serialization;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Responses;
public sealed record SearchUniversitiesResponse
{
    [JsonPropertyName("universities")]
    public IEnumerable<University> Universities { get; init; } = new List<University>();
}

public sealed record University
{
    [JsonPropertyName("uid")]
    public long Code { get; init; }

    [JsonPropertyName("un")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("ids")]
    public string OfferCodes { get; init; } = string.Empty;
}
