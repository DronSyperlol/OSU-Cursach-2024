#pragma warning disable IDE1006

using Backend.Controllers.Acccount.Logic.Types;
using Backend.Tools;

namespace Backend.Controllers.Acccount.Responses
{
    public class GetDialogsResponse : HttpDataBase
    {
        public required List<DialogInfo> dialogs { get; set; }
    }
}
