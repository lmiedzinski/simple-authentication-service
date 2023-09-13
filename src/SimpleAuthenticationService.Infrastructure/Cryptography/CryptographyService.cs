using SimpleAuthenticationService.Application.Abstractions.Cryptography;

namespace SimpleAuthenticationService.Infrastructure.Cryptography;

internal sealed class CryptographyService : ICryptographyService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}