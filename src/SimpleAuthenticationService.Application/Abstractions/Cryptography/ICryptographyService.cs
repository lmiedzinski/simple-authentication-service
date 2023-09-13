namespace SimpleAuthenticationService.Application.Abstractions.Cryptography;

public interface ICryptographyService
{
    string HashPassword(string password);
}