using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace SimpleAuthenticationService.Infrastructure.Token;

internal sealed class RsaKeysProvider
{
    public RsaKeysProvider(IOptions<TokenOptions> tokenOptions)
    {
        RsaPrivateSecurityKey = PreparePrivateKey(tokenOptions.Value.AccessTokenOptions.PrivateRsaCertificate);
        RsaPublicSecurityKey = PreparePublicKey(tokenOptions.Value.AccessTokenOptions.PublicRsaCertificate);
    }

    public RsaSecurityKey RsaPrivateSecurityKey { get; private set; }
    public RsaSecurityKey RsaPublicSecurityKey { get; private set; }

    private static RsaSecurityKey PreparePrivateKey(string base64Key)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(
            Convert.FromBase64String(base64Key),
            out _);
        return new RsaSecurityKey(rsa);
    }
    
    private static RsaSecurityKey PreparePublicKey(string base64Key)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(
            Convert.FromBase64String(base64Key),
            out _);
        return new RsaSecurityKey(rsa);
    }
}