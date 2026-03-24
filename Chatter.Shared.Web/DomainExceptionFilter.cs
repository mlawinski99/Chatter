using Chatter.Shared.DomainTypes;
using Chatter.Shared.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chatter.Shared.Web;

public class DomainExceptionFilter(IAppLogger<DomainExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not DomainException domainException)
            return;

        logger.LogError("Domain rule violation: {Message}", domainException.Message);

        context.Result = new BadRequestObjectResult("Invalid request.");
        context.ExceptionHandled = true;
    }
}
