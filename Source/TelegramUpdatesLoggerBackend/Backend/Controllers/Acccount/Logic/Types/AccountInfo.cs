using Backend.Tools;

namespace Backend.Controllers.Acccount.Logic.Types
{
    public class AccountInfo : ParsebleToDictionaryBase
    {
        public required string PhoneNumber { get; set; }
        public required string Title { get; set; }
        public string? Username { get; set; }
        public required string PhotoUrl { get; set; }

    }
}
