using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Application.Abstractions.Token;

public interface ITokenService
{
    string GenerateAccessToken(UserAccountId userAccountId, IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    int GetRefreshTokenLifetimeInMinutes();
    UserAccountId GetUserAccountIdFromContext();
    Claim GetInternalAdministratorClaim();
}