using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.UserAccounts.AddUserAccountClaim;

public sealed class AddUserAccountClaimCommandHandler : ICommandHandler<AddUserAccountClaimCommand>
{
    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddUserAccountClaimCommandHandler(
        IUserAccountWriteRepository userAccountWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _userAccountWriteRepository = userAccountWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        AddUserAccountClaimCommand request,
        CancellationToken cancellationToken)
    {
        var userAccount = await _userAccountWriteRepository.GetByIdAsync(
            new UserAccountId(request.UserAccountId),
            cancellationToken);
        if (userAccount is null)
            throw new NotFoundException(nameof(UserAccount), request.UserAccountId.ToString());
        
        userAccount.AddClaim(new Claim(request.ClaimType, request.ClaimValue));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}