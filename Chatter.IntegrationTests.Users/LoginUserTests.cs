using System.Net;
using System.Net.Http.Json;
using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.IntegrationTests.Users.Infrastructure;
using Chatter.Users.Application.Users.Errors;
using FluentAssertions;
using Xunit;

namespace Chatter.IntegrationTests.Users;

[Collection("UsersApi")]
public class LoginUserTests
{
    private readonly HttpClient _client;

    public LoginUserTests(UsersApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task LoginUser_ValidCredentials_Returns200WithToken()
    {
        var response = await _client.PostAsJsonAsync("/api/users/login", new
        {
            Username = KeycloakTestUsersData.TestUsername,
            Password = KeycloakTestUsersData.TestPassword
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("accessToken");
        body.Should().Contain("refreshToken");
    }

    [Fact]
    public async Task LoginUser_InvalidPassword_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/users/login", new
        {
            Username = KeycloakTestUsersData.TestUsername,
            Password = "wrong-password"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(ErrorMessages.InvalidUsernameOrPassword);
    }

    [Fact]
    public async Task LoginUser_NonExistentUser_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/users/login", new
        {
            Username = "nonexistent",
            Password = "password123"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LoginUser_EmptyUsername_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/users/login", new
        {
            Username = "",
            Password = "password123"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(ValidationMessages.UsernameRequired);
    }

    [Fact]
    public async Task LoginUser_EmptyPassword_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/users/login", new
        {
            Username = "user",
            Password = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(ValidationMessages.PasswordRequired);
    }
}