using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using RemindMe.Models;
using RemindMe.Adapters.Extensions;

namespace RemindMe.Adapters
{
    public class AwsCognitoAdapter : IAuthAdapter
    {
        private readonly IAmazonCognitoIdentityProvider _awsCognitoClient;
        private readonly string _userPoolId;
        private readonly string _clientId;

        public AwsCognitoAdapter(IAmazonCognitoIdentityProvider awsCognitoClient, AwsCognitoAdapterConfig cognitoConfig)
        {
            _awsCognitoClient = awsCognitoClient;
            _userPoolId = cognitoConfig.UserPoolId;
            _clientId = cognitoConfig.ClientId;
        }

        public async Task<HttpResponseMessage> RegisterNewUserAsync(RemindMeUser user)
        {
            if (await UserExists(user))
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

        private async Task<bool> UserExists(RemindMeUser user)
        {
            return await _awsCognitoClient.LookupUserByEmailAsync(_userPoolId, user.UserName) != null;
        }

        private async Task<bool> UserIsConfirmed(RemindMeUser user)
        {
            var cognitoUser = await _awsCognitoClient.LookupUserByEmailAsync(_userPoolId, user.UserName);
            return cognitoUser.UserStatus == UserStatusType.CONFIRMED;
        }
    }
}