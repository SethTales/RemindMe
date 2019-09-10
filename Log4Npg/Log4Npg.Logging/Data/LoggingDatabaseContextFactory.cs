using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Log4Npg.Logging.Data
{
    public class LoggingDatabaseContextFactory : IDesignTimeDbContextFactory<LoggingDatabaseContext>
    {
        public LoggingDatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LoggingDatabaseContext>();
            optionsBuilder.UseNpgsql("User ID=root;Password=password;server=localhost;Port=5432;Database=Log4NpgTestDatabase");

            return new LoggingDatabaseContext(optionsBuilder.Options);
        }
    }
}