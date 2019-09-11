using Log4Npg;
using RemindMe.Api.Controllers;
using RemindMe.Adapters;
using NUnit.Framework;
using NSubstitute;
using Microsoft.AspNetCore.Mvc;

namespace RemindMe.Tests.UnitTests.Controllers
{
    [TestFixture]
    public class AccountsControllerTests
    {
        private INpgLogger _logger;
        private IAuthAdapter _authAdapter;
        private AccountsController _accountsController;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<INpgLogger>();
            _authAdapter = Substitute.For<IAuthAdapter>();
            _accountsController = new AccountsController(_logger, _authAdapter);
        }

        [Test]
        public void GetCreateAccountView_ReturnsExpectedView()
        {
            var getCreateAccountViewResult = _accountsController.GetCreateAccountView(string.Empty) as ViewResult;
            Assert.AreEqual("CreateAccount", getCreateAccountViewResult.ViewName);
        }
    }
}