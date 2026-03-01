using Chatter.Shared.ResultPattern;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chatter.Shared.Web;

public class ResultActionFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value is Result result)
        {
            objectResult.StatusCode = (int)result.Code;
        }

        await next();
    }
}