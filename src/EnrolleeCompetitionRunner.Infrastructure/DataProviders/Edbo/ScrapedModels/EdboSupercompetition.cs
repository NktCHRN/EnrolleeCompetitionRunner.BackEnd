using System.Text.Json.Serialization;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.ScrapedModels;
public sealed record EdboSupercompetition
{
    [JsonPropertyName("d")]
    public int TotalPlaces { get; init; }
}
