using EnrolleeCompetitionRunner.Api.UtilityMethods;
using Microsoft.AspNetCore.Mvc;

namespace EnrolleeCompetitionRunner.Api.Controllers;

public abstract class BaseController : ControllerBase
{
    protected IActionResult OkResponse() => OkResponse<object?>(new object());

    protected IActionResult OkResponse<T>(T Result)
    {
        return Ok(ApiResponseCreator.FromSuccess(Result));
    }
}
