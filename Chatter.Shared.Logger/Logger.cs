using Microsoft.Extensions.Logging;

namespace Chatter.Shared.Logger;

public class Logger<T> : ILogger<T>
{
    private readonly Microsoft.Extensions.Logging.ILogger<T> _logger;

    public Logger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<T>();
    }
    
    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogError(string message, params object[] args)
    {
        _logger.LogError(message, args);
    }
}