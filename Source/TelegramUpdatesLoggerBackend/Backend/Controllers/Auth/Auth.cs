using Backend.Controllers.Auth.Requests;
using Backend.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Auth
{
    [Route("auth")]
    [ApiController]
    public class Auth : ControllerBase
    {
        [HttpPost("logIn")]
        public IActionResult LogIn(LogInRequest args)
        {
            var webApp = new WebApp(args.initData);

            return new ObjectResult(new { StatusCode = 200 });
        }

        public IActionResult Ping([FromHeader] string session)
        {
            throw new NotImplementedException();
        }

        public IActionResult LogOut([FromHeader] string session)
        {
            throw new NotImplementedException();
        }
    }
}
