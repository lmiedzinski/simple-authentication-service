using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.UserAccounts;

namespace SimpleAuthenticationService.Application.UserAccounts.GetUserAccounts;

public sealed class GetUserAccountsQueryHandler
    : IQueryHandler<GetUserAccountsQuery, IEnumerable<GetUserAccountsQueryResponse>>
{
    private readonly IUserAccountReadService _userAccountReadService;

    public GetUserAccountsQueryHandler(
        IUserAccountReadService userAccountReadService)
    {
        _userAccountReadService = userAccountReadService;
    }

    public async Task<IEnumerable<GetUserAccountsQueryResponse>> Handle(
        GetUserAccountsQuery request,
        CancellationToken cancellationToken)
    {
        var userAccounts = await _userAccountReadService.GetUserAccountsAsync();

        return userAccounts.Select(x => new GetUserAccountsQueryResponse(
            x.Id,
            x.Login,
            x.Status,
            x.Claims.Select(y => new GetUserAccountsQueryResponseClaim(y.Type, y.Value))));
    }
}