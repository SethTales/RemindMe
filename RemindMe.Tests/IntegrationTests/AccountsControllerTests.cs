using NUnit.Framework;
using System.Net.Http;
using System;
using Log4Npg;
using Amazon.CognitoIdentityProvider;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using RemindMe.Models;
using Newtonsoft.Json;
using RemindMe.Adapters;
using RemindMe.Adapters.Helpers;
using NSubstitute;
using System.Net;
using System.Linq;
using System.Collections.Generic;

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
        private IAuthAdapter _mockAuthAdapter;
        private IAwsCognitoAdapterHelper _mockCognitoHelper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _webAppFactory = new TestWebApplicationFactory();
            _httpClient = _webAppFactory.CreateClient();
            _testServiceProvider = _webAppFactory.GetTestServiceProvider();
            _mockLogger = TestStartup.GetTestLogger();
            _mockAuthAdapter = TestStartup.GetMockAuthAdapter();
            _mockCognitoHelper = TestStartup.GetMockAuthHelper();
            var createDbContext = _webAppFactory.GetRemindMeDatabaseContext();
            createDbContext.ExecuteDatabaseMigration();
        }

        [SetUp]
        public void Setup()
        {
            _testDbContext = _testServiceProvider.GetRequiredService<TestDatabaseContext>();
        }

        [TearDown]
        public void TearDown()
        {
            _testDbContext = _testServiceProvider.GetRequiredService<TestDatabaseContext>();
            _testDbContext.DeleteTestData(new string[] {"RemindMeUsers"} );
        }

        [Test]
        public async Task AccountsController_CreateAccountEndpoint_AddsUserToDatabase()
        {
            var testCognitoUser = new AwsCognitoUser
            {
                UserName = "testUser",
                Password = "password"
            };
            _mockAuthAdapter.RegisterNewUserAsync(Arg.Any<AwsCognitoUser>()).Returns(new HttpResponseMessage(HttpStatusCode.Created));
            
            var formContent = new FormUrlEncodedContent(new []
            {
                new KeyValuePair<string, string>("UserName", testCognitoUser.UserName),
                new KeyValuePair<string, string>("Password", testCognitoUser.Password)
            });
            await _httpClient.PostAsync("accounts/create", formContent);

            var testUsers = await _testDbContext.GetTestUsers();
            Assert.AreEqual(testCognitoUser.UserName, testUsers.First().Username);
        }

    }
}