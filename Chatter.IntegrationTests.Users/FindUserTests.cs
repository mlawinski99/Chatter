using System.Net;
using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.IntegrationTests.Users.Infrastructure;
using Chatter.Users.Application.Users.Queries;
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
        var client = _fixture.Api.CreateAuthenticatedClient();

        var response = await client.GetAsync(
            $"/api/users/search?search={KeycloakTestUsersData.TestUsername}");

        var result = await response.ReadResult<FindUser.FindUserDto>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Username.Should().Be(KeycloakTestUsersData.TestUsername);
        result.Data.Email.Should().Be(KeycloakTestUsersData.TestEmail);
    }

    [Fact]
    public async Task FindUser_ExistingEmail_Returns200WithUser()
    {
        var client = _fixture.Api.CreateAuthenticatedClient();

        var response = await client.GetAsync(
            $"/api/users/search?search={KeycloakTestUsersData.TestEmail}");

        var result = await response.ReadResult<FindUser.FindUserDto>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Username.Should().Be(KeycloakTestUsersData.TestUsername);
    }

    [Fact]
    public async Task FindUser_NonExistentUser_Returns200WithNullData()
    {
        var client = _fixture.Api.CreateAuthenticatedClient();

        var response = await client.GetAsync("/api/users/search?search=nonexistentuser");

        var result = await response.ReadResult<FindUser.FindUserDto>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeNull();
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