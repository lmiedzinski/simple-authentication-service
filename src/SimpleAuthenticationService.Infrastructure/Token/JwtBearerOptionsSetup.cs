using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SimpleAuthenticationService.Infrastructure.Token;

internal sealed class JwtBearerOptionsSetup : IConfigureOptions<JwtBearerOptions>
{
    private readonly TokenOptions _tokenOptions;
    private readonly RsaKeysProvider _rsaKeysProvider;

    public JwtBearerOptionsSetup(
        IOptions<TokenOptions> tokenServiceOptions,
        RsaKeysProvider rsaKeysProvider)
    {
        _tokenOptions = tokenServiceOptions.Value;
        _rsaKeysProvider = rsaKeysProvider;
    }

    public void Configure(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            RequireSignedTokens = true,
            RequireExpirationTime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _tokenOptions.AccessTokenOptions.Issuer,
            ValidAudience = _tokenOptions.AccessTokenOptions.Audience,
            IssuerSigningKey = _rsaKeysProvider.RsaPublicSecurityKey,
            ClockSkew = TimeSpan.FromSeconds(5)
        };
    }
}