using Xunit;

namespace SimpleAuthenticationService.IntegrationTests.TestsSetup;

[CollectionDefinition("SharedTestCollection")]
public class SharedTestCollection : ICollectionFixture<SimpleAuthenticationServiceFactory>
{
}