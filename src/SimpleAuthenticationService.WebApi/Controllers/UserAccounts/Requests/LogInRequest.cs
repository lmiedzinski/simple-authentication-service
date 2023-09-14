namespace SimpleAuthenticationService.WebApi.Controllers.UserAccounts.Requests;

public sealed record LogInRequest(string Login, string Password);