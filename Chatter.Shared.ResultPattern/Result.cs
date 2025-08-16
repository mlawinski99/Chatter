namespace Chatter.Shared.ResultPattern;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? Error { get; private set; }

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