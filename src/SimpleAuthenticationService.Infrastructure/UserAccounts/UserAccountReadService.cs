using Dapper;
using SimpleAuthenticationService.Application.Abstractions.UserAccounts;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Infrastructure.SqlConnection;

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
}