using Chatter.Shared.DataAccessTypes;
using Chatter.Shared.Encryption.JsonSerializable;
using Microsoft.Extensions.DependencyInjection;

namespace Chatter.Shared.Encryption;

public static class InfrastructureDependencyInstaller
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // @TODO split into multiple dll
        services.AddSingleton<IEncryptor, AesEncryptor>();
        services.AddScoped<IUserProvider, UserProvider>();
        services.AddScoped<IJsonSerializer, JsonSerializer>();

        return services;
    }
}