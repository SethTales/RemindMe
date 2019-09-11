using Log4Npg;
using RemindMe.Api.Controllers;
using RemindMe.Adapters;
using NUnit.Framework;
using NSubstitute;
using Microsoft.AspNetCore.Mvc;
using RemindMe.Data;
using System.Threading.Tasks;
using RemindMe.Models;
using System.Net.Http;
using System.Net;

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
            Assert.AreEqual("GetLoginView", createAccountResponse.ActionName);

        }
    }
}