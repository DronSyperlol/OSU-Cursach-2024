using Config;
using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Users> Users { get; set; }

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
