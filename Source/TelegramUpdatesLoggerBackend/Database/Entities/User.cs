

using System.ComponentModel.DataAnnotations;

namespace Database.Entities
{
    public class User
    {
        public required long Id { get; set; } // Вместо AUTO_INCREMENT будут айдишники из телеграма.
        [MaxLength(64)]
        public required string FirstName { get; set; }
        [MaxLength(64)]
        public string? LastName { get; set; }
        [MaxLength(32)]
        public string? Username { get; set; }
        [MaxLength(10)]
        public string? LanguageCode {  get; set; }


        public static User CreateAndAttach(ApplicationContext context, long userId)
        {
            var user = new User() { Id = userId, FirstName = "" };
            context.Users.Attach(user);
            return user;
        }
        
        public static User CreateAndAttachOrUpdate(ApplicationContext context, long userId)
        {
            var user = new User() { Id = userId, FirstName = "" };
            try
            {
                context.Users.Attach(user);
            }
            catch
            {
                context.Users.Update(user);
                context.Users.Attach(user);
            }
            return user;
        }

    }
}
