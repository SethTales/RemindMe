using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.CognitoIdentityProvider;
using RemindMe.Adapters;
using Amazon.S3;
using Amazon;
using Newtonsoft.Json;
using Log4Npg.Logging;
using Log4Npg.Logging.Extensions;
using Log4Npg.Logging.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace RemindMe.Api
{
    public class Startup
    {

        public IConfigurationRoot Configuration { get; }
        private readonly string _environment;
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            _environment = hostingEnvironment.EnvironmentName;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json")
                .AddEnvironmentVariables();
            
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var deployBucket = Configuration.GetSection("AWS").GetSection("S3")["DeployBucket"];
            var cognitoConfigKey = Configuration.GetSection("AWS").GetSection("S3")["CognitoConfigurationKey"];
            var loggingDbConnectionStringKey = Configuration.GetSection("AWS").GetSection("RDS")["LoggingDbConnectionStringKey"];

            services.AddScoped<IAmazonS3, AmazonS3Client>(s => new AmazonS3Client(RegionEndpoint.USWest2));
            services.AddScoped<IStorageAdapter, AwsS3Adapter>();
            var provider = services.BuildServiceProvider();
            var storageAdapter = provider.GetRequiredService<IStorageAdapter>();

            var cognitoAdapterConfig = JsonConvert.DeserializeObject<AwsCognitoAdapterConfig>(storageAdapter.GetObjectAsync(deployBucket, cognitoConfigKey).Result);
            var loggingDbConnectionString = storageAdapter.GetObjectAsync(deployBucket, loggingDbConnectionStringKey).Result;

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddNpgLoggerScoped(loggingDbConnectionString);

            services.AddSingleton<AwsCognitoAdapterConfig>(s => cognitoAdapterConfig);
            services.AddScoped<IAmazonCognitoIdentityProvider, AmazonCognitoIdentityProviderClient>();
            services.AddScoped<IAuthAdapter, AwsCognitoAdapter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }
    }
}
