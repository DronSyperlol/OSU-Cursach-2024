using Backend.Tools;
using Backend.Tools.Structs;

namespace Backend.Controllers.Auth.Response
{
    public class LogInResponse : HttpDataBase
    {
        public string sessionCode { get; set; }
        public User me { get; set; }
        public int accountCount { get; set; }
    }
}
