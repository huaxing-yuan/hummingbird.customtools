using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Hummingbird.CustomTools
{
    /// <summary>
    /// https://www.codeproject.com/Tips/1208535/Create-and-Consume-JWT-Tokens-in-Csharp
    /// </summary>
    internal static class JwtUtility
    {


        internal static readonly Dictionary<string, string> JwtSignatureAlgorithms = new Dictionary<string, string>()
        {
            //{ SecurityAlgorithms.None, "None" },
            { SecurityAlgorithms.HmacSha256,    "HMAC-SHA256" },
            { SecurityAlgorithms.HmacSha384,    "HMAC-SHA384" },
            { SecurityAlgorithms.HmacSha512,    "HMAC-SHA512" },
            { SecurityAlgorithms.RsaSha256,     "RSA-SHA256" },
            { SecurityAlgorithms.RsaSha384,     "RSA-SHA384" },
            { SecurityAlgorithms.RsaSha512,     "RSA-SHA512" },
            { SecurityAlgorithms.EcdsaSha256,   "ECDSA-SHA256" },
            { SecurityAlgorithms.EcdsaSha384,   "ECDSA-SHA384" },
            { SecurityAlgorithms.EcdsaSha512,   "ECDSA-SHA512" },
            //{ SecurityAlgorithms.RsaSsaPssSha256,   "RSASSA-PSS-SHA256" }, //not supported yet
            //{ SecurityAlgorithms.RsaSsaPssSha384,   "RSASSA-PSS-SHA384" },
            //{ SecurityAlgorithms.RsaSsaPssSha512,   "RSASSA-PSS-SHA512" },
            //{ SecurityAlgorithms.RsaOAEP, "RSA-OAEP" },
        };

    }
}
