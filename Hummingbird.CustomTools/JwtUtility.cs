using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hummingbird.CustomTools
{
    /// <summary>
    /// https://www.codeproject.com/Tips/1208535/Create-and-Consume-JWT-Tokens-in-Csharp
    /// </summary>
    public class JwtUtility
    {

        internal static Dictionary<string, string> JwtHeaderDescription = new Dictionary<string, string>()
        {
            { "alg", "Signature or encryption algorithm" },
            { "typ", "Token type. if present, it is recommended to set to 'JWT'" },
            { "cty", "Content type. If nested signing or encryption is employed, it is recommended to set this to JWT, otherwise omit this field" },
        };

        internal static Dictionary<string, string> JwtSignatureAlgorithms = new Dictionary<string, string>()
        {
            //{ SecurityAlgorithms.None, "None" },
            { SecurityAlgorithms.HmacSha256,    "HMAC-SHA256" },
            { SecurityAlgorithms.HmacSha384,    "HMAC-SHA384" },
            { SecurityAlgorithms.HmacSha512,    "HMAC-SHA512" },
            //{ SecurityAlgorithms.RsaSha256,     "RSA-SHA256" },
            //{ SecurityAlgorithms.RsaSha384,     "RSA-SHA384" },
            //{ SecurityAlgorithms.RsaSha512,     "RSA-SHA512" },
            //{ SecurityAlgorithms.EcdsaSha256,   "ECDSA-SHA256" },
            //{ SecurityAlgorithms.EcdsaSha384,   "ECDSA-SHA384" },
            //{ SecurityAlgorithms.EcdsaSha512,   "ECDSA-SHA512" },
            //{ SecurityAlgorithms.RsaSsaPssSha256,   "RSASSA-PSS-SHA256" },
            //{ SecurityAlgorithms.RsaSsaPssSha384,   "RSASSA-PSS-SHA384" },
            //{ SecurityAlgorithms.RsaSsaPssSha512,   "RSASSA-PSS-SHA512" },
            //{ SecurityAlgorithms.RsaOAEP, "RSA-OAEP" },
        };

        internal static Dictionary<string, string> JwtPayloadDescription = new Dictionary<string, string>()
        {
            { "iss", "Issuer - Identifies principal that issued the JWT. " },
            { "sub", "Subject - Identifies the subject of the JWT. " },
            { "aud", "Audience - Identifies the recipients that the JWT is intended for. Each principal intended to process the JWT must identify itself with a value in the audience claim. If the principal processing the claim does not identify itself with a value in the aud claim when this claim is present, then the JWT must be rejected." },
            { "exp", "Expiration Time - Identifies the expiration time on and after which the JWT must not be accepted for processing. The value must be a NumericDate[10]: either an integer or decimal, representing seconds past 1970-01-01 00:00:00Z. " },
            { "nbf", "Not Before - Identifies the time on which the JWT will start to be accepted for processing. The value must be a NumericDate. " },
            { "iat", "Issued at - Identifies the time at which the JWT was issued. The value must be a NumericDate. " },
            { "jti", "JWT ID - Case sensitive unique identifier of the token even among different issuers. " },
        };

        /// <summary>
        /// Creates a Json Web Token (JWT) by given parameters (using symmetric algorithms)
        /// </summary>
        /// <param name="base64Key">The base64url encoded secret. The key size must match selected <paramref name="algorithm" />, for example with HS256, the key size must &gt;= 256 bits.</param>
        /// <param name="algorithm">The signature or encryption algorithm, choose one from <see cref="SecurityAlgorithms" />.</param>
        /// <param name="payload">The Jwt payload.</param>
        /// <param name="token">The string representation of the web token that can be used by the client.</param>
        /// <returns>A Json Web Token generated with given parameters</returns>
        /// <remarks>
        /// According to the section 4 & 5 of RFC 4648, the Url Base64 encoding method used in JWT signature is different from regular Base 64 Encoding.
        /// </remarks>
        public static JwtSecurityToken CreateToken(string base64Key, string algorithm, JwtPayload payload, out string token)
        {
            
            byte[] key = Base64UrlEncoder.DecodeBytes(base64Key);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, algorithm);
            var jwtHeader = new JwtHeader(credentials);
            var secToken = new JwtSecurityToken(jwtHeader, payload);
            var handler = new JwtSecurityTokenHandler();
            token = handler.WriteToken(secToken);
            return secToken;
        }

    }
}
