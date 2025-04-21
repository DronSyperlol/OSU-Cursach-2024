using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table("MessageUpdates")]
    public class UpdateMessageLog : UpdateLog
    {
        public int MessageId { get; set; }
        [MaxLength(4096)]
        public required string Text { get; set; }
        public required string TextEntities { get; set; } // https://core.telegram.org/type/MessageEntity
        public long? PrevEditId { get; set; } 
        public UpdateMessageLog? PrevEdit { get; set; } // Если сообщение изменено,
                                                        // то это поле указывает на предыдущую версию сообщения
        public DateTime? MsgDate { get; set; }
        // public MediaType? MediaType {get; set;}
        // [MaxLength(256)]
        // public string? MediaUrl {get; set;}


    }
}
