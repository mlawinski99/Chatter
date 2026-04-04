using Core.CQRS;
using Core.KeycloakService;
using Core.ResultPattern;
using Core.Web;
using Chatter.Users.Application.Users.Queries;
using Chatter.UsersService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chatter.UsersService.Controllers;

[Route("api/[controller]")]
public class UsersController(IRequestDispatcher requestDispatcher) : BaseController(requestDispatcher)
{

    [HttpGet("search")]
    [Authorize]
    public async Task<Result<FindUser.FindUserDto?>> FindUser(string search)
    {
        var query = new FindUser.FindUserQuery(search);
        return await _requestDispatcher.Dispatch(query);
    }

    [HttpPost("register")]
    public async Task<Result> RegisterUser(RegisterUserRequest model)
    {
        return await _requestDispatcher.Dispatch(model.ToCommand());
    }

    [HttpPost("login")]
    public async Task<Result<KeycloakTokenResponse>> LoginUser(LoginUserRequest model)
    {
        return await _requestDispatcher.Dispatch(model.ToCommand());
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<Result> LogoutUser(LogoutUserRequest model)
    {
        return await _requestDispatcher.Dispatch(model.ToCommand());
    }
}