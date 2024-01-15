using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.GetUserAccountClaims;

public sealed record GetUserAccountClaimsQuery(Guid UserAccountId)
    : IQuery<IEnumerable<GetUserAccountClaimsQueryResponse>>;