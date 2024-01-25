using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SimpleAuthenticationService.Api.Controllers.UserAccounts.Requests;
using SimpleAuthenticationService.Application.UserAccounts.LogInUserAccount;
using SimpleAuthenticationService.Application.UserAccounts.RefreshUserAccountToken;
using SimpleAuthenticationService.IntegrationTests.TestsSetup;
using Xunit;

namespace SimpleAuthenticationService.IntegrationTests;

public class AuthControllerTests : BaseTest
{
    public AuthControllerTests(SimpleAuthenticationServiceFactory webAppFactory) : base(webAppFactory)
    {
    }

    [Fact]
    public async Task LogIn_Returns_LogInUserAccountResponse_On_Success()
    {
        // Arrange
        const string login = "testlogin";
        const string password = "testPassword123";
        
        await CreateTestUserAsync(login, password);
        
        var request = new LogInRequest(login, password);

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/auth/log-in", request);
        var logInUserAccountResponse = await response.Content.ReadFromJsonAsync<LogInUserAccountResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        logInUserAccountResponse.Should().NotBeNull();
        logInUserAccountResponse!.AccessToken.Should().NotBeNullOrWhiteSpace();
        logInUserAccountResponse.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public async Task RefreshToken_Returns_RefreshUserAccountTokenResponse_On_Success()
    {
        // Arrange
        const string login = "testlogin";
        const string password = "testPassword123";
        
        var testUser = await CreateTestUserAsync(login, password);
        var accessToken = GenerateAccessTokenForUser(testUser.Id.Value);
        
        var request = new RefreshTokenRequest(testUser.RefreshToken!.Value);

        // Act
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await HttpClient.PostAsJsonAsync("api/auth/refresh-token", request);
        var logInUserAccountResponse = await response.Content.ReadFromJsonAsync<RefreshUserAccountTokenResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        logInUserAccountResponse.Should().NotBeNull();
        logInUserAccountResponse!.AccessToken.Should().NotBeNullOrWhiteSpace();
    }
    
    [Fact]
    public async Task LogOut_Returns_EmptyOk_On_Success()
    {
        // Arrange
        const string login = "testlogin";
        const string password = "testPassword123";
        
        var testUser = await CreateTestUserAsync(login, password);
        var accessToken = GenerateAccessTokenForUser(testUser.Id.Value);
        
        // Act
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await HttpClient.PostAsync("api/auth/log-out", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}