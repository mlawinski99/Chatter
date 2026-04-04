using System.Net.Http.Headers;
using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.IntegrationTests.Shared.Infrastructure.Containers;
using Chatter.MessagesDataAccess.DbContexts;
using Chatter.MessagesService.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Chatter.IntegrationTests.Messages.Infrastructure;

public class MessagesApiFactory : WebApplicationFactory<MessagesController>, IAsyncLifetime
{
    private readonly PostgresContainerFixture _postgresFixture = new("messagesdb");

    public async Task InitializeAsync()
    {
        await _postgresFixture.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        MessagesDbSeeder.Seed(db);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            var settingsPath = Path.Combine(AppContext.BaseDirectory, "Settings", "test-settings.json");
            config.AddJsonFile(settingsPath, optional: false);

            var solutionDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
            var migrationPath = Path.Combine(solutionDir, "src", "Chatter.Messages.DataAccess", "Migrations");

            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:MessagesDb"] = _postgresFixture.ConnectionString,
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