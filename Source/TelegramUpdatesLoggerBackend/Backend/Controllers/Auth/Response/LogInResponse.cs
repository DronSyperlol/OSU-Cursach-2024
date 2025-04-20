#pragma warning disable IDE1006

using Backend.Tools;
using Backend.Tools.Structs;

namespace Backend.Controllers.Auth.Response
{
    public class LogInResponse : HttpDataBase
    {
        public required string sessionCode { get; set; }
        public required User me { get; set; }
        public int accountCount { get; set; }
    }
}
