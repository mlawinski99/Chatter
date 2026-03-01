using static Chatter.Users.Application.Users.Commands.RegisterUser;

namespace Chatter.UsersService.Models;

public class RegisterUserRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Email { get; set; }

    public RegisterUserCommand ToCommand() =>
        new(Username, Password, ConfirmPassword, Email);
}