using Chatter.Users.DataAccess.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chatter.Users.DataAccess;

public static class UsersDataAccessDependencyInstaller
{
    public static IServiceCollection AddUsersDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(configuration["ConnectionStrings:UsersDb"]);
        });

        return services;
    }
}