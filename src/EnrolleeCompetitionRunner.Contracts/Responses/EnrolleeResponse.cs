using EnrolleeCompetitionRunner.Contracts.Models;

namespace EnrolleeCompetitionRunner.Contracts.Responses;
public sealed record EnrolleeResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal UkrainianLanguageExamScore { get; set; }

    public IEnumerable<EnrolleeOfferWithOfferInfoModel> Offers { get; set; } = Array.Empty<EnrolleeOfferWithOfferInfoModel>();
}
