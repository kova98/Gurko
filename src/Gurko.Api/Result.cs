namespace Gurko.Api;

public class Result<T>
{
    public T? Value { get; private init; }
    public ResultStatus Status { get; private init; }
    public string? Error { get; private set; }

    private Result()
    {
    }

    public static Result<T> Ok(T value) => new()
    {
        Value = value,
        Status = ResultStatus.Ok
    };
    
    public static Result<T> Created(T value) => new()
    {
        Value = value,
        Status = ResultStatus.Created
    };

    public static Result<T> Fail(string error) => new()
    {
        Error = error,
        Status = ResultStatus.Failure
    };
}

public enum ResultStatus
{
    Ok,
    Created,
    Failure
}

public record ErrorBody(string Error);

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result, string? location = null) => result.Status switch
    {
        ResultStatus.Ok => Results.Ok(result.Value),
        ResultStatus.Failure => Results.BadRequest(new ErrorBody(result.Error)),
        ResultStatus.Created => Results.Created($"{location}/{result.Value}", null),
        _ => throw new ArgumentOutOfRangeException(nameof(result.Status), result.Status, null)
    };
}