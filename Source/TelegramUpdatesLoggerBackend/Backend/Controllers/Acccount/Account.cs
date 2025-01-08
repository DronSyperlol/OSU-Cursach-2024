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
            var status = await AccountManager.OpenNewAccount(args.phone, userId);
            var response = new NewAccountResponse()
            {
                ownerId = userId,
                phone = args.phone,
                status  = status,
            };
            response.Sign(userId, sessionCode);
            return new ObjectResult(response);
        }


        [HttpPost("setCode")]
        public async Task<IActionResult> SetCode(
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
            var status = await AccountManager.SetCodeToNewAccount(args.phone, args.code);
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
            var status = await AccountManager.SetCloudPasswordToNewAccount(args.phone, args.password);
            var response = new NewAccountResponse()
            {
                ownerId = userId,
                phone = args.phone,
                status = status,
            };
            response.Sign(userId, sessionCode);
            return new ObjectResult(response);
        }
    }
}
