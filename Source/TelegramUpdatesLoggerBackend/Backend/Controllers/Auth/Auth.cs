using Backend.Controllers.Auth.Logic;
using Backend.Controllers.Auth.Requests;
using Backend.Tools;
using Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Backend.Controllers.Auth
{
    [Route("auth")]
    [ApiController]
    public class Auth : ControllerBase
    {
        [HttpPost("logIn")]
        public async Task<IActionResult> LogIn(LogInRequest args)
        {
            ArgumentNullException.ThrowIfNull(args.initData);

            Console.WriteLine("LogIn triggered");

            var webApp = new WebApp(args.initData);
            if (webApp.User == null)
                BadRequest("Wrong initData!");

            var context = new ApplicationContext();
            await UserManager.RegisterOrUpdate(context, webApp.User ?? new());

            return new ObjectResult(new { StatusCode = 200 });
        }

        [HttpPost("ping")]
        public IActionResult Ping()
        {
            throw new NotImplementedException();
        }

        public IActionResult LogOut([FromHeader] string session)
        {
            throw new NotImplementedException();
        }
    }
}
