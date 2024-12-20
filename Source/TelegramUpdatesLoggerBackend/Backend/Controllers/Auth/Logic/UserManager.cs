using Backend.Tools.Structs;
using Database;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers.Auth.Logic
{
    public static class UserManager
    {
        public static async Task RegisterOrUpdate(ApplicationContext context, User userInfo)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == userInfo.Id);
            if (user == null)
            {
                context.Users.Add(new()
                {
                    Id = userInfo.Id,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    LanguageCode = userInfo.LanguageCode,
                    Username = userInfo.Username
                });
            }
            else
            {
                user.FirstName = userInfo.FirstName;
                user.LastName = userInfo.LastName;
                user.Username = userInfo.Username;
            }
        }
    }
}
