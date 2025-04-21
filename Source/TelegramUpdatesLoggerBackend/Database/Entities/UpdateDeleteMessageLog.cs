using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    public class UpdateDeleteMessageLog : UpdateLog
    {
        public int MessageId { get; set; }
    }
}
