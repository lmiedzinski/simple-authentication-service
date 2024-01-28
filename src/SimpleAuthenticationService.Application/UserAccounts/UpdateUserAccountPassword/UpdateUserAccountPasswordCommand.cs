using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.UpdateUserAccountPassword;

public sealed record UpdateUserAccountPasswordCommand(
    Guid UserAccountId,
    string NewPassword) : ICommand;