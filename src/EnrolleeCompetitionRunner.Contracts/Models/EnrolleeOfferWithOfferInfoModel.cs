namespace EnrolleeCompetitionRunner.Contracts.Models;
public sealed record EnrolleeOfferWithOfferInfoModel
{
    public short? Priority { get; set; }
    public bool IsContractOnly { get; set; }

    public decimal Score { get; set; }

    public bool HasQuote1 { get; set; }
    public bool HasQuote2 { get; set; }
    public bool HasInterview { get; set; }

    public bool Passed { get; set; }
    public bool HasScholarship { get; set; }

    public OfferHeaderModel Offer { get; set; } = new();
}
