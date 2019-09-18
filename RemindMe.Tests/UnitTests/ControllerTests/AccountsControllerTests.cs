using Log4Npg;
using RemindMe.Api.Controllers;
using RemindMe.Adapters;
using NUnit.Framework;
using NSubstitute;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RemindMe.Data;
using System.Threading.Tasks;
using RemindMe.Models;
using System.Net.Http;
using System.Net;
using System;
using NSubstitute.ExceptionExtensions;
using Amazon.CognitoIdentityProvider.Model;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace RemindMe.Tests.UnitTests.Controllers
{
    [TestFixture]
    public class AccountsControllerTests
    {
        private INpgLogger _logger;
        private IAuthAdapter _authAdapter;
        private IApplicationRepository _appRepository;
        private AccountsController _accountsController;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<INpgLogger>();
            _authAdapter = Substitute.For<IAuthAdapter>();
            _appRepository = Substitute.For<IApplicationRepository>();
            _accountsController = new AccountsController(_logger, _authAdapter, _appRepository);
        }

        //CreateAccount tests
        [Test]
        public void GetCreateAccountView_ReturnsExpectedView()
        {
            var getCreateAccountViewResult = _accountsController.GetCreateAccountView(string.Empty) as ViewResult;
            Assert.AreEqual("createAccount", getCreateAccountViewResult.ViewName);
        }

        [Test]
        public async Task SuccessfulPostToCreateAccount_CallsAppRepositoryMethod_AndRedirectsToCorrectAction()
        {
            _authAdapter.RegisterNewUserAsync(Arg.Any<AwsCognitoUser>()).Returns(new HttpResponseMessage(HttpStatusCode.Created));
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                Password = "fakePassword"
            };

            var createAccountResponse = await _accountsController.CreateAccount(user) as RedirectToActionResult;

            await _appRepository.Received(1).AddUserAsync(Arg.Any<string>());
            Assert.AreEqual("GetConfirmAccountView", createAccountResponse.ActionName);
        }

        [Test]
        public async Task AttemptToCreateAccount_ButUserExists_RedirectsToCorrectAction()
        {
            _authAdapter.RegisterNewUserAsync(Arg.Any<AwsCognitoUser>()).Returns(new HttpResponseMessage(HttpStatusCode.Conflict));
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                Password = "fakePassword"
            };

            var createAccountResponse = await _accountsController.CreateAccount(user) as RedirectToActionResult;

            await _appRepository.Received(0).AddUserAsync(Arg.Any<string>());
            Assert.AreEqual("GetCreateAccountView", createAccountResponse.ActionName);
        }

        [Test]
        public async Task IfExceptionIsThrown_ByAuthAdapter_ExceptionIsLogged_AndUserIsRedirectedBackToCreateAccountView()
        {
            _authAdapter.RegisterNewUserAsync(Arg.Any<AwsCognitoUser>()).Throws(new Exception());
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                Password = "fakePassword"
            };

            var createAccountResponse = await _accountsController.CreateAccount(user) as RedirectToActionResult;

            _logger.Received(1).LogError(Arg.Any<object>());
            await _appRepository.Received(0).AddUserAsync(Arg.Any<string>());
            Assert.AreEqual("GetCreateAccountView", createAccountResponse.ActionName);
        }

        //ConfirmAccount tests
        [Test]
        public void GetConfirmAccountView_ReturnsExpectedView()
        {
            var getConfirmAccountViewResult = _accountsController.GetConfirmAccountView(string.Empty) as ViewResult;
            Assert.AreEqual("confirmAccount", getConfirmAccountViewResult.ViewName);
        }

        [Test]
        public async Task SuccessfulPostToConfirmAccount_RedirectsTo_GetLoginView()
        {
            _authAdapter.ConfirmUserAsync(Arg.Any<AwsCognitoUser>()).Returns(new HttpResponseMessage(HttpStatusCode.OK));
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                ConfirmationCode = "123456"
            };

            var confirmAccountResponse = await _accountsController.ConfirmAccount(user) as RedirectToActionResult;

            Assert.AreEqual("GetLoginView", confirmAccountResponse.ActionName);
        }

        [Test]
        public async Task AttemptToConfirmAccount_ButUserDoesNotExist_RedirectsTo_GetCreateAccountView()
        {
            _authAdapter.ConfirmUserAsync(Arg.Any<AwsCognitoUser>()).Returns(new HttpResponseMessage(HttpStatusCode.NotFound));
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                ConfirmationCode = "123456"
            };

            var confirmAccountResponse = await _accountsController.ConfirmAccount(user) as RedirectToActionResult;

            Assert.AreEqual("GetCreateAccountView", confirmAccountResponse.ActionName);
        }

        [Test]
        public async Task AttemptToConfirmAccount_ButUserIsAlreadyConfirmed_RedirectsTo_GetLoginView()
        {
            _authAdapter.ConfirmUserAsync(Arg.Any<AwsCognitoUser>()).Returns(new HttpResponseMessage(HttpStatusCode.Conflict));
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                ConfirmationCode = "123456"
            };

            var confirmAccountResponse = await _accountsController.ConfirmAccount(user) as RedirectToActionResult;

            Assert.AreEqual("GetLoginView", confirmAccountResponse.ActionName);
        }

        [Test]
        public async Task IfCodeMismatchException_IsThrownBy_ConfirmNewUserAsync_ExceptionIsLogged_AndUserRedirectedTo_GetConfirmAccountView()
        {
            _authAdapter.ConfirmUserAsync(Arg.Any<AwsCognitoUser>()).Throws(new CodeMismatchException("code mismatch"));
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                ConfirmationCode = "123456"
            };

            var confirmAccountResponse = await _accountsController.ConfirmAccount(user) as RedirectToActionResult;

            _logger.Received(1).LogError(Arg.Any<object>());
            Assert.AreEqual("GetConfirmAccountView", confirmAccountResponse.ActionName);
        }

        [Test]
        public async Task IfException_IsThrownBy_ConfirmNewUserAsync_ExceptionIsLogged_AndUserRedirectedTo_GetConfirmAccountView()
        {
            _authAdapter.ConfirmUserAsync(Arg.Any<AwsCognitoUser>()).Throws(new Exception());
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                ConfirmationCode = "123456"
            };

            var confirmAccountResponse = await _accountsController.ConfirmAccount(user) as RedirectToActionResult;

            _logger.Received(1).LogError(Arg.Any<object>());
            Assert.AreEqual("GetConfirmAccountView", confirmAccountResponse.ActionName);
        }

        [Test]
        public async Task IfExpiredCodeException_IsThrownBy_ConfirmNewUserAsync_ExceptionIsLogged_AndUserRedirectedTo_ResendConfirmationCodeView()
        {
            _authAdapter.ConfirmUserAsync(Arg.Any<AwsCognitoUser>()).Throws(new ExpiredCodeException("expired code"));
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                ConfirmationCode = "123456"
            };

            var confirmAccountResponse = await _accountsController.ConfirmAccount(user) as RedirectToActionResult;

            _logger.Received(1).LogError(Arg.Any<object>());
            Assert.AreEqual("GetResendConfirmationCodeView", confirmAccountResponse.ActionName);
        }

        [Test]
        public async Task SuccessfulAuthentication_ReturnsCorrectAuthHeaders_AndRedirectsTo_GetRemindersView()
        {
            var fakeAccessToken = "fakeAccessToken";
            var fakeIdToken = "fakeIdToken";
            var fakeRefreshToken = "fakeRefreshToken";

            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                Password = "fakePassword"
            };

            var mockAuthResult = new AuthenticationResultType
            {
                AccessToken = fakeAccessToken,
                IdToken = fakeIdToken,
                RefreshToken = fakeRefreshToken
            };

            _authAdapter.AuthenticateUserAsync(Arg.Any<AwsCognitoUser>()).Returns(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(mockAuthResult))
            });

            var context = _accountsController.ControllerContext.HttpContext = new DefaultHttpContext();
            var authResponse = await _accountsController.Login(user) as RedirectToActionResult;
            Assert.AreEqual("GetRemindersView", authResponse.ActionName);
            Assert.IsTrue(context.Response.Headers.Contains(new KeyValuePair<string, StringValues>("Authorization", fakeIdToken)));
            Assert.IsTrue(context.Response.Headers.Contains(new KeyValuePair<string, StringValues>("Refresh", fakeRefreshToken)));
            Assert.IsTrue(context.Response.Headers.Contains(new KeyValuePair<string, StringValues>("Access", fakeAccessToken)));
        }

        [Test]
        public async Task AttemptToLogin_ButUserDoesNotExist_RedirectsCreateAccountView()
        {
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                Password = "fakePassword"
            };

            _authAdapter.AuthenticateUserAsync(Arg.Any<AwsCognitoUser>()).Returns(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

            var authResponse = await _accountsController.Login(user) as RedirectToActionResult;
            Assert.AreEqual("GetCreateAccountView", authResponse.ActionName);
        }

        [Test]
        public async Task AttemptToLogin_ButUserIsNotConfirmed_RedirectsToConfirmAccountView()
        {
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                Password = "fakePassword"
            };

            _authAdapter.AuthenticateUserAsync(Arg.Any<AwsCognitoUser>()).Returns(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

            var authResponse = await _accountsController.Login(user) as RedirectToActionResult;
            Assert.AreEqual("GetConfirmAccountView", authResponse.ActionName);
        }

        [Test]
        public async Task IfExpiredCodeException_IsThrownBy_AuthenticateUserAsync_ExceptionIsLogged_AndUserRedirectedTo_GetLoginView()
        {
            _authAdapter.AuthenticateUserAsync(Arg.Any<AwsCognitoUser>()).Throws(new Exception());
            
            var user = new AwsCognitoUser
            {
                UserName = "fakeUsername",
                Password = "fakePassword"
            };

            var authResponse = await _accountsController.Login(user) as RedirectToActionResult;

            _logger.Received(1).LogError(Arg.Any<object>());
            Assert.AreEqual("GetLoginView", authResponse.ActionName);
        }
    }
}