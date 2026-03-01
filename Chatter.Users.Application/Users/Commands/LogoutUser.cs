using Chatter.Shared.CQRS;
using Chatter.Shared.DataAccessTypes;
using Chatter.Shared.KeycloakService;
using Chatter.Shared.Logger;
using Chatter.Shared.ResultPattern;
using FluentValidation;

using static Chatter.Users.Application.Users.Errors.ErrorMessages;

namespace Chatter.Users.Application.Users.Commands;

public class LogoutUser : ICommandHandler<LogoutUser.LogoutUserCommand, Result>
{
    public record LogoutUserCommand(string RefreshToken) : ICommand<Result>;

    private readonly IKeycloakService _keycloakService;
    private readonly IValidator<LogoutUserCommand> _validator;
    private readonly IAppLogger<LogoutUser> _logger;
    private readonly IUserProvider _userProvider;

    public LogoutUser(IKeycloakService keycloakService, IValidator<LogoutUserCommand> validator, IAppLogger<LogoutUser> logger, IUserProvider userProvider)
    {
        _keycloakService = keycloakService;
        _validator = validator;
        _logger = logger;
        _userProvider = userProvider;
    }

    public async Task<Result> Handle(LogoutUserCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Result.BadRequest(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        try
        {
            await _keycloakService.LogoutUser(command.RefreshToken);
            return Result.Success;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError("Failed to logout user {UserId}: {Error}", _userProvider.UserId, ex.Message);
            return Result.BadRequest(FailedToLogout);
        }
    }
}