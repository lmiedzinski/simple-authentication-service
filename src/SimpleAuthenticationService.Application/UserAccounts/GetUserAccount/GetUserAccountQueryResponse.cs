using SimpleAuthenticationService.Application.Abstractions.UserAccounts.UserAccountStatus;

namespace SimpleAuthenticationService.Application.UserAccounts.GetUserAccount;

public sealed record GetUserAccountQueryResponse(
    Guid Id,
    string Login,
    UserAccountStatusDto Status,
    IEnumerable<GetUserAccountQueryResponseClaim> Claims);

public sealed record GetUserAccountQueryResponseClaim(
    string Type,
    string? Value);