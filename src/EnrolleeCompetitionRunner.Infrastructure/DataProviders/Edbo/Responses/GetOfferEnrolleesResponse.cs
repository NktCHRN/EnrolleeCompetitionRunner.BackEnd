using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Common.Converters;
using System.Text.Json.Serialization;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Responses;
public sealed record GetOfferEnrolleesResponse
{
    [JsonPropertyName("requests")]
    public IEnumerable<OfferEnrollee> Requests { get; init; } = Array.Empty<OfferEnrollee>();
}

public sealed record OfferEnrollee
{
    [JsonPropertyName("prid")]
    public long Code { get; init; }

    [JsonPropertyName("n")]
    public int Position { get; init; }

    [JsonPropertyName("fio")]
    public string EnrolleeName { get; init; } = string.Empty;

    [JsonPropertyName("prsid")]
    public int Status { get; init; }

    [JsonPropertyName("p")]
    [JsonConverter(typeof(ShortOrStringPrimitiveConverter))]
    public short Priority { get; init; }

    [JsonPropertyName("kv")]
    public decimal Score { get; init; }

    [JsonPropertyName("rss")]
    public IEnumerable<OfferEnrolleeCoefficient> Coefficients { get; init; } = new List<OfferEnrolleeCoefficient>();
}

public sealed record OfferEnrolleeCoefficient
{
    [JsonPropertyName("id")]
    public long Code { get; init; }

    [JsonPropertyName("sn")]
    public string? Name { get; init; }

    [JsonPropertyName("f")]
    public string? Formula { get; init; }
}
