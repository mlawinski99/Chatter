using System.Net;
using Chatter.IntegrationTests.Messages.Infrastructure;
using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.Messages.Application.Message.Errors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Chatter.IntegrationTests.Messages;

[Collection("MessagesApi")]
public class LoadMessagesTests
{
    private readonly MessagesTestFixture _fixture;

    public LoadMessagesTests(MessagesTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task LoadMessages_ValidChatId_Returns200WithMessages()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.GetAsync(
            $"/Messages?chatId={MessagesDbSeeder.PrivateChat1Id}&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var db = _fixture.CreateDbContext();
        var dbMessages = await db.Messages
            .AsNoTracking()
            .Where(m => m.ChatId == MessagesDbSeeder.PrivateChat1Id)
            .ToListAsync();

        var body = await response.Content.ReadAsStringAsync();
        foreach (var msg in dbMessages)
        {
            body.Should().Contain(msg.Content.Text);
        }
    }

    [Fact]
    public async Task LoadMessages_WithCursor_ReturnsOlderMessages()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        using var db = _fixture.CreateDbContext();
        var cursorMessage = await db.Messages
            .AsNoTracking()
            .FirstAsync(m => m.Id == MessagesDbSeeder.Message2Id);

        var response = await client.GetAsync(
            $"/Messages?chatId={MessagesDbSeeder.PrivateChat1Id}&lastMessageId={MessagesDbSeeder.Message2Id}&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().NotContain(cursorMessage.Content.Text);
    }

    [Fact]
    public async Task LoadMessages_NonExistentChat_Returns404()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.GetAsync(
            $"/Messages?chatId={Guid.NewGuid()}&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(ErrorMessages.ChatNotFound);
    }

    [Fact]
    public async Task LoadMessages_EmptyChat_Returns200WithEmptyList()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        using var db = _fixture.CreateDbContext();
        var count = await db.Messages
            .AsNoTracking()
            .CountAsync(m => m.ChatId == MessagesDbSeeder.PrivateChat2Id);
        count.Should().Be(0);

        var response = await client.GetAsync(
            $"/Messages?chatId={MessagesDbSeeder.PrivateChat2Id}&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"items\":[]");
    }

    [Fact]
    public async Task LoadMessages_Unauthenticated_Returns401()
    {
        var client = _fixture.Api.CreateClient();

        var response = await client.GetAsync(
            $"/Messages?chatId={MessagesDbSeeder.PrivateChat1Id}&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}