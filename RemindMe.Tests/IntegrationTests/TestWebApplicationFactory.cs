using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Log4Npg;
using RemindMe.Api;
using RemindMe.Adapters;
using RemindMe.Data;
using Amazon.CognitoIdentityProvider;
using NSubstitute;
using System;

namespace RemindMe.Tests.IntegrationTests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return Program.CreateWebHostBuilder(new string[0])
                .UseStartup<TestStartup>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
        }
        
        public IServiceProvider GetTestServiceProvider()
        {
            return TestStartup.GetServiceProvider();
        }

        public RemindMeDatabaseContext GetRemindMeDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<RemindMeDatabaseContext>()
                .UseNpgsql(TestStartup.GetConnectionString());
            return new RemindMeDatabaseContext(options.Options);
        }
    }

    internal class TestStartup : Startup
    {
        private static IServiceCollection _services;
        private static INpgLogger _mockLogger;
        private static IAmazonCognitoIdentityProvider _mockCognitoClient;
        private static string _connectionString;
        public TestStartup(IHostingEnvironment env) : base(env) {}

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            var deployBucket = Configuration.GetSection("AWS").GetSection("S3")["DeployBucket"];
            var testDbConnectionStringKey = Configuration.GetSection("AWS").GetSection("RDS")["AppDbConnectionStringKey"];

            var provider = services.BuildServiceProvider();
            var storageAdapter = provider.GetRequiredService<IStorageAdapter>();

            _connectionString = storageAdapter.GetObjectAsync(deployBucket, testDbConnectionStringKey).Result;
            services
                .AddEntityFrameworkNpgsql()
                .AddDbContext<RemindMeDatabaseContext>(options =>
                    options.UseNpgsql(_connectionString));

            var mockLogger = Substitute.For<INpgLogger>();
            services.AddSingleton(mockLogger);
            _mockLogger = mockLogger;

            var mockCognitoClient = Substitute.For<IAmazonCognitoIdentityProvider>();
            services.AddSingleton(mockCognitoClient);
            _mockCognitoClient = mockCognitoClient;

            _services = services;
        }

        public static IServiceProvider GetServiceProvider()
        {
            return _services.BuildServiceProvider();
        }

        public static INpgLogger GetTestLogger()
        {
            return _mockLogger;
        }

        public static IAmazonCognitoIdentityProvider GetMockCognitoClient()
        {
            return _mockCognitoClient;
        }

        public static string GetConnectionString() => _connectionString;
    }
}