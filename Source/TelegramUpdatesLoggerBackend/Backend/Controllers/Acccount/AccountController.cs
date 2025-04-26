using Backend.Controllers.Acccount.Logic;
using Backend.Controllers.Acccount.Logic.Types;
using Backend.Controllers.Acccount.Requests;
using Backend.Controllers.Acccount.Responses;
using Backend.Tools;
using Database;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Acccount
{
    [Route("account")]
    [ApiController]
    public class AccountController(ILogger<AccountController> logger) : ControllerBase
    {
        [HttpPost("newAccount")]
        public async Task<IActionResult> NewAccount(
            ApplicationContext context,
            [FromBody] NewAccountRequest args,
            [FromHeader] long userId,
            [FromHeader] string sessionCode)
        {
            ArgumentNullException.ThrowIfNull(args.phone);
            try { args.Verify(userId, sessionCode); } catch { return Unauthorized(); }
            try
            {
                var status = await AccountManager.NewAccount(context, args.phone, userId);
                var response = new NewAccountResponse()
                {
                    ownerId = userId,
                    phone = args.phone,
                    status = status,
                };
                response.Sign(userId, sessionCode);
                return response.ToObjectResult();
            }
            catch (Exception ex)
            {
                logger.LogError("Unexpected exception in account/newAccount: {ex}", ex);
                return HttpErrorData.CreateAndSign(System.Net.HttpStatusCode.InternalServerError, "Something wrong", userId, sessionCode);
            }
        }


        [HttpPost("setCode")]
        public async Task<IActionResult> SetCode(
            ApplicationContext context,
            [FromBody] SetCodeRequest args,
            [FromHeader] long userId,
            [FromHeader] string sessionCode)
        {
            ArgumentNullException.ThrowIfNull(args.phone);
            ArgumentNullException.ThrowIfNull(args.code);
            try { args.Verify(userId, sessionCode); } catch { return Unauthorized(); }
            try
            {
                var status = await AccountManager.NewAccountSetCode(context, args.phone, args.code);
                var response = new NewAccountResponse()
                {
                    ownerId = userId,
                    phone = args.phone,
                    status = status,
                };
                response.Sign(userId, sessionCode);
                return response.ToObjectResult();
            }
            catch (Exception ex)
            {
                logger.LogError("Unexpected exception in account/setCode: {ex}", ex);
                return HttpErrorData.CreateAndSign(System.Net.HttpStatusCode.InternalServerError, "Something wrong", userId, sessionCode);
            }
        }

        [HttpPost("setPassword")]
        public async Task<IActionResult> SetPassword(
            ApplicationContext context,
            [FromBody] SetPasswordRequest args,
            [FromHeader] long userId,
            [FromHeader] string sessionCode)
        {
            ArgumentNullException.ThrowIfNull(args.phone);
            ArgumentNullException.ThrowIfNull(args.password);
            try { args.Verify(userId, sessionCode); } catch { return Unauthorized(); }
            try
            {
                var status = await AccountManager
                    .NewAccountSetCloudPassword(context, args.phone, args.password);
                var response = new NewAccountResponse()
                {
                    ownerId = userId,
                    phone = args.phone,
                    status = status,
                };
                response.Sign(userId, sessionCode);
                return response.ToObjectResult();
            }
            catch (Exception ex)
            {
                logger.LogError("Unexpected exception in account/setPassword: {ex}", ex);
                return HttpErrorData.CreateAndSign(System.Net.HttpStatusCode.InternalServerError, "Something wrong", userId, sessionCode);
            }
        }



        [HttpPost("getMyAccounts")]
        public async Task<IActionResult> GetMyAccounts(
            ApplicationContext context,
            [FromBody] HttpEmptyData args,
            [FromHeader] long userId,
            [FromHeader] string sessionCode)
        {
            try { args.Verify(userId, sessionCode); } catch { return Unauthorized(); }
            try
            {
                var result = await AccountManager.GetAccounts(context, userId);
                var response = new GetMyAccountsResponse() { accounts = result };
                response.Sign(userId, sessionCode);
                return response.ToObjectResult();
            }
            catch (Exception ex)
            {
                logger.LogError("Unexpected exception in account/getMyAccounts: {ex}", ex);
                return HttpErrorData.CreateAndSign(System.Net.HttpStatusCode.InternalServerError, "Something wrong", userId, sessionCode);
            }
        }

        [HttpPost("getDialogs")]
        public async Task<IActionResult> GetDialogs(
            ApplicationContext context,
            [FromBody] GetDialogsRequest args,
            [FromHeader] long userId,
            [FromHeader] string sessionCode,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(args.phoneNumber);
            try { args.Verify(userId, sessionCode); } catch { return Unauthorized(); }
            try
            {
                List<DialogInfo> result = await AccountManager
                    .GetDialogs(context, userId, args.phoneNumber, args.offsetId, args.limit, cancellationToken);
                var response = new GetDialogsResponse() { dialogs = result };
                response.Sign(userId, sessionCode);
                return response.ToObjectResult();
            }
            catch (Exception ex)
            {
                logger.LogError("Unexpected exception in account/getDialogs: {ex}", ex);
                return HttpErrorData.CreateAndSign(System.Net.HttpStatusCode.InternalServerError, "Something wrong", userId, sessionCode);
            }
        }
    }
}
