using Microsoft.AspNetCore.Mvc;
using Log4Npg;
using RemindMe.Adapters;
using System.Threading.Tasks;
using RemindMe.Models;
using RemindMe.Data;
using System.Net.Http;
using System;
using System.Net;
using Amazon.CognitoIdentityProvider.Model;
using Newtonsoft.Json;

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
            return View("createAccount");
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
                    return RedirectToAction("GetConfirmAccountView", "Accounts", new {message = $"Account successfully created. Please check your email for a confirmation code."});               
                default:
                    _logger.LogError(signUpResponse);
                    return RedirectToAction("GetCreateAccountView", "Accounts", new {message = "An error has occurred."});               
            }             
        }

        [HttpGet]
        [Route("user/status")]
        public IActionResult GetConfirmAccountView(string message = "")
        {
            ViewBag.InfoMessage = message;
            return View("confirmAccount");
        }

        [HttpPost]
        [Route("user/status")]
        public async Task<IActionResult> ConfirmAccount(AwsCognitoUser cognitoUser, string message = "")
        {
            ViewBag.InfoMessage = message;

            HttpResponseMessage confirmSignUpResponse;

            try
            {
                confirmSignUpResponse = await _authAdapter.ConfirmUserAsync(cognitoUser);
            }
            catch (CodeMismatchException ex)
            {
                _logger.LogError(ex);
                return RedirectToAction("GetConfirmAccountView", "Accounts", new { message = "Invalid verification code provided. Please try again."});
            }
            catch (ExpiredCodeException ex)
            {
                _logger.LogError(ex);
                return RedirectToAction("GetResendConfirmationCodeView", "Accounts", new { message = "Verification code expired. Please request a new code."});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return RedirectToAction("GetConfirmAccountView", "Accounts", new { message = "An error has occurred." });
            }
            
            switch (confirmSignUpResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return RedirectToAction("GetCreateAccountView", "Accounts", new { message = $"The username {cognitoUser.UserName} does not exist. Please create an account." });
                case HttpStatusCode.Conflict:
                    return RedirectToAction("GetLoginView", "Accounts", new { message = $"The username {cognitoUser.UserName} is already confirmed. Please login to continue." });
                case HttpStatusCode.OK:
                    return RedirectToAction("GetLoginView", "Accounts", new { message = "Account confirmed. Please login to continue." });
                default:
                    _logger.LogError(confirmSignUpResponse);
                    return RedirectToAction("GetCreateAccountView", "Accounts", new { message = "An error has occurred." });
            }
        }

        [HttpGet]
        [Route("users/authenticate")]
        public IActionResult GetLoginView(string message = "")
        {
            ViewBag.InfoMessage = message;
            return View("login");
        }

        [HttpPost]
        [Route("users/authenticate")]
        public async Task<IActionResult> Login(AwsCognitoUser cognitoUser, string message = "")
        {
            ViewBag.InfoMessage = message;
            HttpResponseMessage loginResponse;
            try
            {
                loginResponse = await _authAdapter.AuthenticateUserAsync(cognitoUser);
                if (loginResponse.IsSuccessStatusCode)
                {
                    var authResult = JsonConvert.DeserializeObject<AuthenticationResultType>(await loginResponse.Content.ReadAsStringAsync());
                    HttpContext.Response.Headers.Add("Authorization", authResult.IdToken);
                    HttpContext.Response.Headers.Add("Refresh", authResult.RefreshToken);
                    HttpContext.Response.Headers.Add("Access", authResult.AccessToken);
                }             
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return RedirectToAction("GetLoginView", "Accounts", new { message = "An error has occurred." });
            }

            switch (loginResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return RedirectToAction("GetRemindersView", "Reminders");
                case HttpStatusCode.BadRequest:
                    return RedirectToAction("GetConfirmAccountView", "Accounts", new { message = $"Login failed. User {cognitoUser.UserName} is unconfirmed. Please confirm account to continue." });
                case HttpStatusCode.NotFound:
                    return RedirectToAction("GetCreateAccountView", "Accounts", new { message = $"Login failed. User {cognitoUser.UserName} does not exist. Please create an account to continue."});
                default:
                    _logger.LogError(loginResponse);
                    return RedirectToAction("GetLoginView", "Accounts", new { message = "An error has occurred." });
            }
        }

        [HttpGet]
        [Route("user/password")]
        public IActionResult GetResetPasswordView(string message = "")
        {
            ViewBag.InfoMessage = message;
            return View("resetPassword");
        }

        [HttpGet]
        [Route("resend")]
        public IActionResult GetResendConfirmationCodeView(string message = "")
        {
            ViewBag.InfoMessage = message;
            return View("resendConfirmationCode");
        }
    }
}