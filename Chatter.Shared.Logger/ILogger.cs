namespace Chatter.Shared.Logger;

public interface ILogger<T>
{
    void LogInformation(string message, params object[] args);
    void LogError(string message, params object[] args);
}