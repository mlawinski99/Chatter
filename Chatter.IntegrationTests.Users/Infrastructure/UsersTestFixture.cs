using Chatter.Shared.KeycloakService;
using Chatter.Users.DataAccess.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Chatter.IntegrationTests.Users.Infrastructure;

public class UsersTestFixture : IAsyncLifetime
{
    public UsersApiFactory Api { get; } = new();

    public IKeycloakService KeycloakService => Api.KeycloakService;

    public UsersDbContext CreateDbContext()
    {
        var scope = Api.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    }

    public Task InitializeAsync() => Api.InitializeAsync();

    public async Task DisposeAsync() => await Api.DisposeAsync();
}