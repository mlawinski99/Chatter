using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Chatter.Shared.CQRS;

public static class CqrsDependencyInstaller
{
    public static IServiceCollection AddCqrs(this IServiceCollection services, Assembly assembly)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddSingleton<IRequestDispatcher, RequestDispatcher>();

        return services;
    }
}