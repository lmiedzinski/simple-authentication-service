using SimpleAuthenticationService.Application.Abstractions.Messaging;

namespace SimpleAuthenticationService.Application.UserAccounts.GetUserAccount;

public sealed record GetUserAccountQuery(Guid UserAccountId) : IQuery<GetUserAccountQueryResponse>;