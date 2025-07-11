using System.Security.Claims;

namespace Chatter.Shared.DataAccessTypes;

public interface IUserProvider
{
    string UserId { get; }
}