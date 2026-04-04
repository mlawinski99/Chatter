using System.Net;
using System.Net.Http.Json;
using Chatter.IntegrationTests.Messages.Infrastructure;
using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.Messages.Application.Message.Errors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Chatter.IntegrationTests.Messages;

[Collection("MessagesApi")]
public class DeleteMessageTests
{
    private readonly MessagesTestFixture _fixture;

    public DeleteMessageTests(MessagesTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DeleteMessage_OwnMessage_Returns200AndSoftDeletes()
    {
        var client = _fixture.Api.CreateAuthenticatedClient();

        // Create message to prevent failure of other tests
        var content = "Message to delete";
        await client.PostAsJsonAsync("/Messages", new
        {
            ChatId = MessagesDbSeeder.PrivateChat1Id,
            Content = content
        });

        using var db = _fixture.CreateDbContext();
        var created = await db.Messages
            .AsNoTracking()
            .FirstAsync(m => m.Content.Text == content);

        var request = new HttpRequestMessage(HttpMethod.Delete, "/Messages")
        {
            Content = JsonContent.Create(new
            {
                ChatId = MessagesDbSeeder.PrivateChat1Id,
                MessageId = created.Id
            })
        };

        var response = await client.SendAsync(request);

        var result = await response.ReadResult();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.IsSuccess.Should().BeTrue();

        var deleted = await db.Messages
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == created.Id);

        deleted.Should().NotBeNull();
        deleted!.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteMessage_OthersMessage_Returns403()
    {
        var client = _fixture.Api.CreateAuthenticatedClient();

        var request = new HttpRequestMessage(HttpMethod.Delete, "/Messages")
        {
            Content = JsonContent.Create(new
            {
                ChatId = MessagesDbSeeder.GroupChatId,
                MessageId = MessagesDbSeeder.Message4Id
            })
        };

        var response = await client.SendAsync(request);

        var result = await response.ReadResult();
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain(ErrorMessages.CanOnlyDeleteOwnMessages);
    }

    [Fact]
    public async Task DeleteMessage_NonExistentMessage_Returns404()
    {
        var client = _fixture.Api.CreateAuthenticatedClient();

        var request = new HttpRequestMessage(HttpMethod.Delete, "/Messages")
        {
            Content = JsonContent.Create(new
            {
                ChatId = MessagesDbSeeder.PrivateChat1Id,
                MessageId = Guid.NewGuid()
            })
        };

        var response = await client.SendAsync(request);

        var result = await response.ReadResult();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain(ErrorMessages.MessageNotFound);
    }

    [Fact]
    public async Task DeleteMessage_WrongChatId_Returns400()
    {
        var client = _fixture.Api.CreateAuthenticatedClient();

        var request = new HttpRequestMessage(HttpMethod.Delete, "/Messages")
        {
            Content = JsonContent.Create(new
            {
                ChatId = MessagesDbSeeder.GroupChatId,
                MessageId = MessagesDbSeeder.Message1Id
            })
        };

        var response = await client.SendAsync(request);

        var result = await response.ReadResult();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain(ErrorMessages.MessageDoesNotBelongToChat);
    }

    [Fact]
    public async Task DeleteMessage_Unauthenticated_Returns401()
    {
        var client = _fixture.Api.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Delete, "/Messages")
        {
            Content = JsonContent.Create(new
            {
                ChatId = MessagesDbSeeder.PrivateChat1Id,
                MessageId = MessagesDbSeeder.Message1Id
            })
        };

        var response = await client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}