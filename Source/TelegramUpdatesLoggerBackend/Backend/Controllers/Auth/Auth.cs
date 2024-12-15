﻿using Backend.Controllers.Auth.Requests;
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
