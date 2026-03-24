using System.Net;
using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.IntegrationTests.Users.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Chatter.IntegrationTests.Users;

[Collection("UsersApi")]
public class FindUserTests
{
    private readonly UsersTestFixture _fixture;

    public FindUserTests(UsersTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task FindUser_ExistingUsername_Returns200WithUser()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.GetAsync(
            $"/api/users/search?search={KeycloakTestUsersData.TestUsername}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(KeycloakTestUsersData.TestUsername);
        body.Should().Contain(KeycloakTestUsersData.TestEmail);
    }

    [Fact]
    public async Task FindUser_ExistingEmail_Returns200WithUser()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.GetAsync(
            $"/api/users/search?search={KeycloakTestUsersData.TestEmail}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(KeycloakTestUsersData.TestUsername);
        body.Should().Contain(KeycloakTestUsersData.TestEmail);
    }

    [Fact]
    public async Task FindUser_NonExistentUser_Returns200WithNullData()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.GetAsync("/api/users/search?search=nonexistentuser");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"data\":null");
    }

    [Fact]
    public async Task FindUser_Unauthenticated_Returns401()
    {
        var client = _fixture.Api.CreateClient();

        var response = await client.GetAsync(
            $"/api/users/search?search={KeycloakTestUsersData.TestUsername}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}