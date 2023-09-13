namespace SimpleAuthenticationService.Domain.UserAccounts;

public interface IUserAccountRepository
{
    void Add(UserAccount userAccount);
    
    Task<UserAccount?> GetByIdAsync(UserAccountId id, CancellationToken cancellationToken = default);
    Task<UserAccount?> GetByLoginAsync(Login login, CancellationToken cancellationToken = default);
    Task<UserAccount?> GetByActiveRefreshTokenValueAsync(string refreshTokenValue, CancellationToken cancellationToken = default);
}