namespace SimpleAuthenticationService.WebApi.Controllers.UserAccounts.Requests;

public sealed record AddUserAccountClaimRequest(string Type, string? Value);