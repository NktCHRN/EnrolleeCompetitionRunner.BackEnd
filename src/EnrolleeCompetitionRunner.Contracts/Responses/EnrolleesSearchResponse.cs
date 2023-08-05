using EnrolleeCompetitionRunner.Contracts.Models;

namespace EnrolleeCompetitionRunner.Contracts.Responses;
public sealed record EnrolleesSearchResponse
{
    public IEnumerable<EnrolleeModel> Enrollees { get; set; } = Array.Empty<EnrolleeModel>();
}
