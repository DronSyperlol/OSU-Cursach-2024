using Backend.Tools;

namespace Backend.Controllers.Acccount.Requests
{
    public class GetDialogsRequest : HttpDataBase
    {
        public string phoneNumber { get; set; }
        public int offsetId { get; set; } = 0;
        public int limit { get; set; } = 10;
    }
}
