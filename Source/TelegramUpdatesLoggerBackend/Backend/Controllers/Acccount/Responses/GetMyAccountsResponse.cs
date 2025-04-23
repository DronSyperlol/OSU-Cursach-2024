#pragma warning disable IDE1006

using Backend.Controllers.Acccount.Logic.Types;
using Backend.Tools;

namespace Backend.Controllers.Acccount.Responses
{
    public class GetMyAccountsResponse : HttpDataBase
    {
        public required List<AccountInfo> accounts { get; set; }
    }
}
