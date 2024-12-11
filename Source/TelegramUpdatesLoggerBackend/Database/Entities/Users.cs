

using System.ComponentModel.DataAnnotations;

namespace Database.Entities
{
    internal class Users
    {
        public long Id { get; set; } // Вместо AUTO_INCREMENT будут айдишники из телеграма.
        [MaxLength(64)]
        public string FirstName { get; set; }
        [MaxLength(64)]
        public string? LastName { get; set; }
        [MaxLength(32)]
        public string? Username { get; set; }
        [MaxLength(10)]
        public string LanguageCode {  get; set; }

    }
}
