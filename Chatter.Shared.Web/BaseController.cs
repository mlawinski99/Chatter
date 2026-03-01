using Chatter.Shared.CQRS;
using Microsoft.AspNetCore.Mvc;

namespace Chatter.Shared.Web;

[ApiController]
[TypeFilter(typeof(ResultActionFilter))]
public abstract class BaseController(IRequestDispatcher requestDispatcher) : ControllerBase
{
    protected readonly IRequestDispatcher _requestDispatcher = requestDispatcher;
}