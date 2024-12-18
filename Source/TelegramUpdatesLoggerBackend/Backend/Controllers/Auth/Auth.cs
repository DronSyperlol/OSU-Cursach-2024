using Backend.Controllers.Auth.Logic;
using Backend.Controllers.Auth.Requests;
using Backend.Controllers.Auth.Response;
using Backend.Tools;
using Config;
using Database;
using Microsoft.AspNetCore.Mvc;

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

            var initData = new InitData(args.initData);
            if (initData.User == null && initData.AuthDate == null)
                return BadRequest("Wrong initData!");

            if (ProgramConfig.DEV == false && 
                initData.AuthDate!.Value.AddSeconds(10) < DateTime.UtcNow)
                return BadRequest("initData is too old!");

            var context = new ApplicationContext();
            try
            {
                await UserManager.RegisterOrUpdate(context, initData.User ?? new());
                string newSession = await SessionManager.CreateNewSession(context, initData);
                await context.SaveChangesAsync();
                return new ObjectResult(new LogInResponse() { 
                    SessionCode = newSession, 
                    Me = initData.User!.Value
                });
            }
            catch (Exception ex) {
                Console.Error.WriteLine(ex.ToString());
                return BadRequest("Something wrong");
            }
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
