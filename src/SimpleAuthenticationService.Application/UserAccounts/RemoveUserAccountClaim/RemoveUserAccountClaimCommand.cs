using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.RemoveUserAccountClaim;

public sealed record RemoveUserAccountClaimCommand(
    Guid UserAccountId,
    string ClaimType,
    string? ClaimValue) : ICommand;