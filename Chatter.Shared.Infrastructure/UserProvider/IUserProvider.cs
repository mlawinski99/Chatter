using System.Security.Claims;

namespace Chatter.Shared.DataAccessTypes;

public interface IUserProvider
{
    Guid? UserId { get; }
}