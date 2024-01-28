using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.UserAccounts.DeleteUserAccount;

public sealed class DeleteUserAccountCommandHandler : ICommandHandler<DeleteUserAccountCommand>
{
    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserAccountCommandHandler(
        IUserAccountWriteRepository userAccountWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _userAccountWriteRepository = userAccountWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteUserAccountCommand request, CancellationToken cancellationToken)
    {
        var userAccount = await _userAccountWriteRepository.GetByIdAsync(
            new UserAccountId(request.UserAccountId),
            cancellationToken);
        if (userAccount is null)
            throw new NotFoundException(nameof(UserAccount), request.UserAccountId.ToString());
        
        userAccount.Delete();
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}