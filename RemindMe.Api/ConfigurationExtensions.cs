using Microsoft.Extensions.Configuration;

namespace RemindMe.Api.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetValidIssuer(this IConfigurationRoot Configuration, string userPoolId)
        {
            return Configuration.GetSection("JWT")["ISSUER"].Replace("{USERPOOL_ID}", userPoolId);
        }
    }
}