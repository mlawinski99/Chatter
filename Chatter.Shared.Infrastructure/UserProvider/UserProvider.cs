using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Chatter.Shared.DataAccessTypes;

public class UserProvider(IHttpContextAccessor httpContextAccessor) : IUserProvider
{
    public string UserId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null)
                return null;

            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}