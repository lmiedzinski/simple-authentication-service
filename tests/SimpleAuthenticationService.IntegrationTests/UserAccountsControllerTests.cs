using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using SimpleAuthenticationService.Api.Controllers.UserAccounts.Requests;
using SimpleAuthenticationService.Application.UserAccounts.GetCurrentLoggedInUserAccount;
using SimpleAuthenticationService.Application.UserAccounts.GetUserAccountClaims;
using SimpleAuthenticationService.Infrastructure.Authorization;
using SimpleAuthenticationService.IntegrationTests.TestsSetup;
using Xunit;

namespace SimpleAuthenticationService.IntegrationTests;

public class UserAccountsControllerTests : BaseTest
{
    public UserAccountsControllerTests(SimpleAuthenticationServiceFactory webAppFactory) : base(webAppFactory)
    {
    }

    [Fact]
    public async Task GetCurrentLoggedInUserAccount_Returns_GetCurrentLoggedInUserAccountQueryResponse_On_Success()
    {
        // Arrange
        const string login = "testlogin";
        const string password = "testPassword123";
        var claims = new Dictionary<string, string>
        {
            { "testClaim1", "testClaim1Value" },
            { "testClaim2", "testClaim2Value" }
        };

        var testUser = await CreateTestUserAsync(login, password, claims);
        var accessToken = GenerateAccessTokenForUser(testUser.Id.Value, claims);

        // Act
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await HttpClient.GetAsync("api/users/me");
        var getCurrentLoggedInUserAccountQueryResponse =
            await response.Content.ReadFromJsonAsync<GetCurrentLoggedInUserAccountQueryResponse>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        getCurrentLoggedInUserAccountQueryResponse.Should().NotBeNull();
        getCurrentLoggedInUserAccountQueryResponse!.Id.Should().Be(testUser.Id.Value);
        getCurrentLoggedInUserAccountQueryResponse.Login.Should().Be(testUser.Login.Value);
        getCurrentLoggedInUserAccountQueryResponse.Claims.Should().HaveCount(claims.Count);
        foreach (var claim in getCurrentLoggedInUserAccountQueryResponse.Claims)
        {
            claim.Value.Should().Be(claims[claim.Type]);
        }
    }
    
    [Fact]
    public async Task CreateUserAccount_Returns_NoContent_On_Success()
    {
        // Arrange
        const string login = "testlogin";
        const string password = "testPassword123";

        var request = new CreateUserAccountRequest(login, password);
        
        var claims = new Dictionary<string, string>
        {
            { AuthorizationPolicies.UserAccountAdministrator, string.Empty }
        };
        var adminAccessToken = GenerateAccessTokenForUser(Guid.NewGuid(), claims);

        // Act
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminAccessToken);
        var response = await HttpClient.PostAsJsonAsync("api/users/", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task GetUserAccountClaims_Returns_GetUserAccountClaimsQueryResponses_On_Success()
    {
        // Arrange
        const string login = "testlogin";
        const string password = "testPassword123";
        var claims = new Dictionary<string, string>
        {
            { "testClaim1", "testClaim1Value" },
            { "testClaim2", "testClaim2Value" }
        };

        var testUser = await CreateTestUserAsync(login, password, claims);
        
        var adminClaims = new Dictionary<string, string>
        {
            { AuthorizationPolicies.UserAccountAdministrator, string.Empty }
        };
        var adminAccessToken = GenerateAccessTokenForUser(Guid.NewGuid(), adminClaims);

        // Act
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminAccessToken);
        var response = await HttpClient.GetAsync($"api/users/{testUser.Id.Value}/claims");
        var getUserAccountClaimsQueryResponses =
            (await response.Content.ReadFromJsonAsync<IEnumerable<GetUserAccountClaimsQueryResponse>>())?.ToList();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        getUserAccountClaimsQueryResponses.Should().NotBeNull();
        foreach (var claim in getUserAccountClaimsQueryResponses!)
        {
            claim.Value.Should().Be(claims[claim.Type]);
        }
    }
    
    [Fact]
    public async Task AddUserAccountClaim_Returns_NoContent_On_Success()
    {
        // Arrange
        const string login = "testlogin";
        const string password = "testPassword123";

        var testUser = await CreateTestUserAsync(login, password);
        
        var claims = new Dictionary<string, string>
        {
            { AuthorizationPolicies.UserAccountAdministrator, string.Empty }
        };
        var adminAccessToken = GenerateAccessTokenForUser(Guid.NewGuid(), claims);
        
        var request = new AddUserAccountClaimRequest("testClaim", "testClaimValue");

        // Act
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminAccessToken);
        var response = await HttpClient.PostAsJsonAsync($"api/users/{testUser.Id.Value}/claims", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task DeleteUserAccountClaim_Returns_NoContent_On_Success()
    {
        // Arrange
        const string login = "testlogin";
        const string password = "testPassword123";

        var claims = new Dictionary<string, string>
        {
            { "testClaim1", "testClaim1Value" },
            { "testClaim2", "testClaim2Value" }
        };
        var testUser = await CreateTestUserAsync(login, password, claims);
        
        var adminClaims = new Dictionary<string, string>
        {
            { AuthorizationPolicies.UserAccountAdministrator, string.Empty }
        };
        var adminAccessToken = GenerateAccessTokenForUser(Guid.NewGuid(), adminClaims);
        
        var claimToRemove = testUser.Claims.First();
        var requestBody = new DeleteUserAccountClaimRequest(claimToRemove.Type, claimToRemove.Value);

        // Act
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminAccessToken);
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/users/{testUser.Id.Value}/claims")
        {
            Content = JsonContent.Create(requestBody)
        };
        var response = await HttpClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}