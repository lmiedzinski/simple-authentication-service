using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.UserAccounts;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.UserAccounts.GetUserAccountClaims;

public sealed class GetUserAccountClaimsQueryHandler
    : IQueryHandler<GetUserAccountClaimsQuery, IEnumerable<GetUserAccountClaimsQueryResponse>>
{
    private readonly IUserAccountReadService _userAccountReadService;

    public GetUserAccountClaimsQueryHandler(IUserAccountReadService userAccountReadService)
    {
        _userAccountReadService = userAccountReadService;
    }

    public async Task<IEnumerable<GetUserAccountClaimsQueryResponse>> Handle(GetUserAccountClaimsQuery request, CancellationToken cancellationToken)
    {
        var userAccount = await _userAccountReadService.GetUserAccountById(new UserAccountId(request.UserAccountId));
        if (userAccount is null) throw new NotFoundException(nameof(UserAccount), request.UserAccountId.ToString());

        return userAccount.Claims
            .Select(x => new GetUserAccountClaimsQueryResponse(x.Type, x.Value));
    }
}