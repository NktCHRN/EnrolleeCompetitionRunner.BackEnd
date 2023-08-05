using AutoMapper;
using EnrolleeCompetitionRunner.Core.Abstractions;
using EnrolleeCompetitionRunner.Core.Dtos;
using EnrolleeCompetitionRunner.Core.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnrolleeCompetitionRunner.Core.Queries;
public sealed class GetEnrolleeByIdQueryHandler : IRequestHandler<GetEnrolleeByIdQuery, EnrolleeDto>
{
    private readonly IApplicationDbContext _applicationDbContext;

    private readonly IMapper _mapper;

    public GetEnrolleeByIdQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    public async Task<EnrolleeDto> Handle(GetEnrolleeByIdQuery request, CancellationToken cancellationToken)
    {
        var enrollee = await _applicationDbContext.Enrollees
            .Include(e => e.Offers
                    .OrderBy(o => o.IsContractOnly)
                    .ThenBy(o => o.Priority))
                .ThenInclude(o => o.Offer)
                    .ThenInclude(o => o.University)
            .Include(e => e.Offers
                    .OrderBy(o => o.IsContractOnly)
                    .ThenBy(o => o.Priority))
                .ThenInclude(o => o.Offer)
                    .ThenInclude(o => o.Speciality)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken: cancellationToken)
                ?? throw new EntityNotFoundException($"Enrollee with id '{request.Id}' was not found");

        return _mapper.Map<EnrolleeDto>(enrollee);
    }
}
