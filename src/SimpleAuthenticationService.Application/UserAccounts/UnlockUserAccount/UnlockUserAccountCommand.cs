using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.UnlockUserAccount;

public sealed record UnlockUserAccountCommand(Guid UserAccountId) : ICommand;