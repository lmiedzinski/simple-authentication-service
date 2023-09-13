using SimpleAuthenticationService.Application.Abstractions.DateAndTime;
using SimpleAuthenticationService.Application.Abstractions.Messaging;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Domain.Abstractions;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.UserAccounts.RefreshUserAccountToken;

public sealed class RefreshUserAccountTokenCommandHandler
    : ICommandHandler<RefreshUserAccountTokenCommand, RefreshUserAccountTokenResponse>
{
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RefreshUserAccountTokenCommandHandler(
        IUserAccountRepository userAccountRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider)
    {
        _userAccountRepository = userAccountRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RefreshUserAccountTokenResponse> Handle(
        RefreshUserAccountTokenCommand request,
        CancellationToken cancellationToken)
    {
        var userAccount = await _userAccountRepository.GetByActiveRefreshTokenValueAsync(
            request.RefreshToken,
            cancellationToken);
        if (userAccount is null) throw new NotFoundException(nameof(RefreshToken));
        
        userAccount.SetNewRefreshToken(
            userAccount.RefreshToken!.Value,
            _dateTimeProvider.UtcNow.AddMinutes(_tokenService.GetRefreshTokenLifetimeInMinutes()));

        var accessToken = _tokenService.GenerateAccessToken(userAccount.Id, userAccount.Claims);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RefreshUserAccountTokenResponse(accessToken);
    }
}