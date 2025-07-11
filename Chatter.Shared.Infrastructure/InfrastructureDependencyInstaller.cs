using Chatter.Shared.DataAccessTypes;
using Microsoft.Extensions.DependencyInjection;

namespace Chatter.Shared.Encryption;

public static class InfrastructureDependencyInstaller
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IEncryptor, AesEncryptor>();
        services.AddScoped<IUserProvider, UserProvider>();

        return services;
    }
}