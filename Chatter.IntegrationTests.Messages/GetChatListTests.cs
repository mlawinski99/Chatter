using System.Net;
using Chatter.IntegrationTests.Messages.Infrastructure;
using Chatter.IntegrationTests.Shared.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Chatter.IntegrationTests.Messages;

[Collection("MessagesApi")]
public class GetChatListTests
{
    private readonly MessagesTestFixture _fixture;

    public GetChatListTests(MessagesTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetChatList_DefaultPagination_Returns200WithUserChats()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.GetAsync("/Chats");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var db = _fixture.CreateDbContext();
        var userChatCount = await db.ChatMembers
            .AsNoTracking()
            .CountAsync(cm => cm.User.Id == MessagesDbSeeder.TestUser1Id);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain($"\"totalCount\":{userChatCount}");
    }

    [Fact]
    public async Task GetChatList_WithPagination_Returns200()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.GetAsync("/Chats?page=1&pageSize=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"page\":1");
        body.Should().Contain("\"pageSize\":2");
    }

    [Fact]
    public async Task GetChatList_IncludesLastMessage_Returns200()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.GetAsync("/Chats");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("lastMessage");
    }

    [Fact]
    public async Task GetChatList_Unauthenticated_Returns401()
    {
        var client = _fixture.Api.CreateClient();

        var response = await client.GetAsync("/Chats");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}