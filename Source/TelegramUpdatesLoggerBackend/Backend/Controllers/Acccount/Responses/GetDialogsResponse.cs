using Backend.Tools;
using Core.Types;

namespace Backend.Controllers.Acccount.Responses
{
    public class GetDialogsResponse : HttpDataBase
    {
        public required List<DialogInfo> dialogs { get; set; }
    }
}
