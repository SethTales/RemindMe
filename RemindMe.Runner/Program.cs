using System;
using RemindMe.Data;
using RemindMeRunner.App.Services;
using System.IO;
using Amazon.SimpleNotificationService;
using Amazon;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RemindMe.Shared;
using Log4Npg;

namespace RemindMeRunner.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json")
                .AddEnvironmentVariables();
            
            var configuration = builder.Build();
            var services = new ServiceCollection();
            ConfigurationHelper.ConfigureSharedServices(configuration, services);
            services.AddScoped<IAmazonSimpleNotificationService>(s => new AmazonSimpleNotificationServiceClient(RegionEndpoint.USWest2));
            services.AddScoped<ISenderService, SenderService>();
            var provider = services.BuildServiceProvider();

            var reminderRepository = provider.GetRequiredService<IReminderRepository>();
            var senderService = provider.GetRequiredService<ISenderService>();
            var logger = provider.GetRequiredService<INpgLogger>();

            try
            {
                var reminderId = Int64.Parse(args[0]);

                var reminder = reminderRepository.GetReminderById(reminderId);
                senderService.SendReminder(reminder);
                reminderRepository.UpdateReminderLastRunTime(reminderId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
            }
                     
        }
    }
}
