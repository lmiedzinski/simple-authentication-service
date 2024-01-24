namespace SimpleAuthenticationService.Application.UserAccounts.GetCurrentLoggedInUserAccount;

public sealed record GetCurrentLoggedInUserAccountQueryResponse(
    Guid Id,
    string Login,
    IEnumerable<GetCurrentLoggedInUserAccountQueryResponseClaim> Claims);

public sealed record GetCurrentLoggedInUserAccountQueryResponseClaim(
    string Type,
    string? Value);