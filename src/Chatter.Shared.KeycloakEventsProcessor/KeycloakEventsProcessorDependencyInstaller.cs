using Chatter.Shared.Context;
using Chatter.Shared.UserEventsProcessor;
using Core.DataAccessTypes;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class KeycloakEventsProcessorDependencyInstaller
{
    public static IServiceCollection AddKeycloakEventProcessor<TContext>(
        this IServiceCollection services, IConfiguration configuration, IRecurringJobManager recurringJobManager) where TContext : BaseDbContext, IUserContext
    {
        services.Configure<KeycloakConfiguration>(
            configuration.GetSection(KeycloakConfiguration.SectionName));
        
        recurringJobManager.AddOrUpdate<KeycloakEventProcessor<TContext>>(
            "keycloak-user-sync-job",
                job => job.Run(),
            "*/30 * * * * *",  //@TODO move cron to config
            TimeZoneInfo.Utc
        );
        
        return services;
    }
}


