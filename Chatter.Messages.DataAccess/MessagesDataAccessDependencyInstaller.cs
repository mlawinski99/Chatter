using Chatter.MessagesDataAccess.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chatter.MessagesDataAccess;

public static class MessagesDataAccessDependencyInstaller
{
    public static IServiceCollection AddMessagesDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ChatDbContext>((serviceProvider, options) =>
        {
            var interceptors = serviceProvider.GetServices<IInterceptor>();
    
            options.UseNpgsql(configuration["ConnectionStrings:MessagesDb"]);

            options.AddInterceptors(interceptors);
        });
        return services;
    }
}