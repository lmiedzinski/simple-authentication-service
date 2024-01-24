namespace SimpleAuthenticationService.Api.Controllers.UserAccounts.Requests;

public sealed record CreateUserAccountRequest(string Login, string Password);