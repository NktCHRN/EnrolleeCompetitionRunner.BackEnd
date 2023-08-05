using EnrolleeCompetitionRunner.Contracts.Responses.Common;
using EnrolleeCompetitionRunner.Contracts.Responses;
using EnrolleeCompetitionRunner.Core.Queries;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MediatR;
using EnrolleeCompetitionRunner.Contracts.Requests;

namespace EnrolleeCompetitionRunner.Api.Controllers;

[ApiController]
[Route("api/enrollees")]
public class EnrolleesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public EnrolleesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<EnrolleeResponse>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetEnrolleeById(Guid id)
    {
        var query = new GetEnrolleeByIdQuery
        {
            Id = id
        };

        var dto = await _mediator.Send(query);

        var response = _mapper.Map<EnrolleeResponse>(dto);

        return OkResponse(response);
    }

    /// <summary>
    /// Search enrollees by name (like Петренко Л. О.) and ukrainian exam (ЗНО/НМТ, Українська мова /Украънська мова та література) score.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<EnrolleesSearchResponse>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> SearchEnrollees([FromQuery] SearchEnrolleesRequest request)
    {
        var query = _mapper.Map<SearchEnrolleesQuery>(request);

        var dto = await _mediator.Send(query);

        var response = _mapper.Map<EnrolleesSearchResponse>(dto);

        return OkResponse(response);
    }
}
