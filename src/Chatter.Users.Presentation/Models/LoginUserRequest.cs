using static Chatter.Users.Application.Users.Commands.LoginUser;

namespace Chatter.UsersService.Models;

public class LoginUserRequest
{
    public string Username { get; set; }
    public string Password { get; set; }

    public LoginUserCommand ToCommand() =>
        new(Username, Password);
}