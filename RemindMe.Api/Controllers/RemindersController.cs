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

namespace RemindMe.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class RemindersController : Controller
    {
        private readonly INpgLogger _logger;
        public RemindersController(INpgLogger logger)
        {
            _logger = logger;
        }

        public IActionResult GetRemindersView(string message = "")
        {
            ViewBag.InfoMessage = message;
            return View("reminders");
        }
    }

}
