using Chatter.Users.Application.Users.Errors;
using FluentValidation;
using static Chatter.Users.Application.Users.Commands.RegisterUser;

namespace Chatter.Users.Application.Users.Validators;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage(ValidationMessages.UsernameRequired);

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationMessages.EmailRequired);
            
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage(ValidationMessages.InvalidEmailFormat);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(ValidationMessages.PasswordRequired);
        
        RuleFor(x => x.Password)
            .MinimumLength(8)
            .WithMessage(ValidationMessages.PasswordMinLength);
        
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage(ValidationMessages.PasswordsDoNotMatch);
    }
}