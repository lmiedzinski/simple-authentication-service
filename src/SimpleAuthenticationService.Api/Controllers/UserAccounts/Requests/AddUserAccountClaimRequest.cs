namespace SimpleAuthenticationService.Api.Controllers.UserAccounts.Requests;

public sealed record AddUserAccountClaimRequest(string Type, string? Value);