using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.Shared.KeycloakService;
using Chatter.SyncKeycloakEventsJob;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Xunit;

namespace Chatter.IntegrationTests.Shared.Fixtures;

public class KeycloakEventSyncTestFixture : IntegrationTestFixtureBase, IAsyncLifetime
{
    private readonly IContainer _keycloakContainer;
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly IHttpClientFactory _httpClientFactory = new KeycloakHttpClientFactory();

    public string KeycloakBaseUrl => $"http://{_keycloakContainer.Hostname}:{_keycloakContainer.GetMappedPublicPort(8080)}";
    public string KeycloakRealm => "test-realm";
    public string KeycloakClientId => "test-client";
    public string KeycloakClientSecret => "test-secret";

    protected override string PostgresConnectionString => _postgresContainer.GetConnectionString();

    public const string TestUserId = "550e8400-e29b-41d4-a716-446655440001";
    public const string TestUsername = "testuser";
    public const string TestEmail = "testuser@test.com";
    public const string TestUserDeleteId = "11111111-1111-1111-1111-111111111111";

    public KeycloakEventSyncTestFixture()
    {
        var realmPath = Path.Combine(AppContext.BaseDirectory, "TestData", "test-realm.json");
        var realmDir = Path.GetDirectoryName(realmPath)!;

        _keycloakContainer = new ContainerBuilder()
            .WithImage("quay.io/keycloak/keycloak:26.0")
            .WithPortBinding(8080, true)
            .WithResourceMapping(realmDir, "/opt/keycloak/data/import")
            .WithEnvironment("KEYCLOAK_ADMIN", "admin")
            .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", "admin")
            .WithCommand("start-dev", "--import-realm")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r
                .ForPath("/realms/test-realm")
                .ForPort(8080)))
            .Build();

        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:18")
            .WithDatabase("synctestdb")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _keycloakContainer.StartAsync(),
            _postgresContainer.StartAsync()
        );

        await InitializeDatabaseAsync();
    }

    private async Task InitializeDatabaseAsync()
    {
        await using var context = CreateDbContext();
        await context.Database.EnsureCreatedAsync();
    }

    public IKeycloakService CreateKeycloakService()
    {
        var config = Options.Create(new KeycloakConfig
        {
            AuthServerUrl = KeycloakBaseUrl,
            Realm = KeycloakRealm,
            ClientId = KeycloakClientId,
            ClientSecret = KeycloakClientSecret
        });

        return new KeycloakService(_httpClientFactory, config, new TestJsonSerializer());
    }

    public KeycloakConfig CreateKeycloakConfig()
    {
        return new KeycloakConfig
        {
            AuthServerUrl = KeycloakBaseUrl,
            Realm = KeycloakRealm,
            ClientId = KeycloakClientId,
            ClientSecret = KeycloakClientSecret,
            ConnectionStrings = new List<string> { PostgresConnectionString }
        };
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            _keycloakContainer.DisposeAsync().AsTask(),
            _postgresContainer.DisposeAsync().AsTask()
        );
    }
}
