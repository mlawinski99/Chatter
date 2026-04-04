using static Chatter.Users.Application.Users.Commands.LogoutUser;

namespace Chatter.UsersService.Models;

public class LogoutUserRequest
{
    public string RefreshToken { get; set; }

    public LogoutUserCommand ToCommand() =>
        new(RefreshToken);
}