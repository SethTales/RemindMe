using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace RemindMe.Api.Controllers.Utils
{
    public static class TokenUtil
    {
        public static string GetUserNameFromToken(string encodedToken)
        {
            return DecodeToken(encodedToken).Claims.FirstOrDefault(c => c.Type == "email").Value;
        }

        private static JwtSecurityToken DecodeToken(string encodedToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(encodedToken);
            return decodedToken;
        }
    }
}