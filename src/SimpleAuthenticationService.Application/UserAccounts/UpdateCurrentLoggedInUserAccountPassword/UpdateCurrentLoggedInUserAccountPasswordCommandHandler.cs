using SimpleAuthenticationService.Application.Abstractions.Cryptography;
using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.UserAccounts.UpdateCurrentLoggedInUserAccountPassword;

public sealed class UpdateCurrentLoggedInUserAccountPasswordCommandHandler : ICommandHandler<UpdateCurrentLoggedInUserAccountPasswordCommand>
{
    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICryptographyService _cryptographyService;
    private readonly ITokenService _tokenService;

    public UpdateCurrentLoggedInUserAccountPasswordCommandHandler(
        IUserAccountWriteRepository userAccountWriteRepository,
        IUnitOfWork unitOfWork,
        ICryptographyService cryptographyService,
        ITokenService tokenService)
    {
        _userAccountWriteRepository = userAccountWriteRepository;
        _unitOfWork = unitOfWork;
        _cryptographyService = cryptographyService;
        _tokenService = tokenService;
    }

    public async Task Handle(
        UpdateCurrentLoggedInUserAccountPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var userAccountId = _tokenService.GetUserAccountIdFromContext();

        var userAccount = await _userAccountWriteRepository.GetByIdAsync(userAccountId, cancellationToken);
        if (userAccount is null)
            throw new NotFoundException(nameof(UserAccount), userAccountId.Value.ToString());
        
        if (!_cryptographyService.IsPasswordMatchingHash(request.CurrentPassword, userAccount.PasswordHash.Value))
            throw new IncorrectPasswordException(userAccountId.Value.ToString());
        
        var passwordHash = _cryptographyService.HashPassword(request.NewPassword);
        userAccount.UpdatePasswordHash(new PasswordHash(passwordHash));
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}