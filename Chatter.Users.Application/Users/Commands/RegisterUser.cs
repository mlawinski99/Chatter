using Chatter.Shared.CQRS;
using Chatter.Shared.KeycloakService;
using Chatter.Shared.Logger;
using Chatter.Shared.ResultPattern;
using FluentValidation;

using static Chatter.Users.Application.Users.Errors.ErrorMessages;

namespace Chatter.Users.Application.Users.Commands;

public class RegisterUser : ICommandHandler<RegisterUser.RegisterUserCommand, Result>
{
        public record RegisterUserCommand(string Username, string Password, string ConfirmPassword, string Email) : ICommand<Result>;

        private readonly IKeycloakService _keycloakService;
        private readonly IValidator<RegisterUserCommand> _validator;
        private readonly IAppLogger<RegisterUser> _logger;

        public RegisterUser(IKeycloakService keycloakService, IValidator<RegisterUserCommand> validator, IAppLogger<RegisterUser> logger)
        {
                _keycloakService = keycloakService;
                _validator = validator;
                _logger = logger;
        }

        public async Task<Result> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
                var validationResult = await _validator.ValidateAsync(command, cancellationToken);
                if (!validationResult.IsValid)
                        return Result.BadRequest(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                try
                {
                        var token = await _keycloakService.GetToken();
                        await _keycloakService.CreateUser(token, command.Username, command.Email);
                        return Result.Success;
                }
                catch (HttpRequestException ex)
                {
                        _logger.LogError("Failed to register user {Username} with email {Email}: {Error}", command.Username, command.Email, ex.Message);
                        return Result.BadRequest(FailedToRegisterUser);
                }
        }
}