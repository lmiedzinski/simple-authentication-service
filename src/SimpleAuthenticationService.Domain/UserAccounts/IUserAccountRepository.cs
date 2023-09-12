namespace SimpleAuthenticationService.Domain.UserAccounts;

public interface IUserAccountRepository
{
    void Add(UserAccount userAccount);
    
    Task<UserAccount?> GetById(UserAccountId id, CancellationToken cancellationToken = default);
}