using Backend.Controllers.Auth.Logic;
using Backend.Controllers.Auth.Requests;
using Backend.Controllers.Auth.Response;
using Backend.Tools;
using Config;
using Core.Workers;
using Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Backend.Controllers.Auth
{
    [Route("auth")]
    [ApiController]
    public class AuthController(ILogger<AuthController> logger ) : ControllerBase
    {
        [HttpPost("logIn")]
        public async Task<IActionResult> LogIn([FromBody] LogInRequest args, ApplicationContext context)
        {
            ArgumentNullException.ThrowIfNull(args.initData);
            var initData = new InitData(args.initData);
            ArgumentNullException.ThrowIfNull(initData.Hash);
            ArgumentNullException.ThrowIfNull(initData.User);
            if (initData.User == null || initData.AuthDate == null)
                return BadRequest("Wrong initData!");
            if (ProgramConfig.DEV == false &&
                initData.AuthDate!.Value.AddSeconds(10) < DateTime.UtcNow)
                return BadRequest("initData is too old!");
            try
            {
                await UserManager.RegisterOrUpdate(context, initData.User);
                string newSession = await SessionManager.OpenNew(context, initData.Hash, initData.User.Id);
                await context.SaveChangesAsync();
                var response = new LogInResponse()
                {
                    sessionCode = newSession,
                    me = initData.User,
                    accountCount = await context.Accounts
                        .CountAsync(a => 
                            a.Owner.Id == initData.User.Id && 
                            a.Status == Database.Enum.AccountStatus.Active)
                };
                response.Sign(initData.User.Id, newSession);
                return response.ToObjectResult();
            }
            catch (Exception ex)
            {
                logger.LogError("Unexpected exception when auth/logIn: {ex}", ex);
                return HttpErrorData.Create();
            }
        }

        [HttpPost("ping")]
        public IActionResult Ping([FromBody] HttpEmptyData args, [FromHeader] long userId, [FromHeader] string sessionCode) 
        {
            try { args.Verify(userId, sessionCode); } catch { return Unauthorized(); } 
            return new HttpEmptyData(userId, sessionCode).ToObjectResult();
        }

        public IActionResult LogOut([FromHeader] long userId, [FromHeader] string session)
        {
            return HttpErrorData.CreateAndSign(System.Net.HttpStatusCode.NotImplemented, "NotImplemented", userId, session);
        }

        // Это middleware
        public static async Task CustomAuthorization(HttpContext context, Func<Task> next)
        {
            if (context.Request.Method == "POST")
            {
                if (!context.Request.Headers.TryGetValue("userId", out StringValues tmp) ||
                    !long.TryParse(tmp.First(), out long userId))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
                if (userId != -1)
                {
                    context.Request.Headers.TryAdd("sessionCode", SessionManager.GetCodeByUser(userId));
                }
                else if (userId == -1 && (context.Request.Path.Value == null || !context.Request.Path.Value.EndsWith("auth/logIn")))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
            }
            await next.Invoke();
        }
    }
}
