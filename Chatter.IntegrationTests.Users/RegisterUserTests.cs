using System.Net;
using System.Net.Http.Json;
using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.IntegrationTests.Users.Infrastructure;
using Chatter.Users.Application.Users.Errors;
using FluentAssertions;
using Xunit;

namespace Chatter.IntegrationTests.Users;

[Collection("UsersApi")]
public class RegisterUserTests
{
    private readonly HttpClient _client;

    public RegisterUserTests(UsersApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterUser_ValidRequest_Returns200()
    {
        var response = await _client.PostAsJsonAsync("/api/users/register", new
        {
            Username = "newuser",
            Password = "password123",
            ConfirmPassword = "password123",
            Email = "newuser@test.com"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterUser_EmptyUsername_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/users/register", new
        {
            Username = "",
            Password = "password123",
            ConfirmPassword = "password123",
            Email = "test@test.com"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(ValidationMessages.UsernameRequired);
    }

    [Fact]
    public async Task RegisterUser_InvalidEmail_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/users/register", new
        {
            Username = "user",
            Password = "password123",
            ConfirmPassword = "password123",
            Email = "invalid-email"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(ValidationMessages.InvalidEmailFormat);
    }

    [Fact]
    public async Task RegisterUser_PasswordTooShort_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/users/register", new
        {
            Username = "user",
            Password = "pass",
            ConfirmPassword = "pass",
            Email = "test@test.com"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(ValidationMessages.PasswordMinLength);
    }

    [Fact]
    public async Task RegisterUser_PasswordsDoNotMatch_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/users/register", new
        {
            Username = "user",
            Password = "password123",
            ConfirmPassword = "anotherpassword123",
            Email = "test@test.com"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(ValidationMessages.PasswordsDoNotMatch);
    }

    [Fact]
    public async Task RegisterUser_DuplicateUsername_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/users/register", new
        {
            Username = KeycloakTestUsersData.TestUsername,
            Password = "password123",
            ConfirmPassword = "password123",
            Email = "test@test.com"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}