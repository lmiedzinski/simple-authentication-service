using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.UserAccounts.RemoveUserAccountClaim;

public sealed class RemoveUserAccountClaimCommandHandler : ICommandHandler<RemoveUserAccountClaimCommand>
{
    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveUserAccountClaimCommandHandler(
        IUserAccountWriteRepository userAccountWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _userAccountWriteRepository = userAccountWriteRepository;
        _unitOfWork = unitOfWork;
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
        
        userAccount.RemoveClaim(new Claim(request.ClaimType, request.ClaimValue));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}