namespace SimpleAuthenticationService.Application.UserAccounts.LogInUserAccount;

public sealed record LogInUserAccountResponse(string AccessToken, string RefreshToken);