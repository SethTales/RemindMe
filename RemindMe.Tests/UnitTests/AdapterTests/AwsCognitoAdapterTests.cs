using NUnit.Framework;
using NSubstitute;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using RemindMe.Adapters.Helpers;
using RemindMe.Adapters;
using System.Threading.Tasks;
using RemindMe.Models;
using System.Collections.Generic;
using System.Net;

namespace RemindMe.Tests.UnitTests.Adapters
{
    [TestFixture]
    public class AwsCognitoAdapterTests
    {
        private IAmazonCognitoIdentityProvider _awsCognitoClient;
        private AwsCognitoAdapterConfig _cognitoAdapterConfig;
        private IAwsCognitoAdapterHelper _cognitoAdapterHelper;
        private IAuthAdapter _authAdapter;

        [SetUp]
        public void Setup()
        {
            _cognitoAdapterConfig = new AwsCognitoAdapterConfig
            {
                UserPoolId = "fake-userpool-id",
                ClientId = "fake-client-id"
            };
            _awsCognitoClient = Substitute.For<IAmazonCognitoIdentityProvider>();
            _cognitoAdapterHelper = Substitute.For<IAwsCognitoAdapterHelper>();
            _authAdapter = new AwsCognitoAdapter(_awsCognitoClient, _cognitoAdapterConfig, _cognitoAdapterHelper);
        }

        [Test]
        public async Task SuccessfulCreationOfAccount_ReturnsStatusCodeCreated()
        {
            var user = new RemindMeUser
            {
                UserName = "fakeUsername",
                Password = "fakePassword"
            };
            _cognitoAdapterHelper.UserExists(Arg.Any<RemindMeUser>()).Returns(false);
            _awsCognitoClient.SignUpAsync(Arg.Any<SignUpRequest>()).Returns(new SignUpResponse());

            var registerUserResponse = await _authAdapter.RegisterNewUserAsync(user);

            Assert.AreEqual(registerUserResponse.StatusCode, HttpStatusCode.Created);
        }

        [Test]
        public async Task IfUserExists_RegisterNewUser_ReturnsStatusCodeConflict()
        {
            var user = new RemindMeUser
            {
                UserName = "fakeUsername",
                Password = "fakePassword"
            };
            _cognitoAdapterHelper.UserExists(Arg.Any<RemindMeUser>()).Returns(true);

            var registerUserResponse = await _authAdapter.RegisterNewUserAsync(user);

            Assert.AreEqual(registerUserResponse.StatusCode, HttpStatusCode.Conflict);
        }
    }
}