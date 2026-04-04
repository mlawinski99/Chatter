using Core.CQRS;
using Core.KeycloakService;
using Core.ResultPattern;
using FluentValidation;

using static Chatter.Users.Application.Users.Errors.ErrorMessages;

namespace Chatter.Users.Application.Users.Commands;

public class LoginUser : ICommandHandler<LoginUser.LoginUserCommand, Result<KeycloakTokenResponse>>
{
    public record LoginUserCommand(string Username, string Password) : ICommand<Result<KeycloakTokenResponse>>;

    private readonly IKeycloakService _keycloakService;
    private readonly IValidator<LoginUserCommand> _validator;

    public LoginUser(IKeycloakService keycloakService, IValidator<LoginUserCommand> validator)
    {
        _keycloakService = keycloakService;
        _validator = validator;
    }

    public async Task<Result<KeycloakTokenResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Result<KeycloakTokenResponse>.BadRequest(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var tokenResponse = await _keycloakService.LoginUser(command.Username, command.Password);

        if (tokenResponse is null)
            return Result<KeycloakTokenResponse>.Unauthorized(InvalidUsernameOrPassword);

        return Result<KeycloakTokenResponse>.Success(tokenResponse);
    }
}