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
                Type = LoggingTargetType.Messages,
                Status = enable ? LoggingTargetStatus.Enabled : LoggingTargetStatus.Disabled,
                PrevTarget = target,
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
            if (offsetId == 0) offsetId = int.MaxValue;
            List<LogInfo> result = [];
            var messageUpdatesQuery = context.UpdatesMessage
                .Include(l => l.LoggingTarget)
                .Include(l => l.LoggingTarget.PrevTarget)
                .Include(l => l.LoggingTarget.FromAccount)
                .Where(l =>
                    l.LoggingTarget.FromAccount.PhoneNumber == phoneNumber &&
                    l.LoggingTarget.PeerId == peerId &&
                    l.MessageId < offsetId)
                .GroupBy(l => l.MessageId)
                .Select(g => new
                {
                    g.Key,
                    List = g.OrderByDescending(l => l.Id).ToList(),
                })
                .OrderByDescending(l => l.Key)
                .Take(limit);
            var deletedMessages = await context.UpdatesDeleteMessage
                .Where(dm => messageUpdatesQuery.Any(g => g.Key == dm.MessageId))
                .ToListAsync(cancellationToken);
            var messageUpdates = await messageUpdatesQuery.ToListAsync(cancellationToken);
            messageUpdates.ForEach(g =>
            {
                var tmp = g.List.Select(LogInfo.FromMessageLog);
                var deleted = deletedMessages.FirstOrDefault(l => l.MessageId == g.Key);
                LogInfo main;
                if (deleted != null)
                    main = LogInfo.FromDeletedMessageLog(deleted);
                else
                    main = tmp.OrderByDescending(l => l.LogTime).First();
                main.PrevChanges = [.. tmp.Where(l => l.DbId != main.DbId)];
                result.Add(main);
            });
            List<LogInfo> targets = [];
            messageUpdates.ForEach(mu =>
            {
                targets.AddRange(mu.List.Select(l => LogInfo.FromTarget(l.LoggingTarget)));
                targets.AddRange(mu.List
                    .Where(l => l.LoggingTarget.PrevTarget != null)
                    .Select(l => LogInfo.FromTarget(l.LoggingTarget.PrevTarget!)));
            });
            result.AddRange(targets.DistinctBy(li => li.DbId));
            return [.. result.OrderByDescending(l => l.LogTime).Take(limit)];
        }
    }
}
