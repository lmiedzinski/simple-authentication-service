using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAuthenticationService.Application.UserAccounts.CreateUserAccount;
using SimpleAuthenticationService.Application.UserAccounts.LogInUserAccount;
using SimpleAuthenticationService.Application.UserAccounts.LogOutUserAccount;
using SimpleAuthenticationService.Application.UserAccounts.RefreshUserAccountToken;
using SimpleAuthenticationService.WebApi.Controllers.UserAccounts.Requests;

namespace SimpleAuthenticationService.WebApi.Controllers.UserAccounts;

[ApiController]
[Route("api/users")]
public class UserAccountsController : ControllerBase
{
    private readonly ISender _sender;

    public UserAccountsController(ISender sender)
    {
        _sender = sender;
    }
    
    [AllowAnonymous]
    [HttpPost("log-in")]
    public async Task<IActionResult> LogIn(
        LogInRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LogInUserAccountCommand(request.Login, request.Password);

        var result = await _sender.Send(command, cancellationToken);

        return Ok(result);
    }
    
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefreshUserAccountTokenCommand(request.RefreshToken);

        var result = await _sender.Send(command, cancellationToken);

        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("log-out")]
    public async Task<IActionResult> LogOut(
        CancellationToken cancellationToken)
    {
        var command = new LogOutUserAccountCommand();

        await _sender.Send(command, cancellationToken);

        return Ok();
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateUserAccount(
        CreateUserAccountRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserAccountCommand(request.Login, request.Password);

        await _sender.Send(command, cancellationToken);

        return Ok();
    }
}