using Microsoft.Extensions.DependencyInjection;

namespace Chatter.Shared.Logger;

public static class LoggerDependencyInstaller
{
    public static IServiceCollection AddLogging(
        this IServiceCollection services)
    {
        services.AddScoped(typeof(ILogger<>), typeof(Logger<>));

        return services;
    }
}