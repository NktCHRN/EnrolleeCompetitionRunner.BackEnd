namespace EnrolleeCompetitionRunner.Contracts.Models;
public sealed record EnrolleeModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal UkrainianLanguageExamScore { get; set; }

    public IEnumerable<EnrolleeOfferWithOfferInfoModel> Offers { get; set; } = Array.Empty<EnrolleeOfferWithOfferInfoModel>();
}
