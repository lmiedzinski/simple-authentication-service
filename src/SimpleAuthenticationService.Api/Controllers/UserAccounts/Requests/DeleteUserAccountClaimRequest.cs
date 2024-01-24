namespace SimpleAuthenticationService.Api.Controllers.UserAccounts.Requests;

public sealed record DeleteUserAccountClaimRequest(string Type, string? Value);