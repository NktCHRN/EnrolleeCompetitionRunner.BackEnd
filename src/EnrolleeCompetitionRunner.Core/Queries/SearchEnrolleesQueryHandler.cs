using AutoMapper;
using EnrolleeCompetitionRunner.Core.Abstractions;
using EnrolleeCompetitionRunner.Core.Dtos;
using EnrolleeCompetitionRunner.Core.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnrolleeCompetitionRunner.Core.Queries;
public sealed class SearchEnrolleesQueryHandler : IRequestHandler<SearchEnrolleesQuery, EnrolleesSearchResultsDto>
{
    private readonly IApplicationDbContext _applicationDbContext;

    private readonly IMapper _mapper;

    public SearchEnrolleesQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    public async Task<EnrolleesSearchResultsDto> Handle(SearchEnrolleesQuery request, CancellationToken cancellationToken)
    {
        const int maxCount = 10;

        request.Name = request.Name.Trim();
        var separatorIndex = request.Name.IndexOf(' ');
        var surname = separatorIndex > 0 ? request.Name[..separatorIndex] : request.Name;

        var enrollees = await _applicationDbContext.Enrollees
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
            .Where(e => e.Name.ToUpper().Contains(request.Name.ToUpper()) || e.Name.ToUpper().Contains(surname.ToUpper() + ' '))   // Improve this code in the future.
            .OrderBy(e => e.Name.ToUpper().Contains(request.Name.ToUpper()) ? 0 : 1)
            .ThenBy(e => Math.Abs(e.UkrainianLanguageExamScore - request.Score))
            .Take(maxCount)
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);

        return new EnrolleesSearchResultsDto
        {
            Enrollees = _mapper.Map<IEnumerable<EnrolleeDto>>(enrollees)
        };
    }
}
