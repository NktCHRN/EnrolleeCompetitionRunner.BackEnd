using EnrolleeCompetitionRunner.Contracts.Responses.Common;

namespace EnrolleeCompetitionRunner.Api.UtilityMethods;

public static class ApiResponseCreator
{
    public static ApiResponse<T> FromSuccess<T>(T result) => new(result, null);
    public static ApiResponse<object?> FromError(ErrorResponse error) => new(null, error);
}
