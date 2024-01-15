using Microsoft.EntityFrameworkCore;
using SimpleAuthenticationService.Application.Abstractions.DateAndTime;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Infrastructure.EntityFramework.Repositories;

internal sealed class UserAccountWriteRepository : IUserAccountWriteRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UserAccountWriteRepository(
        ApplicationDbContext dbContext,
        IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public void Add(
        UserAccount userAccount)
    {
        _dbContext.UserAccounts.Add(userAccount);
    }

    public async Task<UserAccount?> GetByIdAsync(
        UserAccountId id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserAccounts
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public async Task<UserAccount?> GetByLoginAsync(
        Login login,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserAccounts
            .FirstOrDefaultAsync(x => x.Login == login, cancellationToken: cancellationToken);
    }

    public async Task<UserAccount?> GetByActiveRefreshTokenValueAsync(
        string refreshTokenValue,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserAccounts
            .FirstOrDefaultAsync(
                x => x.RefreshToken != null
                     && x.RefreshToken.IsActive
                     && x.RefreshToken.ExpirationDateUtc > _dateTimeProvider.UtcNow
                     && x.RefreshToken.Value == refreshTokenValue,
                cancellationToken: cancellationToken);
    }
}