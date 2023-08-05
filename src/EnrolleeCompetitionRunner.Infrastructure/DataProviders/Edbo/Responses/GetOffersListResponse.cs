using System.Text.Json.Serialization;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Responses;
public sealed record GetOffersListResponse
{
    [JsonPropertyName("offers")]
    public IEnumerable<OfferInfo> Offers { get; init; } = new List<OfferInfo>();
}

public sealed record OfferInfo
{
    [JsonPropertyName("usid")]
    public long Code { get; init; }

    [JsonPropertyName("usn")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("ox")]
    public int BudgetPlaces { get; init; }

    // Also, we have to get number of quote places.

    [JsonPropertyName("uid")]
    public long UniversityCode { get; init; }
    [JsonPropertyName("ssc")]
    public string SpecialityCode { get; init; } = string.Empty;
    [JsonPropertyName("szc")]
    public string SpecialityCodeAndSpecializationCode { get; init; } = string.Empty;        // Will be empty if there is no specialization.

    [JsonPropertyName("ufn")]
    public string? FacultyName { get; init; }

    [JsonPropertyName("ebid")]
    public long EnrollmentBasisCode { get; init; }
    [JsonPropertyName("qid")]
    public string EducationalStageCode { get; init; } = string.Empty;

    [JsonPropertyName("os")]
    public IDictionary<string, SubjectInfo> SubjectsInfo { get; init; } = new Dictionary<string, SubjectInfo>();         // Like
    /*
     * "56534": {
                    "sn": "Українська мова",
                    "k": 0.3,
                    "efid": 3,
                    "t": "Сертифікат НМТ 2022-2023 року або сертифікат ЗНО 2020-2021 року",
                    "icon": "3n",
                    "ch": 0,
                    "mv": 100
                },
                "56537": {
                    "sn": "Математика",
                    "k": 0.5,
                    "efid": 3,
                    "t": "Сертифікат НМТ 2022-2023 року або сертифікат ЗНО 2020-2021 року",
                    "icon": "3n",
                    "ch": 0,
                    "mv": 100
                },
                "56535": {
                    "sn": "Іноземна мова",
                    "k": 0.3,
                    "efid": 3,
                    "t": "Сертифікат НМТ 2022-2023 року або сертифікат ЗНО 2020-2021 року",
                    "icon": "3n",
                    "ch": 1,
                    "mv": 100
                },
                "745829": {
                    "sn": "Українська мова",
                    "k": 0.3,
                    "efid": 3,
                    "t": "Сертифікат ЗНО 2020-2021 року",
                    "icon": "3",
                    "ch": 0,
                    "mv": 100
                },
                "745837": {
                    "sn": "Математика",
                    "k": 0.5,
                    "efid": 3,
                    "t": "Сертифікат ЗНО 2020-2021 року",
                    "icon": "3",
                    "ch": 0,
                    "mv": 100
                },
                "745842": {
                    "sn": "Іноземна мова",
                    "k": 0.2,
                    "efid": 3,
                    "t": "Сертифікат ЗНО 2020-2021 року",
                    "icon": "3",
                    "ch": 1,
                    "mv": 100
                },
     */
}

public sealed record SubjectInfo
{
    [JsonPropertyName("sn")]
    public string Name { get; init; } = string.Empty;
}
