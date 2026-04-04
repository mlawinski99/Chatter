using Chatter.MessagesDataAccess.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chatter.MessagesDataAccess;

public static class MessagesDataAccessDependencyInstaller
{
    public static IServiceCollection AddMessagesDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ChatDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration["ConnectionStrings:MessagesDb"]);
        });
        return services;
    }
}