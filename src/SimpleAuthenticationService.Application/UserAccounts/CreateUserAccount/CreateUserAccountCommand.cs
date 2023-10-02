using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.CreateUserAccount;

public sealed record CreateUserAccountCommand(string Login, string Password) : ICommand;