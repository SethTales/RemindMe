using Microsoft.EntityFrameworkCore;
using Log4Npg.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore.Design;

namespace Log4Npg.Logging.Data
{
    public class LoggingDatabaseContext : DbContext
    {
        public LoggingDatabaseContext(DbContextOptions<LoggingDatabaseContext> options) : base(options)
        {

        }

        public DbSet<LogEntry> LogEntries {get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<LogEntry>()
                .Property(l => l.Message)
                .HasColumnType("jsonb");
        }
    }
}