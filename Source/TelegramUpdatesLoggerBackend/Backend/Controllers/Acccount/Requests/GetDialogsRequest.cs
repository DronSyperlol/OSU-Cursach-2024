using Backend.Tools;

namespace Backend.Controllers.Acccount.Requests
{
    public class GetDialogsRequest : HttpDataBase
    {
        public int offset { get; set; } = 0;
        public int limit { get; set; } = 10;
    }
}
