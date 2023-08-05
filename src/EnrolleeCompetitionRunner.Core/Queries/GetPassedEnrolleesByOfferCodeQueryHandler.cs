using AutoMapper;
using EnrolleeCompetitionRunner.Core.Abstractions;
using EnrolleeCompetitionRunner.Core.Dtos;
using EnrolleeCompetitionRunner.Core.Exceptions;
using EnrolleeCompetitionRunner.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnrolleeCompetitionRunner.Core.Queries;
public sealed class GetPassedEnrolleesByOfferCodeQueryHandler : IRequestHandler<GetPassedEnrolleesByOfferCodeQuery, OfferDto>
{
    private readonly IApplicationDbContext _applicationDbContext;

    private readonly IMapper _mapper;

    public GetPassedEnrolleesByOfferCodeQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    public async Task<OfferDto> Handle(GetPassedEnrolleesByOfferCodeQuery request, CancellationToken cancellationToken)
    {
        var offer = await _applicationDbContext.Offers
            .Include(o => o.Enrollees)
                .ThenInclude(e => e.Enrollee)
            .Include(o => o.Speciality)
            .Include(o => o.University)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Code == request.Code, cancellationToken: cancellationToken)
            ?? throw new EntityNotFoundException($"Offer with code '{request.Code}' was not found");

        var offerDto = _mapper.Map<OfferDto>(offer);

        offerDto.PassedEnrollees = offerDto.PassedEnrollees
            .OrderByDescending(e => e.Passed)
            .ThenBy(e => e.SubcompetitionType)                 // Simple workaround as EF cannot compare enums by value when they are stored as strings.
            .ThenByDescending(e => e.Score)
            .ThenBy(e => e.Priority)
            .ToList();

        offerDto.TotalPassedEnrollees = offer.GetPassedEnrolleesCount();
        offerDto.Quote1PassingScore = offer.CalculateQuote1PassingScore();
        offerDto.Quote2PassingScore = offer.CalculateQuote2PassingScore();
        offerDto.GeneralPassingScore = offer.CalculateGeneralPassingScore();

        offerDto.MinScholarshipScore = offer.CalculateMinScholarshipScore();

        return offerDto;
    }
}
