using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

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
            //optionsBuilder.UseSqlite("Data Source=DistSysACW.db;");
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DistSysACW;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().Property(user => user.Logs).
            HasConversion(log => JsonConvert.SerializeObject(log),
            log => JsonConvert.DeserializeObject<List<Log>>(log));

            builder.Entity<User>().HasData(new User
            {
                ApiKey = "06356a9f-f7a2-4228-a722-e5ac48e6832a",
                Logs = new List<Log>(),
                Role = UserRole.Admin,
                UserName = "UserOne"
            });
        }
    }
}