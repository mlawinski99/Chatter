using Testcontainers.PostgreSql;
using Xunit;

namespace Chatter.IntegrationTests.Shared.Fixtures;

public class IntegrationTestFixture : IntegrationTestFixtureBase, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;

    protected override string PostgresConnectionString => _postgresContainer.GetConnectionString();

    public IntegrationTestFixture()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:18")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        await InitializeDatabaseAsync();
    }

    private async Task InitializeDatabaseAsync()
    {
        await using var context = CreateDbContext();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }
}
