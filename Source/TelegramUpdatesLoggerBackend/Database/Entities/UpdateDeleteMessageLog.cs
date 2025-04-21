using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table("DeleteMessageUpdates")]
    public class UpdateDeleteMessageLog : UpdateLog
    {
        public int MessageId { get; set; }
    }
}
