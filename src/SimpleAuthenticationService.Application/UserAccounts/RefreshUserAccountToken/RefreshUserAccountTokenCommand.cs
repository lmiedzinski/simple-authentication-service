using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.RefreshUserAccountToken;

public sealed record RefreshUserAccountTokenCommand(string RefreshToken)
    : ICommand<RefreshUserAccountTokenResponse>;