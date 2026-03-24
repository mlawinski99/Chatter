using Chatter.Messages.Application.Chat.Queries;
using Chatter.Shared.CQRS;
using Chatter.Shared.Pager;
using Chatter.Shared.ResultPattern;
using Chatter.Shared.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chatter.MessagesService.Controllers;

[Route("[controller]")]
[Authorize]
public class ChatsController(IRequestDispatcher requestDispatcher) : BaseController(requestDispatcher)
{
    [HttpGet]
    public async Task<Result<PagedResult<GetChatList.ChatDto>>> GetChatList(int page = 1, int pageSize = 20)
    {
        var request = new GetChatList.GetChatListQuery(page, pageSize);

        return await _requestDispatcher.Dispatch(request);
    }
}