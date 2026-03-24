using System.Net;
using System.Net.Http.Json;
using Chatter.IntegrationTests.Messages.Infrastructure;
using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.Messages.Application.Message.Errors;
using Chatter.Shared.DomainTypes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Chatter.IntegrationTests.Messages;

[Collection("MessagesApi")]
public class EditMessageTests
{
    private readonly MessagesTestFixture _fixture;

    public EditMessageTests(MessagesTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task EditMessage_OwnMessage_Returns200AndUpdatesInDb()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var newContent = $"Edited content {Guid.NewGuid()}";
        var response = await client.PutAsJsonAsync("/Messages", new
        {
            ChatId = MessagesDbSeeder.PrivateChat1Id,
            MessageId = MessagesDbSeeder.Message1Id,
            Content = newContent
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var db = _fixture.CreateDbContext();
        var message = await db.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == MessagesDbSeeder.Message1Id);

        message.Should().NotBeNull();
        message!.Content.Text.Should().Be(newContent);
        message.Status.Should().Be(MessageStatus.Edited);
    }

    [Fact]
    public async Task EditMessage_OthersMessage_Returns403()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.PutAsJsonAsync("/Messages", new
        {
            ChatId = MessagesDbSeeder.PrivateChat1Id,
            MessageId = MessagesDbSeeder.Message2Id,
            Content = "Trying to edit someone else's message"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(ErrorMessages.CanOnlyEditOwnMessages);
    }

    [Fact]
    public async Task EditMessage_NonExistentMessage_Returns404()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.PutAsJsonAsync("/Messages", new
        {
            ChatId = MessagesDbSeeder.PrivateChat1Id,
            MessageId = Guid.NewGuid(),
            Content = "Editing non-existent message"
        });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(ErrorMessages.MessageNotFound);
    }

    [Fact]
    public async Task EditMessage_WrongChatId_Returns400()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.PutAsJsonAsync("/Messages", new
        {
            ChatId = MessagesDbSeeder.GroupChatId,
            MessageId = MessagesDbSeeder.Message1Id,
            Content = "Editing with wrong chat ID"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain(ErrorMessages.MessageDoesNotBelongToChat);
    }

    [Fact]
    public async Task EditMessage_EmptyContent_ReturnsBadRequest()
    {
        var client = await _fixture.Api.CreateAuthenticatedClientAsync(
            KeycloakTestUsersData.TestUsername, KeycloakTestUsersData.TestPassword);

        var response = await client.PutAsJsonAsync("/Messages", new
        {
            ChatId = MessagesDbSeeder.PrivateChat1Id,
            MessageId = MessagesDbSeeder.Message1Id,
            Content = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task EditMessage_Unauthenticated_Returns401()
    {
        var client = _fixture.Api.CreateClient();

        var response = await client.PutAsJsonAsync("/Messages", new
        {
            ChatId = MessagesDbSeeder.PrivateChat1Id,
            MessageId = MessagesDbSeeder.Message1Id,
            Content = "Unauthorized edit"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}