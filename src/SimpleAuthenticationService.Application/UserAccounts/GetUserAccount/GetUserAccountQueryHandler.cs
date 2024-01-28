using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.UserAccounts;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.UserAccounts.GetUserAccount;

public sealed class GetUserAccountQueryHandler
    : IQueryHandler<GetUserAccountQuery, GetUserAccountQueryResponse>
{
    private readonly IUserAccountReadService _userAccountReadService;

    public GetUserAccountQueryHandler(
        IUserAccountReadService userAccountReadService)
    {
        _userAccountReadService = userAccountReadService;
    }

    public async Task<GetUserAccountQueryResponse> Handle(
        GetUserAccountQuery request,
        CancellationToken cancellationToken)
    {
        var userAccount = await _userAccountReadService.GetUserAccountByIdAsync(new UserAccountId(request.UserAccountId));
        if (userAccount is null) throw new NotFoundException(
            nameof(UserAccount),
            request.UserAccountId.ToString());

        return new GetUserAccountQueryResponse(
            userAccount.Id,
            userAccount.Login,
            userAccount.Status,
            userAccount.Claims.Select(y => new GetUserAccountQueryResponseClaim(y.Type, y.Value)));
    }
}