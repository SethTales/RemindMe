using NUnit.Framework;
using System.Net.Http;
using System;
using Log4Npg;
using Amazon.CognitoIdentityProvider;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace RemindMe.Tests.IntegrationTests
{
    [TestFixture]
    public class AccountsControllerTests
    {
        private TestWebApplicationFactory _webAppFactory;
        private HttpClient _httpClient;
        private TestDatabaseContext _testDbContext;
        private IServiceProvider _testServiceProvider;
        private INpgLogger _mockLogger;
        private IAmazonCognitoIdentityProvider _mockCognitoClient;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _webAppFactory = new TestWebApplicationFactory();
            _httpClient = _webAppFactory.CreateClient();
            _testServiceProvider = _webAppFactory.GetTestServiceProvider();
            _mockLogger = TestStartup.GetTestLogger();
            _mockCognitoClient = TestStartup.GetMockCognitoClient();
            var createDbContext = _webAppFactory.GetRemindMeDatabaseContext();
            createDbContext.ExecuteDatabaseMigration();
        }

        [TearDown]
        public void TearDown()
        {
            _testDbContext = _testServiceProvider.GetRequiredService<TestDatabaseContext>();
            _testDbContext.DeleteTestData(new string[] {"RemindMeUsers"} );
        }

        [Test]
        public async Task IntegrationTestsWork()
        {
            var response = await _httpClient.GetAsync("/accounts/create");
        }

    }
}