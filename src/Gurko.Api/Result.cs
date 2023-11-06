namespace Gurko.Api;

public class Result
{
    public ResultStatus Status { get; init; }
    public string? Error { get; set; }

    protected Result()
    {
    }

    public static Result Ok() => new Result
    {
        Status = ResultStatus.Ok
    };

    public static Result Fail(string error) => new Result
    {
        Error = error,
        Status = ResultStatus.Failure
    };
}

public class Result<T> : Result
{
    public T? Value { get; private init; }

    private Result()
    {
    }

    public static Result<T> Ok(T value) => new()
    {
        Value = value,
        Status = ResultStatus.Ok
    };
    
    public new static Result<T> Fail(string error) => new Result<T>
    {
        Error = error,
        Status = ResultStatus.Failure
    };

    public static Result<T> Created(T value) => new()
    {
        Value = value,
        Status = ResultStatus.Created
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
        ResultStatus.Failure => Results.BadRequest(new ErrorBody(result.Error!)),
        ResultStatus.Created => Results.Created($"{location}/{result.Value}", null),
        _ => throw new ArgumentOutOfRangeException(nameof(result.Status), result.Status, null)
    };
    
    public static IResult ToHttpResult(this Result result) => result.Status switch
    {
        ResultStatus.Ok => Results.NoContent(),
        ResultStatus.Failure => Results.BadRequest(new ErrorBody(result.Error!)),
        _ => throw new ArgumentOutOfRangeException(nameof(result.Status), result.Status, null)
    };
}