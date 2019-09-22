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
using Microsoft.AspNetCore.Authorization;
using RemindMe.Api.Controllers.Utils;

namespace RemindMe.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class RemindersController : Controller
    {
        private readonly INpgLogger _logger;
        private readonly IReminderRepository _reminderRepository;
        private readonly IUserRepository _userRepository;
        public RemindersController(IReminderRepository reminderRepository, IUserRepository userRepository, INpgLogger logger)
        {
            _reminderRepository = reminderRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public IActionResult GetRemindersView(string message = "")
        {
            ViewBag.InfoMessage = message;
            var username = TokenUtil.GetUserNameFromToken(Request.Cookies["IdToken"]);
            var user = _userRepository.LookupUserByUsername(username);
            var reminders = _reminderRepository.GetRemindersForUser(user.UserId);
            return View("reminders", reminders);
        }
    }

}
