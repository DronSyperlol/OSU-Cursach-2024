using Config;
using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountLog> AccountHistory { get; set; }
        public DbSet<LoggingTarget> Targets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                ProgramConfig.DatabaseAuth.ConnectionString,
                new MySqlServerVersion("1.0"), mySqlOptionsAction =>
                {
                    mySqlOptionsAction.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });
        }
    }
}
