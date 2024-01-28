using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Application.Abstractions.UserAccounts;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.UserAccounts.RemoveUserAccountClaim;

public sealed class RemoveUserAccountClaimCommandHandler : ICommandHandler<RemoveUserAccountClaimCommand>
{
    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IUserAccountReadService _userAccountReadService;

    public RemoveUserAccountClaimCommandHandler(
        IUserAccountWriteRepository userAccountWriteRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IUserAccountReadService userAccountReadService)
    {
        _userAccountWriteRepository = userAccountWriteRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _userAccountReadService = userAccountReadService;
    }

    public async Task Handle(
        RemoveUserAccountClaimCommand request,
        CancellationToken cancellationToken)
    {
        var userAccount = await _userAccountWriteRepository.GetByIdAsync(
            new UserAccountId(request.UserAccountId),
            cancellationToken);
        if (userAccount is null)
            throw new NotFoundException(nameof(UserAccount), request.UserAccountId.ToString());
        
        var claimToRemove = new Claim(request.ClaimType, request.ClaimValue);

        if (claimToRemove == _tokenService.GetInternalAdministratorClaim()
            && await _userAccountReadService.GetActiveInternalAdministratorsCountAsync() < 2)
            throw new LastInternalAdministratorRemovalNotAllowedException();
        
        userAccount.RemoveClaim(claimToRemove);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}