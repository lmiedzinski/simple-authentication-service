namespace SimpleAuthenticationService.Application.UserAccounts.GetUserAccountClaims;

public sealed record GetUserAccountClaimsQueryResponse(
    string ClaimType,
    string? ClaimValue);