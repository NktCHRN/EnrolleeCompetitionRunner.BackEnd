using AutoMapper;
using EnrolleeCompetitionRunner.Contracts.Responses;
using EnrolleeCompetitionRunner.Contracts.Responses.Common;
using EnrolleeCompetitionRunner.Core.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EnrolleeCompetitionRunner.Api.Controllers;

[ApiController]
[Route("api/offers")]
public sealed class OffersController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public OffersController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("{code}")]
    [ProducesResponseType(typeof(ApiResponse<PassedEnrolleesByOfferCodeResponse>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetPassedEnrollees(string code)
    {
        var query = new GetPassedEnrolleesByOfferCodeQuery
        {
            Code = code
        };

        var dto = await _mediator.Send(query);

        var response = _mapper.Map<PassedEnrolleesByOfferCodeResponse>(dto);

        return OkResponse(response);
    }
}
