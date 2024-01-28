using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.LockUserAccount;

public sealed record LockUserAccountCommand(Guid UserAccountId) : ICommand;