using Backend.Controllers.Acccount.Requests;
using Backend.Controllers.Acccount.Responses;
using Core.Workers;
using Database;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Acccount
{
    [Route("account")]
    [ApiController]
    public class Account : ControllerBase
    {
        [HttpPost("newAccount")]
        public async Task<IActionResult> NewAccount(
            ApplicationContext context,
            [FromBody] NewAccountRequest args,
            [FromHeader] long userId,
            [FromHeader] string sessionCode)
        {
            ArgumentNullException.ThrowIfNull(args.phone);
            try
            {
                args.Verify(userId, sessionCode);
            }
            catch
            {
                return Unauthorized();
            }
            var status = await AccountManager.OpenNewAccount(context, args.phone, userId);
            var response = new NewAccountResponse()
            {
                ownerId = userId,
                phone = args.phone,
                status = status,
            };
            response.Sign(userId, sessionCode);
            return new ObjectResult(response);
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
            try
            {
                args.Verify(userId, sessionCode);
            }
            catch
            {
                return Unauthorized();
            }
            var status = await AccountManager
                .SetCodeToNewAccount(context, args.phone, args.code);
            var response = new NewAccountResponse()
            {
                ownerId = userId,
                phone = args.phone,
                status = status,
            };
            response.Sign(userId, sessionCode);
            return new ObjectResult(response);
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
            try
            {
                args.Verify(userId, sessionCode);
            }
            catch
            {
                return Unauthorized();
            }
            var status = await AccountManager
                .SetCloudPasswordToNewAccount(context, args.phone, args.password);
            var response = new NewAccountResponse()
            {
                ownerId = userId,
                phone = args.phone,
                status = status,
            };
            response.Sign(userId, sessionCode);
            return new ObjectResult(response);
        }

        [HttpPost("getDialogs")]
        public async Task<IActionResult> GetDialogs(
            [FromBody] GetDialogsRequest args,
            [FromHeader] long userId,
            [FromHeader] string sessionCode)
        {
            try
            {
                args.Verify(userId, sessionCode);
            }
            catch
            {
                return Unauthorized();
            }
            throw new NotImplementedException();
        }
    }
}
