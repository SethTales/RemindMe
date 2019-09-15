using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Log4Npg;
using RemindMe.Api;
using RemindMe.Adapters;
using RemindMe.Data;
using RemindMe.Api.Controllers;
using RemindMe.Adapters.Helpers;
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
        private static IAuthAdapter _mockAuthAdapter;
        private static IAwsCognitoAdapterHelper _mockCognitoHelper;
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
                .AddDbContext<TestDatabaseContext>(options =>
                    options.UseNpgsql(_connectionString));

            var mockLogger = Substitute.For<INpgLogger>();
            services.AddSingleton(mockLogger);
            _mockLogger = mockLogger;

            var mockAuthAdapter = Substitute.For<IAuthAdapter>();
            services.AddSingleton(mockAuthAdapter);
            _mockAuthAdapter = mockAuthAdapter;

            var mockCognitoHelper = Substitute.For<IAwsCognitoAdapterHelper>();
            services.AddSingleton(mockCognitoHelper);
            _mockCognitoHelper = mockCognitoHelper;

            services.AddMvc()
                .AddApplicationPart(typeof(AccountsController).Assembly);

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

        public static IAuthAdapter GetMockAuthAdapter()
        {
            return _mockAuthAdapter;
        }

        public static IAwsCognitoAdapterHelper GetMockAuthHelper()
        {
            return _mockCognitoHelper;
        }


        public static string GetConnectionString() => _connectionString;
    }
}