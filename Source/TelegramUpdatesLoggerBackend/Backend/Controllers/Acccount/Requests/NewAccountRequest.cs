#pragma warning disable IDE1006

namespace Backend.Controllers.Acccount.Requests
{
    public class NewAccountRequest: Tools.HttpDataBase
    {
        public string? phone { get; set; }
    }
}
