using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure;

public class UserProvider(IHttpContextAccessor httpContextAccessor) : IUserProvider
{
    public Guid? UserId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null)
                return null;

            return Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}