using Microsoft.Extensions.DependencyInjection;
using SimpleAuthenticationService.Application.Abstractions.Cryptography;
using SimpleAuthenticationService.Application.Abstractions.DateAndTime;
using SimpleAuthenticationService.Application.Abstractions.Token;
using SimpleAuthenticationService.Domain.UserAccounts;
using SimpleAuthenticationService.Infrastructure.Database.EntityFramework;
using Xunit;

namespace SimpleAuthenticationService.IntegrationTests.TestsSetup;

[Collection("SharedTestCollection")]
public abstract class BaseTest : IAsyncLifetime
{
    private readonly ICryptographyService _cryptographyService;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly Func<Task> _resetDatabase;
    
    protected readonly HttpClient HttpClient;
    protected readonly ApplicationDbContext DbContext;
    
    protected BaseTest(SimpleAuthenticationServiceFactory webAppFactory)
    {
        HttpClient = webAppFactory.HttpClient;
        _resetDatabase = webAppFactory.ResetDatabaseAsync;
        
        var serviceScope = webAppFactory.Services.CreateScope();
        DbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _cryptographyService = serviceScope.ServiceProvider.GetRequiredService<ICryptographyService>();
        _tokenService = serviceScope.ServiceProvider.GetRequiredService<ITokenService>();
        _dateTimeProvider = serviceScope.ServiceProvider.GetRequiredService<IDateTimeProvider>();
    }

    protected async Task<UserAccount> CreateTestUserAsync(
        string login = "testuser",
        string password = "Test123456789",
        Dictionary<string, string>? claims = null)
    {
        var testUser = UserAccount.Create(
            new Login(login),
            new PasswordHash(_cryptographyService.HashPassword(password)));

        if (claims is not null)
        {
            foreach (var claim in claims)
            {
                testUser.AddClaim(new Claim(claim.Key, claim.Value));
            }
        }
        
        testUser.SetNewRefreshToken(
            _tokenService.GenerateRefreshToken(),
            _dateTimeProvider.UtcNow.AddMinutes(_tokenService.GetRefreshTokenLifetimeInMinutes()));
        
        testUser.ClearDomainEvents();
        
        await DbContext.UserAccounts.AddAsync(testUser);
        await DbContext.SaveChangesAsync();
        
        return testUser;
    }

    protected string GenerateAccessTokenForUser(Guid userAccountId, Dictionary<string, string>? claims = null)
    {
        return _tokenService.GenerateAccessToken(
            new UserAccountId(userAccountId),
            claims?.Select(x => new Claim(x.Key, x.Value)) ?? []);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}