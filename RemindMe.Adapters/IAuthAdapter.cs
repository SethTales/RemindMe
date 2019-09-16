using System;
using System.Threading.Tasks;
using System.Net.Http;
using RemindMe.Models;

namespace RemindMe.Adapters
{
    public interface IAuthAdapter
    {
        Task<HttpResponseMessage> RegisterNewUserAsync(AwsCognitoUser user);
        Task<HttpResponseMessage> ConfirmUserAsync(AwsCognitoUser user);
        Task<HttpResponseMessage> AuthenticateUserAsync(AwsCognitoUser user);
    }
}
