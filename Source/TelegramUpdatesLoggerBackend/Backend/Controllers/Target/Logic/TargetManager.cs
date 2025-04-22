using Core.Extensions;
using Database;
using Database.Entities;
using Database.Enum;

namespace Backend.Controllers.Target.Logic
{
    public class TargetManager
    {
        public static async Task SetTarget(
            ApplicationContext context, 
            long userId, string phone, 
            long peerId, string? accessHash,
            bool enable)
        {
            var account = context.Accounts.FirstOrDefault(a => a.OwnerId == userId && a.PhoneNumber == phone)
                ?? throw new KeyNotFoundException($"Account with phone {phone} not found for user {userId}");
            var target = context.Targets.FirstOrDefault(t => t.PeerId == peerId && t.FromAccount.PhoneNumber == phone);
            if (target == null) {
                target ??= new LoggingTarget()
                {
                    AccessHash = accessHash != null ? long.Parse(accessHash) : null,
                    FromAccount = account,
                    PeerId = peerId,
                    Status = enable ? LoggingTargetStatus.Enabled : LoggingTargetStatus.Disabled,
                };
                await context.Targets.AddAsync(target);
            }
            else 
                target.Status = enable ? LoggingTargetStatus.Enabled : LoggingTargetStatus.Disabled;
            await context.SaveChangesAsync();   
        }
    }
}
