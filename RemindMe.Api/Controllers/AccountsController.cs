using Microsoft.AspNetCore.Mvc;
using Log4Npg;
using RemindMe.Adapters;
using System.Threading.Tasks;
using RemindMe.Models;
using RemindMe.Data;
using System.Net.Http;
using System;
using System.Net;

namespace RemindMe.Api.Controllers
{
    [Route("[controller]")]
    public class AccountsController: Controller
    {
        private readonly INpgLogger _logger;
        private readonly IAuthAdapter _authAdapter;
        private readonly IApplicationRepository _appRepository;

        public AccountsController(INpgLogger logger, IAuthAdapter authAdapter, IApplicationRepository appRepository)
        {
            _logger = logger;
            _authAdapter = authAdapter;
            _appRepository = appRepository;
        }

        [HttpGet]
        [Route("create")]
        public IActionResult GetCreateAccountView(string message = "")
        {
            ViewBag.InfoMessage = message;
            return View("CreateAccount");
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAccount(AwsCognitoUser cognitoUser, string message = "")
        {
            //use this to display info messages after redirect
            ViewBag.InfoMessage = message;
            
            HttpResponseMessage signUpResponse;

            try 
            {
                signUpResponse = await _authAdapter.RegisterNewUserAsync(cognitoUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return RedirectToAction("GetCreateAccountView", "Accounts", new { message = "An error has occurred."});
            }

            switch (signUpResponse.StatusCode)
            {
                case HttpStatusCode.Conflict:
                    return RedirectToAction("GetCreateAccountView", "Accounts", new {message = $"The email address {cognitoUser.UserName} already has an account associated with it."});
                case HttpStatusCode.Created:
                    await _appRepository.AddUserAsync(cognitoUser.UserName);
                    return RedirectToAction("GetLoginView", "Accounts", new {message = $"Account successfully created. Please check your email for a confirmation code."});               
                default:
                    _logger.LogError(signUpResponse);
                    return RedirectToAction("GetCreateAccountView", "Accounts", new {message = "An error has occurred."});               
            }             
        }

        [HttpGet]
        [Route("login")]
        public IActionResult GetLoginView(string message = "")
        {
            return new ObjectResult("ROUTE NOT YET IMPLEMENTED");
        }

        [HttpGet]
        [Route("user/password")]
        public IActionResult GetResetPasswordView(string message = "")
        {
            return new ObjectResult("ROUTE NOT YET IMPLEMENTED");
        }

        [HttpGet]
        [Route("user/status")]
        public IActionResult GetConfirmAccountView(string message = "")
        {
            return new ObjectResult("ROUTE NOT YET IMPLEMENTED");
        }
    }
}