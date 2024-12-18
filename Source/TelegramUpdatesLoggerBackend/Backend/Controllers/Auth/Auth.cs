using Backend.Controllers.Auth.Logic;
using Backend.Controllers.Auth.Requests;
using Backend.Controllers.Auth.Response;
using Backend.Tools;
using Config;
using Database;
using Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Backend.Controllers.Auth
{
    [Route("auth")]
    [ApiController]
    public class Auth : ControllerBase
    {
        [HttpPost("logIn")]
        public async Task<IActionResult> LogIn([FromBody] LogInRequest args, ApplicationContext context)
        {
            ArgumentNullException.ThrowIfNull(args.initData);
            var initData = new InitData(args.initData);
            if (initData.User == null || initData.AuthDate == null)
                return BadRequest("Wrong initData!");
            if (ProgramConfig.DEV == false &&
                initData.AuthDate!.Value.AddSeconds(10) < DateTime.UtcNow)
                return BadRequest("initData is too old!");
            try
            {
                await UserManager.RegisterOrUpdate(context, initData.User);
                string newSession = await SessionManager.CreateNewSession(context, initData);
                await context.SaveChangesAsync();
                var response = new LogInResponse()
                {
                    SessionCode = newSession,
                    Me = initData.User,
                    AccountCount = await context.Accounts.CountAsync(a => a.Owner.Id == initData.User.Id)
                };
                response.Sign(initData.User.Id, newSession);
                return new ObjectResult(response);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return BadRequest("Something wrong");
            }
        }

        [HttpPost("ping")]
        public IActionResult Ping([FromBody] HttpEmptyData args, [FromHeader] long userId, [FromHeader] string sessionCode) 
        {
            try { 
                args.Verify(userId, sessionCode);
            }
            catch
            {
                return Unauthorized();
            } 
            return new ObjectResult(new HttpEmptyData(userId, sessionCode));
        }

        public IActionResult LogOut([FromHeader] string session)
        {
            throw new NotImplementedException();
        }

        public static async Task CustomAuthorization(HttpContext context, Func<Task> next)
        {
            if (context.Request.Method != "POST")
                await next.Invoke();
            else
            {
                if (!context.Request.Headers.TryGetValue("userId", out StringValues tmp) ||
                    !long.TryParse(tmp.First(), out long userId))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
                Session? session = await new ApplicationContext().Sessions
                    .FirstOrDefaultAsync(s => s.ToUser.Id == userId && s.Status == Database.Enum.SessionStatus.Active);
                if (userId == -1 && (context.Request.Path.Value == null || !context.Request.Path.Value.EndsWith("auth/logIn")))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
                context.Request.Headers.TryAdd("sessionCode", session?.Code ?? "_");
                await next.Invoke();
            }
        }
    }
}
