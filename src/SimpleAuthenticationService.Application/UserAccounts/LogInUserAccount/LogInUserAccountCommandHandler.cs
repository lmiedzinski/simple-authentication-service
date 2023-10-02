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
    private readonly IUserAccountWriteRepository _userAccountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICryptographyService _cryptographyService;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LogInUserAccountCommandHandler(
        IUserAccountWriteRepository userAccountWriteRepository,
        IUnitOfWork unitOfWork,
        ICryptographyService cryptographyService,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider)
    {
        _userAccountWriteRepository = userAccountWriteRepository;
        _unitOfWork = unitOfWork;
        _cryptographyService = cryptographyService;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
    }


    public async Task<LogInUserAccountResponse> Handle(LogInUserAccountCommand request, CancellationToken cancellationToken)
    {
        var userAccount = await _userAccountWriteRepository.GetByLoginAsync(new Login(request.Login), cancellationToken);
        if (userAccount is null)
            throw new UserAccountNotFoundOrGivenPasswordIsIncorrectException(request.Login);

        if (!_cryptographyService.IsPasswordMatchingHash(request.Password, userAccount.PasswordHash.Value))
            throw new UserAccountNotFoundOrGivenPasswordIsIncorrectException(request.Login);
        
        userAccount.SetNewRefreshToken(
            _tokenService.GenerateRefreshToken(),
            _dateTimeProvider.UtcNow.AddMinutes(_tokenService.GetRefreshTokenLifetimeInMinutes()));

        var accessToken = _tokenService.GenerateAccessToken(userAccount.Id, userAccount.Claims);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LogInUserAccountResponse(accessToken, userAccount.RefreshToken!.Value);
    }
}