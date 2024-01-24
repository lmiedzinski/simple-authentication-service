using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.GetCurrentLoggedInUserAccount;

public sealed record GetCurrentLoggedInUserAccountQuery()
    : IQuery<GetCurrentLoggedInUserAccountQueryResponse>;