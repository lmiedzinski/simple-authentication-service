using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.RefreshUserAccountToken;

public record RefreshUserAccountTokenCommand(string RefreshToken)
    : ICommand<RefreshUserAccountTokenResponse>;