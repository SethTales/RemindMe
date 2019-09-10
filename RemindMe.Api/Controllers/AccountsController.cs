using Microsoft.AspNetCore.Mvc;
using Log4Npg;
using RemindMe.Adapters;
using System.Threading.Tasks;
using RemindMe.Models;

namespace RemindMe.Api.Controllers
{
    [Route("[controller]")]
    public class AccountsController: Controller
    {
        private readonly INpgLogger _logger;
        private readonly IAuthAdapter _authAdapter;

        public AccountsController(INpgLogger logger, IAuthAdapter authAdapter)
        {
            _logger = logger;
            _authAdapter = authAdapter;
        }

        [HttpGet]
        [Route("create")]
        public IActionResult CreateAccount(string message = "")
        {
            ViewBag.InfoMessage = message;
            return View();
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAccount(RemindMeUser user, string message = "")
        {
            return new ObjectResult("ROUTE NOT YET IMPLEMENTED");
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login(string message = "")
        {
            return new ObjectResult("ROUTE NOT YET IMPLEMENTED");
        }

        [HttpGet]
        [Route("user/password")]
        public IActionResult ResetPassword(string message = "")
        {
            return new ObjectResult("ROUTE NOT YET IMPLEMENTED");
        }

        [HttpGet]
        [Route("user/status")]
        public IActionResult ConfirmAccount(string message = "")
        {
            return new ObjectResult("ROUTE NOT YET IMPLEMENTED");
        }
    }
}