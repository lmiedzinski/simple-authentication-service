namespace SimpleAuthenticationService.WebApi.Controllers.UserAccounts.Requests;

public sealed record CreateUserAccountRequest(string Login, string Password);