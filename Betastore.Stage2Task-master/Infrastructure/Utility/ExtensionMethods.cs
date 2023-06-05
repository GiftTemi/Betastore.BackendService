using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Infrastructure.Utility
{
    public static class ExtensionMethods
    {
        public static JwtSecurityToken ExtractToken(this string str)
        {
            var stream = str.Remove(0, 7);

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var token = jsonToken as JwtSecurityToken;
            //var jti = tokenS.Claims.First(claim => claim.Type == "Sub").Value;
            return token;
        }

        public static bool ValidateToken(this JwtSecurityToken accessToken, string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    throw new Exception("Invalid User Id");
                }
                if (accessToken == null)
                    throw new Exception("Invalid Token Credentials");
                var token = accessToken.Claims.FirstOrDefault(claim => claim.Type == "userid");
                if (token == null)
                    throw new Exception("Invalid Token Credentials. UserId not found");
                if (userId != token.Value)
                {
                    throw new Exception("Invalid Token Credentials");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool CheckAccessLevels(this JwtSecurityToken accessToken, string whichRole)
        {
            try
            {
                var role = accessToken.Claims.FirstOrDefault(claim => claim.Type == "role");
                if (role == null)
                {
                    throw new Exception("Access Denied. No role assigned");

                }
                if (!role.Value.ToLower().Contains(whichRole.ToLower()))
                {
                    throw new Exception("Access Denied");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
