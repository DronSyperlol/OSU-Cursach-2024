using Core.Extensions;
using Database;
using Database.Entities;
using Database.Enum;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers.Target.Logic
{
    public class TargetManager
    {
        public static async Task SetTarget(
            ApplicationContext context,
            long userId, string phone,
            long peerId, string? accessHash,
            bool enable,
            CancellationToken cancellationToken)
        {
            var account = context.Accounts.FirstOrDefault(a => a.OwnerId == userId && a.PhoneNumber == phone)
                ?? throw new KeyNotFoundException($"Account with phone {phone} not found for user {userId}");
            var target = await context.Targets
                .OrderByDescending(t => t.Time)
                .FirstOrDefaultAsync(t => t.PeerId == peerId && t.FromAccount.PhoneNumber == phone, cancellationToken: cancellationToken);
            if (target == null && enable == false) return;
            var newTargetLog = new LoggingTarget()
            {
                AccessHash = accessHash != null ? long.Parse(accessHash) : null,
                FromAccount = account,
                PeerId = peerId,
                Status = enable ? LoggingTargetStatus.Enabled : LoggingTargetStatus.Disabled,
            };
            await context.Targets.AddAsync(newTargetLog, cancellationToken: cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await Core.Services.LoggingService.Instance.Update(cancellationToken);
        }
    }
}
