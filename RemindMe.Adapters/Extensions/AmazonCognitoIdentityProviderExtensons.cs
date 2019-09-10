using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace RemindMe.Adapters.Extensions
{
    public static class AmazonCognitoIdentityProviderExtensions
    {
        public static async Task<UserType> LookupUserByEmailAsync(this IAmazonCognitoIdentityProvider cognitoIdentityProvider, string userPoolId, string userName)
        {
            var listUsersRequest = new ListUsersRequest()
            {
                AttributesToGet = new List<string>() {"sub"},
                Filter = $"email=\"{userName}\"",
                UserPoolId = userPoolId               
            };
            var listUsersResponse = await cognitoIdentityProvider.ListUsersAsync(listUsersRequest);
            listUsersResponse.Users.FirstOrDefault();
            var user = listUsersResponse.Users.FirstOrDefault();
            return user;
        }
    }
}