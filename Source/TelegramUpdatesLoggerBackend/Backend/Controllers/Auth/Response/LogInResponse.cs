using Backend.Tools.Structs;

namespace Backend.Controllers.Auth.Response
{
    public class LogInResponse
    {
        public string SessionCode { get; set; }
        public User Me { get; set; }
    }
}
