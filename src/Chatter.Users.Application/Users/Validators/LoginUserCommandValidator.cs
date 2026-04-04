using Chatter.Users.Application.Users.Errors;
using FluentValidation;
using static Chatter.Users.Application.Users.Commands.LoginUser;

namespace Chatter.Users.Application.Users.Validators;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage(ValidationMessages.UsernameRequired);
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(ValidationMessages.PasswordRequired);
    }
}