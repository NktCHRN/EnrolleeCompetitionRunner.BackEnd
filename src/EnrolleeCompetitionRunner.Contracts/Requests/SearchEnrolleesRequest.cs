namespace EnrolleeCompetitionRunner.Contracts.Requests;
public sealed record SearchEnrolleesRequest
{
    public string Name { get; set; } = string.Empty;

    public decimal Score { get; set; }
}
