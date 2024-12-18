#pragma warning disable IDE1006

using Backend.Tools;

namespace Backend.Controllers.Auth.Requests
{
    public class LogInRequest : HttpDataBase
    {
        public string? initData { get; set; }

    }
}
