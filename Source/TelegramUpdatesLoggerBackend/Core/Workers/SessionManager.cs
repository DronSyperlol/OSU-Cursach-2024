using Core.Interfaces;
using Core.Tools;
using Database;
using Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Core.Workers
{
    public class SessionManager : IWorker
    {
        static readonly List<Types.Session> Sessions = [];

        async Task IWorker.Handle(ApplicationContext context)
        {
            bool needSave = false;
            foreach (Types.Session session in Sessions.ToArray())
            {
                if (session.LastTrigger.AddSeconds(15) <= DateTime.UtcNow)
                {
                    var dbEntity = await context.Sessions
                        .FirstOrDefaultAsync(s => s.Id == session.Id);
                    if (dbEntity != null)
                    {
                        dbEntity.DiedAt = DateTime.UtcNow;
                        dbEntity.Status = Database.Enum.SessionStatus.Died;
                        needSave = true;
                    }
                    Sessions.Remove(session);
                }
            }
            if (needSave)
                await context.SaveChangesAsync();
        }

        public static async Task<string> OpenNew(ApplicationContext context, string hash, long userId)
        {
            byte[] sessionCode = HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(userId.ToEncodedString()),
                Encoding.UTF8.GetBytes(hash));
            Session? currentSession = await context.Sessions
                .FirstOrDefaultAsync(s =>
                    s.ToUser.Id == userId &&
                    s.Status == Database.Enum.SessionStatus.Active);
            if (currentSession != null)
            {
                currentSession.Status = Database.Enum.SessionStatus.Revoked;
                currentSession.DiedAt = DateTime.UtcNow;
                if (Sessions.Any(s => s.Id == currentSession.Id))
                    Sessions.Remove(Sessions.First(s => s.Id == currentSession.Id));
            }
            User user = new() { Id = userId };
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
            await context.SaveChangesAsync();
            Sessions.Add(new(newSession));
            return newSession.Code;
        }

        public static string GetCodeByUser(long userId)
        {
            var tmp = Sessions.FirstOrDefault(s => s.ToUserId == userId);
            if (tmp == null)
                return "_";
            tmp.Trigger();
            return tmp.Code ?? "_";
        }
    }
}
