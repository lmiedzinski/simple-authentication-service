using SimpleAuthenticationService.Application.Abstractions.Cryptography;
using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.UserAccounts.UpdateUserAccountPassword;

public sealed class UpdateUserAccountPasswordCommandHandler : ICommandHandler<UpdateUserAccountPasswordCommand>
{
    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICryptographyService _cryptographyService;
    private readonly ITokenService _tokenService;

    public UpdateUserAccountPasswordCommandHandler(
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
        UpdateUserAccountPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var userAccount = await _userAccountWriteRepository.GetByIdAsync(
            new UserAccountId(request.UserAccountId),
            cancellationToken);
        if (userAccount is null) throw new NotFoundException(
            nameof(UserAccount),
            request.UserAccountId.ToString());
        
        var actingUserId = _tokenService.GetUserAccountIdFromContext();
        if (userAccount.Id == actingUserId) throw new SelfOperationNotAllowedException();
        
        var passwordHash = _cryptographyService.HashPassword(request.NewPassword);
        userAccount.UpdatePasswordHash(new PasswordHash(passwordHash));
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}