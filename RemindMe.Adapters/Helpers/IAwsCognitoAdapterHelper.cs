using System.Threading.Tasks;
using RemindMe.Models;
using Amazon.CognitoIdentityProvider.Model;

namespace RemindMe.Adapters.Helpers
{
    public interface IAwsCognitoAdapterHelper
    {
        Task<bool> UserExists(AwsCognitoUser user);
        Task<bool> UserIsConfirmed(AwsCognitoUser user);
    }
}