using System.Net.Http.Headers;
using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.IntegrationTests.Shared.Infrastructure.Containers;
using Chatter.Shared.KeycloakService;
using Chatter.SyncKeycloakEventsJob;
using Chatter.Users.DataAccess.DbContexts;
using Chatter.UsersService.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Chatter.IntegrationTests.Users.Infrastructure;

public class UsersApiFactory : WebApplicationFactory<UsersController>, IAsyncLifetime
{
    private readonly KeycloakContainerFixture _keycloakFixture = new();
    private readonly PostgresContainerFixture _postgresFixture = new("usersdb");

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _keycloakFixture.StartAsync(),
            _postgresFixture.StartAsync()
        );

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        await db.Database.EnsureCreatedAsync();
        UsersDbSeeder.Seed(db);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            var settingsPath = Path.Combine(AppContext.BaseDirectory, "Settings", "test-settings.json");
            config.AddJsonFile(settingsPath, optional: false);

            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:UsersDb"] = _postgresFixture.ConnectionString,
                ["Keycloak:AuthServerUrl"] = _keycloakFixture.BaseUrl,
                ["Keycloak:Authority"] = $"{_keycloakFixture.BaseUrl}/realms/{_keycloakFixture.Realm}",
                ["Keycloak:Realm"] = _keycloakFixture.Realm,
                ["Keycloak:ClientId"] = _keycloakFixture.ClientId,
                ["Keycloak:ClientSecret"] = _keycloakFixture.ClientSecret,
            });
        });

        builder.ConfigureServices(services =>
        {
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var authority = $"{_keycloakFixture.BaseUrl}/realms/{_keycloakFixture.Realm}";
                options.Authority = authority;
                options.MetadataAddress = $"{authority}/.well-known/openid-configuration";
                options.RequireHttpsMetadata = false;
                options.BackchannelHttpHandler = new HttpClientHandler();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                };
            });

        });
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync(string username, string password)
    {
        var (client, _) = await CreateAuthenticatedClientWithTokensAsync(username, password);
        return client;
    }

    public async Task<(HttpClient Client, KeycloakTokenResponse Tokens)> CreateAuthenticatedClientWithTokensAsync(
        string username, string password)
    {
        var keycloakService = CreateKeycloakService();
        var tokenResponse = await keycloakService.LoginUser(username, password);

        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenResponse!.AccessToken);
        return (client, tokenResponse);
    }

    private IKeycloakService CreateKeycloakService()
    {
        var config = Options.Create(new KeycloakConfig
        {
            AuthServerUrl = _keycloakFixture.BaseUrl,
            Realm = _keycloakFixture.Realm,
            ClientId = _keycloakFixture.ClientId,
            ClientSecret = _keycloakFixture.ClientSecret
        });

        return new KeycloakService(
            new TestHttpClientFactory(), config, new TestJsonSerializer());
    }

    public new async Task DisposeAsync()
    {
        await Task.WhenAll(
            _keycloakFixture.DisposeAsync().AsTask(),
            _postgresFixture.DisposeAsync().AsTask()
        );
        await base.DisposeAsync();
    }
}