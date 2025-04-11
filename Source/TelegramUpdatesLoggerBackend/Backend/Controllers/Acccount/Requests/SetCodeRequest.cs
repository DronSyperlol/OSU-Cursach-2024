#pragma warning disable IDE1006


namespace Backend.Controllers.Acccount.Requests
{
    public class SetCodeRequest: Tools.HttpDataBase
    {
        public string? phone { get; set; }
        public string? code { get; set; }
    }
}
