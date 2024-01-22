namespace SimpleAuthenticationService.Api.Controllers.UserAccounts.Requests;

public sealed record LogInRequest(string Login, string Password);