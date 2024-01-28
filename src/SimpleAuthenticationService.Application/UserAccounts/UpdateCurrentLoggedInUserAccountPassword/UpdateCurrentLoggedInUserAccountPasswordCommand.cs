using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.UpdateCurrentLoggedInUserAccountPassword;

public sealed record UpdateCurrentLoggedInUserAccountPasswordCommand(
    string CurrentPassword,
    string NewPassword) : ICommand;