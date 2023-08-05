using EnrolleeCompetitionRunner.Api.UtilityMethods;
using EnrolleeCompetitionRunner.Core.Exceptions;
using System.Net;

namespace EnrolleeCompetitionRunner.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            var (statusCode, content) = ex switch
            {
                EntityNotFoundException notFoundEx => (HttpStatusCode.NotFound, notFoundEx.Message),
                _ => (HttpStatusCode.InternalServerError, "Something went wrong :(")
            };

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError("Exception occured: {ex}", ex);
            }

            await httpContext.Response.WriteAsJsonAsync(ApiResponseCreator.FromError(new(content)));
        }
    }
}
