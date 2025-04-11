#pragma warning disable IDE1006

namespace Backend.Controllers.Acccount.Requests
{
    public class SetPasswordRequest: Tools.HttpDataBase
    {
        public string? phone { get; set; }
        public string? password { get; set; }
    }
}
