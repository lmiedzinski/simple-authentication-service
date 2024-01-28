using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAuthenticationService.Api.Controllers.UserAccounts.Requests;
using SimpleAuthenticationService.Application.UserAccounts.AddUserAccountClaim;
using SimpleAuthenticationService.Application.UserAccounts.CreateUserAccount;
using SimpleAuthenticationService.Application.UserAccounts.DeleteUserAccount;
using SimpleAuthenticationService.Application.UserAccounts.GetCurrentLoggedInUserAccount;
using SimpleAuthenticationService.Application.UserAccounts.GetUserAccountClaims;
using SimpleAuthenticationService.Application.UserAccounts.GetUserAccounts;
using SimpleAuthenticationService.Application.UserAccounts.LockUserAccount;
using SimpleAuthenticationService.Application.UserAccounts.RemoveUserAccountClaim;
using SimpleAuthenticationService.Application.UserAccounts.UnlockUserAccount;
using SimpleAuthenticationService.Application.UserAccounts.UpdateCurrentLoggedInUserAccountPassword;
using SimpleAuthenticationService.Infrastructure.Authorization;

namespace SimpleAuthenticationService.Api.Controllers.UserAccounts;

[ApiController]
[Route("api/users")]
public class UserAccountsController : ControllerBase
{
    private readonly ISender _sender;

    public UserAccountsController(ISender sender)
    {
        _sender = sender;
    }
    
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentLoggedInUserAccount(
        CancellationToken cancellationToken)
    {
        var query = new GetCurrentLoggedInUserAccountQuery();

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }
    
    [Authorize]
    [HttpPut("me/password")]
    public async Task<IActionResult> UpdateCurrentLoggedInUserAccountPassword(
        UpdateCurrentLoggedInUserAccountPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCurrentLoggedInUserAccountPasswordCommand(
            request.CurrentPassword,
            request.NewPassword);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }
    
    [Authorize(Policy = AuthorizationPolicies.UserAccountAdministrator)]
    [HttpGet]
    public async Task<IActionResult> GetUserAccounts(
        CancellationToken cancellationToken)
    {
        var query = new GetUserAccountsQuery();

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }
    
    [Authorize(Policy = AuthorizationPolicies.UserAccountAdministrator)]
    [HttpPost]
    public async Task<IActionResult> CreateUserAccount(
        CreateUserAccountRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserAccountCommand(request.Login, request.Password);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }
    
    [Authorize(Policy = AuthorizationPolicies.UserAccountAdministrator)]
    [HttpGet("{id:guid}/claims")]
    public async Task<IActionResult> GetUserAccountClaims(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetUserAccountClaimsQuery(id);

        var response = await _sender.Send(query, cancellationToken);

        return Ok(response);
    }
    
    [Authorize(Policy = AuthorizationPolicies.UserAccountAdministrator)]
    [HttpPost("{id:guid}/claims")]
    public async Task<IActionResult> AddUserAccountClaim(
        Guid id,
        AddUserAccountClaimRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddUserAccountClaimCommand(id, request.Type, request.Value);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }
    
    [Authorize(Policy = AuthorizationPolicies.UserAccountAdministrator)]
    [HttpDelete("{id:guid}/claims")]
    public async Task<IActionResult> DeleteUserAccountClaim(
        Guid id,
        DeleteUserAccountClaimRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RemoveUserAccountClaimCommand(id, request.Type, request.Value);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }
    
    [Authorize(Policy = AuthorizationPolicies.UserAccountAdministrator)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUserAccount(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserAccountCommand(id);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }
    
    [Authorize(Policy = AuthorizationPolicies.UserAccountAdministrator)]
    [HttpPost("{id:guid}/lock")]
    public async Task<IActionResult> LockUserAccount(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new LockUserAccountCommand(id);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }
    
    [Authorize(Policy = AuthorizationPolicies.UserAccountAdministrator)]
    [HttpPost("{id:guid}/unlock")]
    public async Task<IActionResult> UnlockUserAccount(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new UnlockUserAccountCommand(id);

        await _sender.Send(command, cancellationToken);

        return NoContent();
    }
}