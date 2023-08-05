using EnrolleeCompetitionRunner.Core.Dtos;
using MediatR;

namespace EnrolleeCompetitionRunner.Core.Queries;
public sealed record SearchEnrolleesQuery : IRequest<EnrolleesSearchResultsDto>
{
    public string Name { get; set; } = string.Empty;

    public decimal Score { get; set; }
}
