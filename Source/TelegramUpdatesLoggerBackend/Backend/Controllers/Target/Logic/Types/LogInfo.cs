using Backend.Tools;
using Database.Entities;
using Database.Enum;

namespace Backend.Controllers.Target.Logic.Types
{
    public class LogInfo : ParsebleToDictionaryBase
    {
        public long DbId { get; set; }
        public required string Type { get; set; }
        public required string Message { get; set; }
        public required string MessageEntities { get; set; }
        public DateTime? MessageDate { get; set; }
        public int? MessageId { get; set; }
        public long FromId { get; set; }
        public DateTime LogTime { get; set; }
        public List<LogInfo>? PrevChanges { get; set; } = [];
        public long? PrevId { get; set; }


        public struct Types
        {
            public const string Message = "Message";
            public const string MessageDeleted = "MessageDeleted";
            public const string Specifications = "Specifications";
        }

        public static LogInfo FromTarget(LoggingTarget tl)
        {
            return new LogInfo
            {
                DbId = tl.Id,
                FromId = -1,
                LogTime = tl.Time,
                Message = tl.Type switch
                {
                    LoggingTargetType.Messages => "Отслеживание сообщений: ",
                    _ => "Unknown"
                } + tl.Status switch
                {
                    LoggingTargetStatus.Enabled => "включено",
                    LoggingTargetStatus.Disabled => "выключено",
                    LoggingTargetStatus.Deleted => "больше недоступно (чат удалён)",
                    LoggingTargetStatus.Banned => "больше недоступно: (чат недоступен или в чёрном списке)",
                    LoggingTargetStatus.Failure => "нарушено. Ошибка: " + tl.Error,
                    _ => "unknown action"
                },
                MessageDate = null,
                MessageEntities = "{}",
                MessageId = null,
                PrevId = null,
                Type = LogInfo.Types.Specifications,
            };
        }

        public static LogInfo FromMessageLog(UpdateMessageLog uml)
        {
            return new LogInfo
            {
                DbId = uml.Id,
                FromId = uml.FromId,
                LogTime = uml.Time,
                Message = uml.Text,
                MessageEntities = uml.TextEntities,
                MessageDate = uml.MsgDate,
                MessageId = uml.MessageId,
                PrevId = uml.PrevEditId,
                Type = LogInfo.Types.Message,
            };
        }

        public static LogInfo FromDeletedMessageLog(UpdateDeleteMessageLog udml)
        {
            return new LogInfo
            {
                DbId = udml.Id,
                FromId = -1,
                LogTime = udml.Time,
                Message = "",
                MessageEntities = "",
                MessageDate = null,
                MessageId = udml.MessageId,
                PrevId = null,
                Type = LogInfo.Types.MessageDeleted,
            };
        }
    }
}
