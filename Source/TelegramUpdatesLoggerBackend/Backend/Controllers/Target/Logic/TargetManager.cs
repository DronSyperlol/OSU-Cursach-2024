using Backend.Controllers.Target.Logic.Types;
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

        public static async Task<List<LogInfo>> GetLogsHistory(
            ApplicationContext context,
            string phoneNumber, long peerId,
            int offsetId, int limit,
            CancellationToken cancellationToken)
        {
            List<LogInfo> result = [];
            var messageUpdates = await context.UpdatesMessage
                .Include(l => l.LoggingTarget)
                .Include(l => l.LoggingTarget.FromAccount)
                .OrderByDescending(l => l.Id)
                .Where(l =>
                    l.LoggingTarget.FromAccount.PhoneNumber == phoneNumber &&
                    l.LoggingTarget.PeerId == peerId &&
                    l.Id < offsetId)
                .Take(limit)
                .ToListAsync(cancellationToken);
            messageUpdates.ForEach(mu => result.Add(LogInfo.FromMessageLog(mu)));
            var targetsLogs = await context.Targets
                .Include(l => l.FromAccount)
                .Where(l =>
                    l.FromAccount.PhoneNumber == phoneNumber &&
                    l.PeerId == peerId && 
                    l.Time >= messageUpdates.Min(mu => mu.Time) &&
                    l.Time <= messageUpdates.Max(mu => mu.Time))
                .ToListAsync(cancellationToken);
            targetsLogs.ForEach(tl => result.Add(LogInfo.FromTarget(tl)));
            if (result.Count == 0) return [];
            var deletedMesssagesUpdates = await context.UpdatesDeleteMessage
                .Include(l => l.LoggingTarget)
                .Include(l => l.LoggingTarget.FromAccount)
                .Where(l =>
                    l.LoggingTarget.FromAccount.PhoneNumber == phoneNumber &&
                    l.LoggingTarget.PeerId == peerId &&
                    l.Id > messageUpdates.First().Id &&
                    l.Id <= messageUpdates.Last().Id)
                .ToListAsync(cancellationToken);
            deletedMesssagesUpdates.ForEach(dmu => result.Add(LogInfo.FromDeletedMessageLog(dmu)));
            return [.. result.OrderBy(l => l.Id).Take(limit)];
        }
    }
}
