using Chatter.MessagesDataAccess.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Chatter.IntegrationTests.Messages.Infrastructure;

public class MessagesTestFixture : IAsyncLifetime
{
    public MessagesApiFactory Api { get; } = new();

    public ChatDbContext CreateDbContext()
    {
        var scope = Api.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ChatDbContext>();
    }

    public Task InitializeAsync() => Api.InitializeAsync();

    public async Task DisposeAsync() => await Api.DisposeAsync();
}