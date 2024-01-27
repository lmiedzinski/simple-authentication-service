using SimpleAuthenticationService.Application.Abstractions.UserAccounts.UserAccountStatus;

namespace SimpleAuthenticationService.Application.UserAccounts.GetUserAccounts;

public sealed record GetUserAccountsQueryResponse(
    Guid Id,
    string Login,
    UserAccountStatusDto Status,
    IEnumerable<GetUserAccountsQueryResponseClaim> Claims);

public sealed record GetUserAccountsQueryResponseClaim(
    string Type,
    string? Value);