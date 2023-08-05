using EnrolleeCompetitionRunner.Api.Middlewares;

namespace EnrolleeCompetitionRunner.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
