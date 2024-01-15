using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.AddUserAccountClaim;

public sealed record AddUserAccountClaimCommand(
    Guid UserAccountId,
    string ClaimType,
    string? ClaimValue) : ICommand;