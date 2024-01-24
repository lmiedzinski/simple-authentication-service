namespace SimpleAuthenticationService.Application.UserAccounts.GetUserAccountClaims;

public sealed record GetUserAccountClaimsQueryResponse(
    string Type,
    string? Value);