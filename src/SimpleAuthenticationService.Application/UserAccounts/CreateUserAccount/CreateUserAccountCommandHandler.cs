using SimpleAuthenticationService.Application.Abstractions.Cryptography;
using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.UserAccounts;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.UserAccounts.CreateUserAccount;

public sealed class CreateUserAccountCommandHandler : ICommandHandler<CreateUserAccountCommand>
{
    private readonly IUserAccountReadService _userAccountReadService;
    private readonly ICryptographyService _cryptographyService;
    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserAccountCommandHandler(
        IUserAccountReadService userAccountReadService,
        ICryptographyService cryptographyService,
        IUserAccountWriteRepository userAccountWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _userAccountReadService = userAccountReadService;
        _cryptographyService = cryptographyService;
        _userAccountWriteRepository = userAccountWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        CreateUserAccountCommand request,
        CancellationToken cancellationToken)
    {
        var isLoginTaken = await _userAccountReadService.ExistsByLoginAsync(new Login(request.Login));
        if (isLoginTaken) throw new LoginAlreadyTakenException(request.Login);

        var passwordHash = _cryptographyService.HashPassword(request.Password);
        
        var userAccount = UserAccount.Create(new Login(request.Login), new PasswordHash(passwordHash));
        _userAccountWriteRepository.Add(userAccount);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}