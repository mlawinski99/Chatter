using Chatter.Users.Application.Users.Errors;
using FluentValidation;
using static Chatter.Users.Application.Users.Commands.LogoutUser;

namespace Chatter.Users.Application.Users.Validators;

public class LogoutUserCommandValidator : AbstractValidator<LogoutUserCommand>
{
    public LogoutUserCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage(ValidationMessages.RefreshTokenRequired);
    }
}