using EnrolleeCompetitionRunner.Core.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EnrolleeCompetitionRunner.Api.Controllers;

[ApiController]
[Route("api/data")]
public sealed class DataController : BaseController
{
    private readonly IMediator _mediator;

    public DataController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Updates data from providers such as edbo and abit-poisk and runs the competition.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateData()
    {
        var command = new UpdateDataCommand();

        await _mediator.Send(command);

        return OkResponse();
    }

    /// <summary>
    /// Runs the competition.
    /// </summary>
    /// <returns></returns>
    [HttpPost("competitions")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> RunCompetitions()
    {
        var command = new RunCompetitionCommand();

        await _mediator.Send(command);

        return OkResponse();
    }
}
