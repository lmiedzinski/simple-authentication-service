using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleAuthenticationService.Application.Abstractions.DateAndTime;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Domain.UserAccounts;
using Claim = SimpleAuthenticationService.Domain.UserAccounts.Claim;

namespace SimpleAuthenticationService.Infrastructure.Token;

internal sealed class TokenService : ITokenService
{
    private readonly TokenServiceOptions _tokenServiceOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly RsaSecurityKey _rsaSecurityKey;

    public TokenService(
        IOptions<TokenServiceOptions> tokenServiceOptions,
        IHttpContextAccessor httpContextAccessor,
        IDateTimeProvider dateTimeProvider,
        RsaSecurityKey rsaSecurityKey)
    {
        _httpContextAccessor = httpContextAccessor;
        _dateTimeProvider = dateTimeProvider;
        _rsaSecurityKey = rsaSecurityKey;
        _tokenServiceOptions = tokenServiceOptions.Value;
    }

    public string GenerateAccessToken(UserAccountId userAccountId, IEnumerable<Claim> claims)
    {
        var signingCredentials = new SigningCredentials(
            key: _rsaSecurityKey,
            algorithm: SecurityAlgorithms.RsaSha256
        );
        
        var claimsIdentity = new ClaimsIdentity();

        claimsIdentity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, userAccountId.Value.ToString()));

        foreach (var claim in claims)
        {
            claimsIdentity.AddClaim(new System.Security.Claims.Claim(claim.Type, claim.Value ?? string.Empty));
        }
        
        var jwtHandler = new JwtSecurityTokenHandler();

        var jwt = jwtHandler.CreateJwtSecurityToken(
            issuer: _tokenServiceOptions.AccessTokenOptions.Issuer,
            audience: _tokenServiceOptions.AccessTokenOptions.Audience,
            subject: claimsIdentity,
            notBefore: _dateTimeProvider.UtcNow,
            expires: _dateTimeProvider.UtcNow.AddSeconds(_tokenServiceOptions.AccessTokenOptions.LifeTimeInSeconds),
            issuedAt: _dateTimeProvider.UtcNow,
            signingCredentials: signingCredentials);

        return jwtHandler.WriteToken(jwt);
    }

    public string GenerateRefreshToken()
    {
        var length = _tokenServiceOptions.RefreshTokenOptions.Length;
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(length));
    }

    public int GetRefreshTokenLifetimeInMinutes()
    {
        return _tokenServiceOptions.RefreshTokenOptions.LifeTimeInMinutes;
    }

    public UserAccountId GetUserAccountIdFromContext()
    {
        var userAccountIdValue = _httpContextAccessor
                .HttpContext?
                .User
                .Claims
                .SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?
                .Value ??
            throw new ApplicationException("UserAccount context is not available");

        if(!Guid.TryParse(userAccountIdValue, out var userAccountIdGuid))
            throw new ApplicationException("UserAccount context is in the wrong format");

        return new UserAccountId(userAccountIdGuid);
    }
}