using EnrolleeCompetitionRunner.Core.Dtos;
using MediatR;

namespace EnrolleeCompetitionRunner.Core.Queries;
public sealed record GetEnrolleeByIdQuery : IRequest<EnrolleeDto>
{
    public Guid Id { get; set; }
}
