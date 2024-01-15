namespace SimpleAuthenticationService.WebApi.Controllers.UserAccounts.Requests;

public sealed record DeleteUserAccountClaimRequest(string Type, string? Value);