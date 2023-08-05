using EnrolleeCompetitionRunner.Core.Dtos;
using MediatR;

namespace EnrolleeCompetitionRunner.Core.Queries;
public sealed record GetPassedEnrolleesByOfferCodeQuery : IRequest<OfferDto>
{
    public string Code { get; init; } = string.Empty;
}
