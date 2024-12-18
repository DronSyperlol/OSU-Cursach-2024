using Backend.Tools;
using Backend.Tools.Structs;

namespace Backend.Controllers.Auth.Response
{
    public class LogInResponse : HttpDataBase
    {
        public string SessionCode { get; set; }
        public User Me { get; set; }
        public int AccountCount { get; set; }
    }
}
