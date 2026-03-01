namespace Chatter.Shared.ResultPattern;

public class Result
{
    public bool IsSuccess { get; protected set; }
    public string? Error { get; protected set; }
    public ResultCode Code { get; protected set; }

    public static Result Success => new() { IsSuccess = true, Code = ResultCode.Ok };

    public static Result BadRequest(string error) => new() { IsSuccess = false, Error = error, Code = ResultCode.BadRequest };
    public static Result Unauthorized(string error) => new() { IsSuccess = false, Error = error, Code = ResultCode.Unauthorized };
    public static Result Forbidden(string error) => new() { IsSuccess = false, Error = error, Code = ResultCode.Forbidden };
    public static Result NotFound(string error) => new() { IsSuccess = false, Error = error, Code = ResultCode.NotFound };
    public static Result InternalError(string error) => new() { IsSuccess = false, Error = error, Code = ResultCode.InternalError };
}

public class Result<T> : Result
{
    public T? Data { get; private set; }

    private Result(bool isSuccess, T? data, string? error, ResultCode code)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        Code = code;
    }

    public static Result<T> Success(T? data = default) => new(true, data, null, ResultCode.Ok);

    public static new Result<T> BadRequest(string error) => new(false, default, error, ResultCode.BadRequest);
    public static new Result<T> Unauthorized(string error) => new(false, default, error, ResultCode.Unauthorized);
    public static new Result<T> Forbidden(string error) => new(false, default, error, ResultCode.Forbidden);
    public static new Result<T> NotFound(string error) => new(false, default, error, ResultCode.NotFound);
    public static new Result<T> InternalError(string error) => new(false, default, error, ResultCode.InternalError);
}