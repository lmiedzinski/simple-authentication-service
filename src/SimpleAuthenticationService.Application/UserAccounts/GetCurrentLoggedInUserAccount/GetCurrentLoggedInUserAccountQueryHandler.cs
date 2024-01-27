using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Application.Abstractions.UserAccounts;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.UserAccounts.GetCurrentLoggedInUserAccount;

public sealed class GetCurrentLoggedInUserAccountQueryHandler
    : IQueryHandler<GetCurrentLoggedInUserAccountQuery, GetCurrentLoggedInUserAccountQueryResponse>
{
    private readonly IUserAccountReadService _userAccountReadService;
    private readonly ITokenService _tokenService;

    public GetCurrentLoggedInUserAccountQueryHandler(
        IUserAccountReadService userAccountReadService,
        ITokenService tokenService)
    {
        _userAccountReadService = userAccountReadService;
        _tokenService = tokenService;
    }

    public async Task<GetCurrentLoggedInUserAccountQueryResponse> Handle(
        GetCurrentLoggedInUserAccountQuery request,
        CancellationToken cancellationToken)
    {
        var userAccountId = _tokenService.GetUserAccountIdFromContext();
        var userAccount = await _userAccountReadService.GetUserAccountByIdAsync(userAccountId);
        if (userAccount is null) throw new NotFoundException(nameof(UserAccount), userAccountId.Value.ToString());

        return new GetCurrentLoggedInUserAccountQueryResponse(
            userAccount.Id,
            userAccount.Login,
            userAccount.Claims
                .Select(x => new GetCurrentLoggedInUserAccountQueryResponseClaim(x.Type, x.Value)));
    }
}