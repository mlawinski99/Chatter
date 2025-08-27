namespace Chatter.Shared.ResultPattern;


public class Result
{
    public bool IsSuccess { get; protected set; }
    public string? Error { get; protected set; }

    public static Result Success => new() { IsSuccess = true };
    public static Result Failure(string error) => new() { IsSuccess = false, Error = error };
}

public class Result<T> : Result
{
    public T? Data { get; private set; }

    private Result(bool isSuccess, T? data, string? error)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
    }
    
    public static Result<T> Success(T? data = default)
    {
        return new Result<T>(true, data, null);
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(false, default, error);
    }
}