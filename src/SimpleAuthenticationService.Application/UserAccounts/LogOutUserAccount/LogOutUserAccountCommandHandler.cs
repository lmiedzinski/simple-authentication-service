using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.UserAccounts.LogOutUserAccount;

public class LogOutUserAccountCommandHandler : ICommandHandler<LogOutUserAccountCommand>
{
    private readonly ITokenService _tokenService;
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogOutUserAccountCommandHandler(
        ITokenService tokenService,
        IUserAccountRepository userAccountRepository,
        IUnitOfWork unitOfWork)
    {
        _tokenService = tokenService;
        _userAccountRepository = userAccountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(LogOutUserAccountCommand request, CancellationToken cancellationToken)
    {
        var userAccountId = _tokenService.GetUserAccountIdFromContext();

        var userAccount = await _userAccountRepository.GetByIdAsync(userAccountId, cancellationToken);
        if (userAccount is null)
            throw new NotFoundException(nameof(UserAccount), userAccountId.Value.ToString());
        
        userAccount.RevokeRefreshToken();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}