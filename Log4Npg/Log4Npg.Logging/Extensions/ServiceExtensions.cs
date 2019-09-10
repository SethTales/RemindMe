using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Log4Npg.Logging.Data;
using Log4Npg.Models;

namespace Log4Npg.Logging.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddNpgLoggerScoped(this IServiceCollection services, string connectionString, LogLevel defaultLogLevel)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LoggingDatabaseContext>();
            optionsBuilder.UseNpgsql(connectionString);
            var dbContext = new LoggingDatabaseContext(optionsBuilder.Options);
            var loggingRepository = new LoggingRepository(dbContext);
            services.AddScoped<INpgLogger>(s => new NpgLogger(loggingRepository, defaultLogLevel));
        }
    }
}