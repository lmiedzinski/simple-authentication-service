using SimpleAuthenticationService.Application.Abstractions.Cryptography;
using SimpleAuthenticationService.Application.Abstractions.DateAndTime;
using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.UserAccounts.LogInUserAccount;

public sealed class LogInUserAccountCommandHandler : ICommandHandler<LogInUserAccountCommand, LogInUserAccountResponse>
{
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICryptographyService _cryptographyService;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LogInUserAccountCommandHandler(
        IUserAccountRepository userAccountRepository,
        IUnitOfWork unitOfWork,
        ICryptographyService cryptographyService,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider)
    {
        _userAccountRepository = userAccountRepository;
        _unitOfWork = unitOfWork;
        _cryptographyService = cryptographyService;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
    }


    public async Task<LogInUserAccountResponse> Handle(LogInUserAccountCommand request, CancellationToken cancellationToken)
    {
        var userAccount = await _userAccountRepository.GetByLoginAsync(new Login(request.Login), cancellationToken);
        if (userAccount is null)
            throw new UserAccountNotFoundOrGivenPasswordIsIncorrectException(request.Login);

        var passwordHash = _cryptographyService.HashPassword(request.Password);
        if (!userAccount.PasswordHash.Value.Equals(passwordHash))
            throw new UserAccountNotFoundOrGivenPasswordIsIncorrectException(request.Login);
        
        userAccount.SetNewRefreshToken(
            _tokenService.GenerateRefreshToken(),
            _dateTimeProvider.UtcNow.AddMinutes(_tokenService.GetRefreshTokenLifetimeInMinutes()));

        var accessToken = _tokenService.GenerateAccessToken(userAccount.Id, userAccount.Claims);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LogInUserAccountResponse(accessToken, userAccount.RefreshToken!.Value);
    }
}