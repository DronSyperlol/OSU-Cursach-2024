using Backend.Controllers.Target.Logic;
using Backend.Controllers.Target.Requests;
using Backend.Controllers.Target.Response;
using Backend.Tools;
using Database;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Target
{
    [Route("target")]
    [ApiController]
    public class TargetController(ILogger<TargetController> logger) : ControllerBase
    {
        [HttpPost("updateTarget")]
        public async Task<IActionResult> UpdateTarget(
            ApplicationContext context,
            [FromBody] UpdateTargetRequest args,
            [FromHeader] long userId,
            [FromHeader] string sessionCode,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(args.phoneNumber);
            ArgumentNullException.ThrowIfNull(args.enable);
            try { args.Verify(userId, sessionCode); } catch { return Unauthorized(); }
            try
            {
                await TargetManager.SetTarget(context, userId, args.phoneNumber, args.peerId, args.accessHash, args.enable ?? false, cancellationToken);
                var response = new TargetStatusResponse()
                {
                    peerId = args.peerId,
                    enable = args.enable ?? false,
                };
                response.Sign(userId, sessionCode);
                return response.ToObjectResult();
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning("User {userId} send request with wrong arguments un target/updateTarget: {ex}", userId, ex);
                return HttpErrorData.CreateAndSign(System.Net.HttpStatusCode.BadRequest, ex.Message, userId, sessionCode);
            }
            catch (Exception ex)
            {
                logger.LogError("Unexpected exception when target/updateTarget: {ex}", ex);
                return HttpErrorData.CreateAndSign(System.Net.HttpStatusCode.InternalServerError, "SomethingWrong", userId, sessionCode);
            }
        }
    }
}
