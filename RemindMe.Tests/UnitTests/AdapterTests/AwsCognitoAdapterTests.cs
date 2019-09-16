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
using Newtonsoft.Json;

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
        public async Task SuccessfulCreationOfAccount_ReturnsStatusCode_Created()
        {
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                Password = "fakePassword"
            };
            _cognitoAdapterHelper.UserExists(Arg.Any<AwsCognitoUser>()).Returns(false);
            _awsCognitoClient.SignUpAsync(Arg.Any<SignUpRequest>()).Returns(new SignUpResponse());

            var registerUserResponse = await _authAdapter.RegisterNewUserAsync(user);

            Assert.AreEqual(registerUserResponse.StatusCode, HttpStatusCode.Created);
        }

        [Test]
        public async Task IfUserExists_RegisterNewUser_ReturnsStatusCode_Conflict()
        {
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                Password = "fakePassword"
            };
            _cognitoAdapterHelper.UserExists(Arg.Any<AwsCognitoUser>()).Returns(true);

            var registerUserResponse = await _authAdapter.RegisterNewUserAsync(user);

            Assert.AreEqual(registerUserResponse.StatusCode, HttpStatusCode.Conflict);
        }

        [Test]
        public async Task SuccessfulConfirmation_OfAccount_ReturnsStatusCode_Ok()
        {
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                ConfirmationCode = "123456"
            };
            _cognitoAdapterHelper.UserExists(Arg.Any<AwsCognitoUser>()).Returns(true);
            _cognitoAdapterHelper.UserIsConfirmed(Arg.Any<AwsCognitoUser>()).Returns(false);
            _awsCognitoClient.ConfirmSignUpAsync(Arg.Any<ConfirmSignUpRequest>()).Returns(new ConfirmSignUpResponse());

            var confirmUserResponse = await _authAdapter.ConfirmUserAsync(user);

            Assert.AreEqual(confirmUserResponse.StatusCode, HttpStatusCode.OK);
        }

        [Test]
        public async Task IsUerDoesNotExist_ConfirmAccount_ReturnsStatusCode_NotFound()
        {
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                ConfirmationCode = "123456"
            };
            _cognitoAdapterHelper.UserExists(Arg.Any<AwsCognitoUser>()).Returns(false);

            var confirmUserResponse = await _authAdapter.ConfirmUserAsync(user);

            Assert.AreEqual(confirmUserResponse.StatusCode, HttpStatusCode.NotFound);
        }

        [Test]
        public async Task IsUerIsAlreadyConfirmed_ConfirmAccount_ReturnsStatusCode_Conflict()
        {
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                ConfirmationCode = "123456"
            };
            _cognitoAdapterHelper.UserExists(Arg.Any<AwsCognitoUser>()).Returns(true);
            _cognitoAdapterHelper.UserIsConfirmed(Arg.Any<AwsCognitoUser>()).Returns(true);

            var confirmUserResponse = await _authAdapter.ConfirmUserAsync(user);

            Assert.AreEqual(confirmUserResponse.StatusCode, HttpStatusCode.Conflict);
        }

        [Test]
        public async Task SuccessfulAuthenticationRequest_ReturnsAuthenticationResult_AndStatusCodeOk()
        {
            var fakeAccessToken = "fakeAccessToken";
            var fakeIdToken = "fakeIdToken";
            var fakeRefreshToken = "fakeRefreshToken";

            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                Password = "fakePassword"
            };
            _cognitoAdapterHelper.UserExists(Arg.Any<AwsCognitoUser>()).Returns(true);
            _cognitoAdapterHelper.UserIsConfirmed(Arg.Any<AwsCognitoUser>()).Returns(true);
            _awsCognitoClient.InitiateAuthAsync(Arg.Any<InitiateAuthRequest>()).Returns(new InitiateAuthResponse
            {
                AuthenticationResult = new AuthenticationResultType
                {
                    AccessToken = fakeAccessToken,
                    IdToken = fakeIdToken,
                    RefreshToken = fakeRefreshToken
                }
            });

            var authResponse = await _authAdapter.AuthenticateUserAsync(user);
            var content = await authResponse.Content.ReadAsStringAsync();
            var authenticationResult = JsonConvert.DeserializeObject<AuthenticationResultType>(content);

            Assert.AreEqual(HttpStatusCode.OK, authResponse.StatusCode);
            Assert.AreEqual(fakeAccessToken, authenticationResult.AccessToken);
            Assert.AreEqual(fakeIdToken, authenticationResult.IdToken);
            Assert.AreEqual(fakeRefreshToken, authenticationResult.RefreshToken);
        }

    }
}