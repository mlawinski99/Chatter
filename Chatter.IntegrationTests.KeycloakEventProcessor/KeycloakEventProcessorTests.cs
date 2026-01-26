using Chatter.IntegrationTests.Shared;
using Chatter.IntegrationTests.Shared.Fixtures;
using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.Shared.Domain;
using Chatter.Shared.Logger;
using Chatter.Shared.UserEventsProcessor;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Chatter.IntegrationTests;

[Collection("KeycloakIntegration")]
public class KeycloakEventProcessorTests : IntegrationTestBase<KeycloakIntegrationTestFixture>
{
    public KeycloakEventProcessorTests(KeycloakIntegrationTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Run_WithCreateUserEvent_ShouldSyncUserFromKeycloak()
    {
        // Arrange
        var keycloakEvent = new KeycloakAdminEvent
        {
            OperationType = "CREATE",
            ResourceType = "USER",
            ResourcePath = $"users/{KeycloakIntegrationTestFixture.TestUserId}",
            Time = Fixture.DateTimeProvider.UtcNow,
            IsProcessed = false
        };

        Db.KeycloakAdminEvents.Add(keycloakEvent);
        await Db.SaveChangesAsync();

        var keycloakService = Fixture.CreateKeycloakService();
        var logger = Substitute.For<IAppLogger<KeycloakEventProcessor<TestDbContext>>>();
        var processor = new KeycloakEventProcessor<TestDbContext>(Db, keycloakService, logger);

        // Act
        await processor.Run();

        // Assert
        var user = await Db.Users.FirstOrDefaultAsync(u => u.KeycloakId == Guid.Parse(KeycloakIntegrationTestFixture.TestUserId));
        user.Should().NotBeNull();
        user.UserName.Should().Be(KeycloakIntegrationTestFixture.TestUsername);
        user.Email.Should().Be(KeycloakIntegrationTestFixture.TestEmail);

        var processedEvent = await Db.KeycloakAdminEvents.FirstAsync();
        processedEvent.IsProcessed.Should().BeTrue();
    }

    [Fact]
    public async Task Run_WithUpdateUserEvent_ShouldUpdateExistingUser()
    {
        // Arrange
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            KeycloakId = Guid.Parse(KeycloakIntegrationTestFixture.TestUserId),
            UserName = "oldusername",
            Email = "old@test.com"
        };
        Db.Users.Add(existingUser);

        var keycloakEvent = new KeycloakAdminEvent
        {
            OperationType = "UPDATE",
            ResourceType = "USER",
            ResourcePath = $"users/{KeycloakIntegrationTestFixture.TestUserId}",
            Time = Fixture.DateTimeProvider.UtcNow,
            IsProcessed = false
        };
        Db.KeycloakAdminEvents.Add(keycloakEvent);
        await Db.SaveChangesAsync();

        var keycloakService = Fixture.CreateKeycloakService();
        var logger = Substitute.For<IAppLogger<KeycloakEventProcessor<TestDbContext>>>();
        var processor = new KeycloakEventProcessor<TestDbContext>(Db, keycloakService, logger);

        // Act
        await processor.Run();

        // Assert
        var user = await Db.Users.FirstOrDefaultAsync(u => u.KeycloakId == Guid.Parse(KeycloakIntegrationTestFixture.TestUserId));
        user.Should().NotBeNull();
        user.UserName.Should().Be(KeycloakIntegrationTestFixture.TestUsername);
        user.Email.Should().Be(KeycloakIntegrationTestFixture.TestEmail);
    }

    [Fact]
    public async Task Run_WithDeleteUserEvent_ShouldDeleteUser()
    {
        // Arrange
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            KeycloakId = Guid.Parse(KeycloakIntegrationTestFixture.TestUserId),
            UserName = KeycloakIntegrationTestFixture.TestUsername,
            Email = KeycloakIntegrationTestFixture.TestEmail
        };
        Db.Users.Add(existingUser);

        var keycloakEvent = new KeycloakAdminEvent
        {
            OperationType = "DELETE",
            ResourceType = "USER",
            ResourcePath = $"users/{KeycloakIntegrationTestFixture.TestUserId}",
            Time = Fixture.DateTimeProvider.UtcNow,
            IsProcessed = false
        };
        Db.KeycloakAdminEvents.Add(keycloakEvent);
        await Db.SaveChangesAsync();

        var keycloakService = Fixture.CreateKeycloakService();
        var logger = Substitute.For<IAppLogger<KeycloakEventProcessor<TestDbContext>>>();
        var processor = new KeycloakEventProcessor<TestDbContext>(Db, keycloakService, logger);

        // Act
        await processor.Run();

        // Assert
        var user = await Db.Users.FirstOrDefaultAsync(u => u.KeycloakId == Guid.Parse(KeycloakIntegrationTestFixture.TestUserId));
        user.Should().BeNull();

        var processedEvent = await Db.KeycloakAdminEvents.FirstAsync();
        processedEvent.IsProcessed.Should().BeTrue();
    }

    [Fact]
    public async Task Run_WithNoEvents_ShouldNotThrow()
    {
        // Arrange
        var keycloakService = Fixture.CreateKeycloakService();
        var logger = Substitute.For<IAppLogger<KeycloakEventProcessor<TestDbContext>>>();
        var processor = new KeycloakEventProcessor<TestDbContext>(Db, keycloakService, logger);

        // Act
        var act = () => processor.Run();

        // Assert
        await act.Should().NotThrowAsync();
    }
}
