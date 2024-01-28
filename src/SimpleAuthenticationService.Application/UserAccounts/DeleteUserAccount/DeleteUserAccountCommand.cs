using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.DeleteUserAccount;

public sealed record DeleteUserAccountCommand(Guid UserAccountId) : ICommand;