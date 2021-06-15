using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Pannotation.Common.Constants
{
    public class AuthOptions
    {
        public const string ISSUER = "PannotationAuthServer";
        public const string AUDIENCE = "Client";
        const string KEY = "1rsfje36ZtLDpEdNgc8H0lY8uDxSN5W42oB2qzaMZZkFyjnmtyDzbIqxRVvHsw7GIqNUUqYsyS2IkLVT6NH3JQ==";
        public const int ACCESS_TOKEN_LIFETIME = 14;
        public const int REFRESH_TOKEN_LIFETIME = 30;

        public static SigningCredentials GetSigningCredentials()
        {
            var hmac = new HMACSHA256(Convert.FromBase64String(KEY));
            var signingCredentials = new SigningCredentials(
             new SymmetricSecurityKey(hmac.Key), SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            return signingCredentials;
        }

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}