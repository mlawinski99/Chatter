using System.Net;
using Chatter.IntegrationTests.Messages.Infrastructure;
using Chatter.IntegrationTests.Shared.Infrastructure;
using Chatter.Messages.Application.Chat.Queries;
using Chatter.Shared.Pager;
using FluentAssertions;
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
        var client = _fixture.Api.CreateAuthenticatedClient();

        var response = await client.GetAsync("/Chats");

        var result = await response.ReadResult<PagedResult<GetChatList.ChatDto>>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalCount.Should().BeGreaterThan(0);
        result.Data.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetChatList_WithPagination_Returns200WithCorrectPage()
    {
        var client = _fixture.Api.CreateAuthenticatedClient();

        var response = await client.GetAsync("/Chats?page=1&pageSize=2");

        var result = await response.ReadResult<PagedResult<GetChatList.ChatDto>>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Page.Should().Be(1);
        result.Data.PageSize.Should().Be(2);
    }

    [Fact]
    public async Task GetChatList_IncludesLastMessage()
    {
        var client = _fixture.Api.CreateAuthenticatedClient();

        var response = await client.GetAsync("/Chats");

        var result = await response.ReadResult<PagedResult<GetChatList.ChatDto>>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().Contain(c => c.LastMessage != null);
    }

    [Fact]
    public async Task GetChatList_Unauthenticated_Returns401()
    {
        var client = _fixture.Api.CreateClient();

        var response = await client.GetAsync("/Chats");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}