using System.Net.Http.Headers;
using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.IntegrationTests.Shared.Infrastructure.Containers;
using Core.KeycloakService;
using Chatter.UsersService.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Chatter.IntegrationTests.Users.Infrastructure;

public class UsersApiFactory : WebApplicationFactory<UsersController>, IAsyncLifetime
{
    private readonly PostgresContainerFixture _postgresFixture = new("usersdb");

    public IKeycloakService KeycloakService { get; } = Substitute.For<IKeycloakService>();

    public async Task InitializeAsync()
    {
        await _postgresFixture.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Chatter.Users.DataAccess.DbContexts.UsersDbContext>();
        UsersDbSeeder.Seed(db);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            var settingsPath = Path.Combine(AppContext.BaseDirectory, "Settings", "test-settings.json");
            config.AddJsonFile(settingsPath, optional: false);

            var solutionDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            var migrationPath = Path.Combine(solutionDir, "src", "Chatter.Users.DataAccess", "Migrations");

            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:UsersDb"] = _postgresFixture.ConnectionString,
                ["Migration:ScriptPath"] = migrationPath,
                ["Keycloak:AuthServerUrl"] = "http://localhost",
                ["Keycloak:Authority"] = "http://localhost/realms/test",
                ["Keycloak:Realm"] = "test",
                ["Keycloak:ClientId"] = "test",
                ["Keycloak:ClientSecret"] = "test",
            });
        });

        builder.ConfigureServices(services =>
        {
            services.AddSingleton(KeycloakService);

            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, _ => { });
        });
    }

    public HttpClient CreateAuthenticatedClient(string userId = KeycloakTestUsersData.TestUserId)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", userId);
        return client;
    }

    public new async Task DisposeAsync()
    {
        await _postgresFixture.DisposeAsync();
        await base.DisposeAsync();
    }
}