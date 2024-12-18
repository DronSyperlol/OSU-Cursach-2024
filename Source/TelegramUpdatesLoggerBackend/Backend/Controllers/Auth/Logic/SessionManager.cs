using Backend.Tools;
using Database;
using Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Controllers.Auth.Logic
{
    public static class SessionManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Code of new session</returns>
        public static async Task<string> CreateNewSession(ApplicationContext context, InitData initData)
        {
            ArgumentNullException.ThrowIfNull(initData.User);
            ArgumentNullException.ThrowIfNull(initData.Hash);
            byte[] sessionCode = HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(UserManager.GetUniqueCode(initData.User.Value.Id)),
                Encoding.UTF8.GetBytes(initData.Hash));
            Session? currentSession = await context.Sessions
                .FirstOrDefaultAsync(s =>
                    s.ToUser.Id == initData.User.Value.Id &&
                    s.Status == Database.Enum.SessionStatus.Active);
            if (currentSession != null) { 
                currentSession.Status = Database.Enum.SessionStatus.Revoked;
                currentSession.DiedAt = DateTime.UtcNow;
            }
            User user = new() { Id = initData.User.Value.Id };
            try
            {
                context.Users.Attach(user);
            }
            catch
            {
                context.Users.Update(user);
                context.Users.Attach(user);
            }
            var newSession = new Session()
            {
                CreatedAt = DateTime.UtcNow,
                Status = Database.Enum.SessionStatus.Active,
                ToUser = user,
                DiedAt = null
            };
            newSession.Code = Convert.ToBase64String(
                HMACSHA256.HashData(
                    sessionCode,
                    Encoding.UTF8.GetBytes(newSession.CreatedAt.ToString("dd.MM.yyyy_HH:mm:ffffff"))));
            await context.Sessions.AddAsync(newSession);
            return newSession.Code;
        }
    }
}
