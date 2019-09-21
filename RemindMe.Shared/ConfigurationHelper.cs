using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.S3;
using Amazon;
using RemindMe.Adapters;
using Log4Npg.Logging.Extensions;
using Log4Npg.Models;
using RemindMe.Data;
using Microsoft.EntityFrameworkCore;

namespace RemindMe.Shared
{
    public static class ConfigurationHelper
    {
        public static void ConfigureSharedServices(IConfigurationRoot configuration, IServiceCollection services)
        {
            var deployBucket = configuration.GetSection("AWS").GetSection("S3")["DeployBucket"];
            var loggingDbConnectionStringKey = configuration.GetSection("AWS").GetSection("RDS")["LoggingDbConnectionStringKey"];
            var appDbConnectionStringKey = configuration.GetSection("AWS").GetSection("RDS")["AppDbConnectionStringKey"];

            var s3Client = new AmazonS3Client(RegionEndpoint.USWest2);
            var storageAdapter = new AwsS3Adapter(s3Client);

            var loggingDbConnectionString = storageAdapter.GetObjectAsync(deployBucket, loggingDbConnectionStringKey).Result;
            var appDbConnectionString = storageAdapter.GetObjectAsync(deployBucket, appDbConnectionStringKey).Result;

            services.AddNpgLoggerScoped(loggingDbConnectionString, LogLevel.All);

            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<RemindMeDatabaseContext>(options =>
                    options.UseNpgsql(appDbConnectionString));
        }
    }
}