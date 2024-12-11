using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers.Acccount
{
    [Route("account")]
    [ApiController]
    public class Account : ControllerBase
    {
        [HttpPost("newAccount")]
        public IActionResult NewAccount()
        {

        }
    }
}
