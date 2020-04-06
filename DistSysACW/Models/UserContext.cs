using Microsoft.EntityFrameworkCore;

namespace DistSysACW.Models
{
    public class UserContext : DbContext
    {
        public DbSet<LogArchive> LogArchive { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<User> Users { get; set; }

        //TODO: Task13

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=DistSysACW.db;");
        }
    }
}