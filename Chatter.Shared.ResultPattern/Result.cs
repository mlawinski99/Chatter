namespace Chatter.Shared.ResultPattern;

public class Result<T>
{
    public readonly bool IsSuccess;
    public bool IsFailure => !IsSuccess;
    public readonly T? Data;
    public readonly string? Error;

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