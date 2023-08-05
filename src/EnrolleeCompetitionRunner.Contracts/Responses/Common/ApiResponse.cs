namespace EnrolleeCompetitionRunner.Contracts.Responses.Common;

public sealed record ApiResponse<TResult>
{
    public TResult? Result { get; private init; }
    public ErrorResponse? Error { get; private init; }
    public ApiResponse(TResult? result, ErrorResponse? error)
    {
        Result = result;
        Error = error;
    }
};
