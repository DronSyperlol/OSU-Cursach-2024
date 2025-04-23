#pragma warning disable IDE1006

using Backend.Controllers.Target.Logic.Types;
using Backend.Tools;

namespace Backend.Controllers.Target.Response
{
    public class GetSavedLogsResponse : HttpDataBase // TODO ... Проверить генерацию подписи с этими данными
    {
        public required List<LogInfo> logs { get; set; }
    }
}
