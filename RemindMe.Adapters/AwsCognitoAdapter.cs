using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using RemindMe.Models;
using RemindMe.Adapters.Helpers;

namespace RemindMe.Adapters
{
    public class AwsCognitoAdapter : IAuthAdapter
    {
        private readonly IAmazonCognitoIdentityProvider _awsCognitoClient;
        private readonly IAwsCognitoAdapterHelper _cognitoAdapterHelper;
        private readonly string _clientId;

        public AwsCognitoAdapter(IAmazonCognitoIdentityProvider awsCognitoClient, AwsCognitoAdapterConfig cognitoConfig)
        {
            _awsCognitoClient = awsCognitoClient;
            _clientId = cognitoConfig.ClientId;
        }

        public async Task<HttpResponseMessage> RegisterNewUserAsync(RemindMeUser user)
        {
            if (await _cognitoAdapterHelper.UserExists(user))
            {
                return new HttpResponseMessage(HttpStatusCode.Conflict);
            }

            var signUpRequest = new SignUpRequest()
            {
                Username = user.UserName,
                Password = user.Password,
                ClientId = _clientId
            };
            var signUpResponse = await _awsCognitoClient.SignUpAsync(signUpRequest);

            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}