using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.GetUserAccounts;

public sealed record GetUserAccountsQuery()
    : IQuery<IEnumerable<GetUserAccountsQueryResponse>>;