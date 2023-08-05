using EnrolleeCompetitionRunner.Contracts.Enums;

namespace EnrolleeCompetitionRunner.Contracts.Models;
public sealed record OfferEnrolleeModel
{
    public Guid EnrolleeId { get; set; }
    public string EnrolleeName { get; set; } = string.Empty;

    public short? Priority { get; set; }
    public bool IsContractOnly { get; set; }

    public decimal Score { get; set; }

    public bool HasQuote1 { get; set; }
    public bool HasQuote2 { get; set; }
    public bool HasInterview { get; set; }

    public bool HasScholarship { get; set; }

    public bool Passed { get; set; }

    public SubcompetitionType SubcompetitionType { get; set; }
}
