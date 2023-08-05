using System.Text.Json.Serialization;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.ScrapedModels;
public sealed record EdboGoItem
{
    [JsonPropertyName("items")]
    public IDictionary<string, string> SpecialitiesAndSpecializationsWithGlobalOrderCodes { get; init; } = new Dictionary<string, string>();
    /*
     *      For example:
     *       "101": "2248",
     *       "103": "2250",
     *       "113": "2254",
     *       "121": "2254",
     *       "122": "2254",
     *       "123": "2254",
     *       "124": "2254",
     *       "125": "2254",
     *       "126": "2254",
     *       "131": "2255",
     */
}
