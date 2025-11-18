using Microsoft.Extensions.DependencyInjection;

namespace Chatter.OutboxService;

public static class OutboxDependencyInstaller
{
    public static IServiceCollection AddOutbox(
        this IServiceCollection services)
    {
        services.AddSingleton(typeof(IOutboxMessageProcessor<>), typeof(OutboxMessageProcessor<>));

        return services;
    }
}