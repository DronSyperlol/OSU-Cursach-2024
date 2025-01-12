using Backend.Tools;
using Core.Types;

namespace Backend.Controllers.Acccount.Responses
{
    public class GetMyAccountsResponse : HttpDataBase
    {
        public required List<AccountInfo> accounts { get; set; }
    }
}
