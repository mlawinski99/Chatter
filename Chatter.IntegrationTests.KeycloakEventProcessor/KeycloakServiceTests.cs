using Chatter.IntegrationTests.Shared;
using Chatter.IntegrationTests.Shared.Fixtures;
using FluentAssertions;
using Xunit;

namespace Chatter.IntegrationTests;

[Collection("KeycloakEventSync")]
public class KeycloakServiceTests : IntegrationTestBase<KeycloakEventSyncTestFixture>
{
    public KeycloakServiceTests(KeycloakEventSyncTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetToken_ShouldReturnValidToken()
    {
        // Arrange
        var keycloakService = Fixture.CreateKeycloakService();

        // Act
        var token = await keycloakService.GetToken();

        // Assert
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetUser_WithExistingUser_ShouldReturnUser()
    {
        // Arrange
        var keycloakService = Fixture.CreateKeycloakService();
        var token = await keycloakService.GetToken();

        // Act
        var user = await keycloakService.GetUser(token, KeycloakEventSyncTestFixture.TestUserId);

        // Assert
        user.Should().NotBeNull();
        user.Username.Should().Be(KeycloakEventSyncTestFixture.TestUsername);
    }

    [Fact]
    public async Task GetUser_WithNonExistingUser_ShouldReturnNull()
    {
        // Arrange
        var keycloakService = Fixture.CreateKeycloakService();
        var token = await keycloakService.GetToken();

        // Act
        var user = await keycloakService.GetUser(token, Guid.NewGuid().ToString());

        // Assert
        user.Should().BeNull();
    }

    [Fact]
    public async Task CreateUser_ShouldCreateUserInKeycloak()
    {
        // Arrange
        var keycloakService = Fixture.CreateKeycloakService();
        var token = await keycloakService.GetToken();

        // Act
        var act = () => keycloakService.CreateUser(token, "testUserCreate", "testUserCreate@test.com");

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateUserEmail()
    {
        // Arrange
        var keycloakService = Fixture.CreateKeycloakService();
        var token = await keycloakService.GetToken();

        // Act
        var act = () => keycloakService.UpdateUser(token, KeycloakEventSyncTestFixture.TestUserId, "updatedEmail@test.com");

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteUser_ShouldDeleteUserFromKeycloak()
    {
        // Arrange
        var keycloakService = Fixture.CreateKeycloakService();
        var token = await keycloakService.GetToken();
        var userIdToDelete = KeycloakEventSyncTestFixture.TestUserDeleteId;

        // Act
        var act = () => keycloakService.DeleteUser(token, userIdToDelete);

        // Assert
        await act.Should().NotThrowAsync();

        var deletedUser = await keycloakService.GetUser(token, userIdToDelete);
        deletedUser.Should().BeNull();
    }
}
