using Microsoft.Extensions.DependencyInjection;

namespace Chatter.Shared.CQRS;

public static class CqrsDependencyInstaller
{
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.AddSingleton<IRequestDispatcher, RequestDispatcher>();

        return services;
    }
}