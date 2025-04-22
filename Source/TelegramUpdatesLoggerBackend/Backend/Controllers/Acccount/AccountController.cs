using Backend.Controllers.Acccount.Requests;
using Backend.Controllers.Acccount.Responses;
using Backend.Tools;
using Core.Extensions;
using Core.Workers;
using Database;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Acccount
{
    [Route("account")]
    [ApiController]
    public class AccountController : ControllerBase
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
            var status = await AccountManager.OpenNewAccount(context, args.phone, userId);
            var response = new NewAccountResponse()
            {
                ownerId = userId,
                phone = args.phone,
                status = status,
            };
            response.Sign(userId, sessionCode);
            return response.ToObjectResult();
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
            var status = await AccountManager
                .SetCodeToNewAccount(context, args.phone, args.code);
            var response = new NewAccountResponse()
            {
                ownerId = userId,
                phone = args.phone,
                status = status,
            };
            response.Sign(userId, sessionCode);
            return response.ToObjectResult();
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
            try { args.Verify(userId, sessionCode); }
            catch { return Unauthorized(); }
            var status = await AccountManager
                .SetCloudPasswordToNewAccount(context, args.phone, args.password);
            var response = new NewAccountResponse()
            {
                ownerId = userId,
                phone = args.phone,
                status = status,
            };
            response.Sign(userId, sessionCode);
            return response.ToObjectResult();
        }

        

        [HttpPost("getMyAccounts")]
        public async Task<IActionResult> GetMyAccounts(
            ApplicationContext context,
            [FromBody] HttpEmptyData args,
            [FromHeader] long userId,
            [FromHeader] string sessionCode)
        {
            try { args.Verify(userId, sessionCode); } catch { return Unauthorized(); }
            List<Core.Types.AccountInfo> result = [];
            var accounts = context.Accounts.Where(a => a.OwnerId == userId);
            foreach (var acc in accounts)
            {
                var tmp = await (await AccountManager.Get(userId, acc.PhoneNumber)).GetMe();
                if (tmp != null)
                {
                    result.Add(tmp);
                }
            }
            var response = new GetMyAccountsResponse() { accounts = result };
            response.Sign(userId, sessionCode);
            return response.ToObjectResult();
        }

        [HttpPost("getDialogs")]
        public async Task<IActionResult> GetDialogs(
            [FromBody] GetDialogsRequest args,
            [FromHeader] long userId,
            [FromHeader] string sessionCode,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(args.phoneNumber);
            try { args.Verify(userId, sessionCode); } catch { return Unauthorized(); }
            List<Core.Types.DialogInfo> result = await (await AccountManager.Get(userId, args.phoneNumber)).GetDialogs(args.offsetId, args.limit, cancellationToken);
            var response = new GetDialogsResponse() { dialogs = result };
            response.Sign(userId, sessionCode);
            return response.ToObjectResult();
        }

        [HttpPost("getDialogHistory")]
        public async Task<IActionResult> GetDialogHistory(
            [FromBody] GetDialogHistoryRequest args,
            [FromHeader] long userId,
            [FromHeader] string sessionCode
            )
        {
            ArgumentNullException.ThrowIfNull(args.phoneNumber);
            ArgumentNullException.ThrowIfNull(args.dialogType);
            try { args.Verify(userId, sessionCode); } catch { return Unauthorized(); }
            var result = await (await AccountManager.Get(userId, args.phoneNumber)).GetDialogHistory(args.InputPeer, args.offsetId, args.limit);

            throw new NotImplementedException();
        }
    }
}
