using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Log4Npg.Logging.Data;

namespace Log4Npg.Logging.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddNpgLoggerScoped(this IServiceCollection services, string connectionString)
        {
            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<LoggingDatabaseContext>(options =>
                    options.UseNpgsql(connectionString));
            services.AddScoped<ILoggingRepository, LoggingRepository>();
            services.AddScoped<INpgLogger, NpgLogger>();
        }
    }
}