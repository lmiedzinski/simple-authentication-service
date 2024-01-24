using Dapper;
using SimpleAuthenticationService.Application.Abstractions.UserAccounts;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Infrastructure.Database.SqlConnection;

namespace SimpleAuthenticationService.Infrastructure.UserAccounts;

internal sealed class UserAccountReadService : IUserAccountReadService
{
    private readonly SqlConnectionFactory _sqlConnectionFactory;

    public UserAccountReadService(SqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<bool> ExistsByLoginAsync(Login login)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();
        
        const string sql = """
                            SELECT CAST(
                                CASE WHEN 
                                    EXISTS (
                                    SELECT 1
                                    FROM user_accounts
                                    WHERE login = @Login
                                    LIMIT 1)
                                THEN 'true'
                                ELSE 'false'
                                END
                                AS BOOLEAN)
                           """;

        return await connection.ExecuteScalarAsync<bool>(sql, new { Login = login.Value});
    }

    public async Task<UserAccountReadModelDto?> GetUserAccountById(UserAccountId id)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var userAccounts = new Dictionary<Guid, UserAccountReadModelDto>();
        
        const string sql = """
                           SELECT
                             ua.id AS Id,
                             ua.login AS Login,
                             ua.status AS Status,
                             uac.type AS Type,
                             uac.value AS Value
                           FROM user_accounts ua
                           LEFT JOIN user_account_claims uac ON ua.id = uac.user_account_id
                           WHERE ua.id = @UserAccountId
                           """;

        var x = await connection.QueryAsync<UserAccountReadModelDto, ClaimReadModelDto, UserAccountReadModelDto>(
            sql,
            (userAccount, claim) =>
            {
                if (userAccounts.TryGetValue(userAccount.Id, out var existingUserAccount))
                {
                    userAccount = existingUserAccount;
                }
                else
                {
                    userAccounts.Add(userAccount.Id, userAccount);
                }
                
                userAccount.Claims.Add(claim);

                return userAccount;
            },
            new
            {
                UserAccountId = id.Value
            },
            splitOn: "Type");
        
            return userAccounts[id.Value];
    }
}