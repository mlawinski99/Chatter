using System.Net;
using System.Net.Http.Json;
using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.IntegrationTests.Users.Infrastructure;
using Chatter.Users.Application.Users.Errors;
using FluentAssertions;
using Xunit;

namespace Chatter.IntegrationTests.Users;

[Collection("UsersApi")]
public class LogoutUserTests
{
    private readonly UsersApiFactory _factory;

    public LogoutUserTests(UsersApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task LogoutUser_ValidRefreshToken_Returns200()
    {
        var (client, tokens) = await _factory.CreateAuthenticatedClientWithTokensAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.PostAsJsonAsync("/api/users/logout", new
        {
            RefreshToken = tokens.RefreshToken
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task LogoutUser_EmptyRefreshToken_Returns400()
    {
        var client = await _factory.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.PostAsJsonAsync("/api/users/logout", new
        {
            RefreshToken = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(ValidationMessages.RefreshTokenRequired);
    }

    [Fact]
    public async Task LogoutUser_InvalidRefreshToken_Returns400()
    {
        var client = await _factory.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.PostAsJsonAsync("/api/users/logout", new
        {
            RefreshToken = "invalid-refresh-token"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task LogoutUser_Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/users/logout", new
        {
            RefreshToken = "some-token"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}