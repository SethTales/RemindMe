using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.CognitoIdentityProvider;
using RemindMe.Adapters;
using Amazon.S3;
using Amazon;
using Newtonsoft.Json;
using RemindMe.Adapters.Helpers;
using RemindMe.Data;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using RemindMe.Api.Extensions;
using System.Net;
using RemindMe.Shared;

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
        public virtual void ConfigureServices(IServiceCollection services)
        {
            ConfigurationHelper.ConfigureSharedServices(Configuration, services);
            
            var deployBucket = Configuration.GetSection("AWS").GetSection("S3")["DeployBucket"];
            var cognitoConfigKey = Configuration.GetSection("AWS").GetSection("S3")["CognitoConfigurationKey"];
            
            var s3Client = new AmazonS3Client(RegionEndpoint.USWest2);
            var storageAdapter = new AwsS3Adapter(s3Client);

            var cognitoAdapterConfig = JsonConvert.DeserializeObject<AwsCognitoAdapterConfig>(storageAdapter.GetObjectAsync(deployBucket, cognitoConfigKey).Result);
            var validIssuer = Configuration.GetValidIssuer(cognitoAdapterConfig.UserPoolId);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                //options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => 
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context => {
                            context.Token = context.Request.Cookies["IdToken"];
                            return Task.CompletedTask;
                        }
                    };

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKeyResolver = (s, token, identifier, parameters) => 
                        {
                            var json = new WebClient().DownloadString($"{validIssuer}/.well-known/jwks.json");
                            var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;
                            return (IEnumerable<SecurityKey>)keys;
                        },
                        ClockSkew = TimeSpan.FromMinutes(5),
                        LifetimeValidator = (notBefore, expires, token, parameters) =>
                        {
                            if (expires != null)
                            {
                                //return false;
                                if (DateTime.UtcNow < expires) return true;
                            }
                            return false;
                        },
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = validIssuer
                    };
                });

            services.AddScoped<AwsCognitoAdapterConfig>(s => cognitoAdapterConfig);
            services.AddScoped<IAwsCognitoAdapterHelper, AwsCognitoAdapterHelper>();
            services.AddScoped<IAmazonCognitoIdentityProvider, AmazonCognitoIdentityProviderClient>();
            services.AddScoped<IAuthAdapter, AwsCognitoAdapter>();

            services.AddScoped<IUserRepository, UserRepository>();
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
            app.UseAuthentication();

            app.UseStatusCodePages(async context => {
                var response = context.HttpContext.Response;
                if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    response.Redirect("/accounts/users/authenticate");
                }
            });

            app.UseMvc(routes => 
            {
                routes
                    .MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}");
            });
        }
    }
}
