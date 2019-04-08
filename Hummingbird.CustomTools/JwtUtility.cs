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
            { SecurityAlgorithms.RsaSha256,     "RSA-SHA256" },
            { SecurityAlgorithms.RsaSha384,     "RSA-SHA384" },
            { SecurityAlgorithms.RsaSha512,     "RSA-SHA512" },
            { SecurityAlgorithms.EcdsaSha256,   "ECDSA-SHA256" },
            { SecurityAlgorithms.EcdsaSha384,   "ECDSA-SHA384" },
            { SecurityAlgorithms.EcdsaSha512,   "ECDSA-SHA512" },
            { SecurityAlgorithms.RsaSsaPssSha256,   "RSASSA-PSS-SHA256" },
            { SecurityAlgorithms.RsaSsaPssSha384,   "RSASSA-PSS-SHA384" },
            { SecurityAlgorithms.RsaSsaPssSha512,   "RSASSA-PSS-SHA512" },
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
        /// Creates a Json Web Token (JWT) by given parameters (using symmetric algorithm HMAC-SHA) 
        /// </summary>
        /// <param name="base64Key">The base64url encoded secret. The key size must match selected <paramref name="algorithm" />, for example with HS256, the key size must &gt;= 256 bits.</param>
        /// <param name="algorithm">The signature or encryption algorithm, choose one from <see cref="SecurityAlgorithms" />.</param>
        /// <param name="payload">The Jwt payload.</param>
        /// <param name="token">The string representation of the web token that can be used by the client.</param>
        /// <returns>A Json Web Token generated with given parameters</returns>
        /// <remarks>
        /// According to the section 4 & 5 of RFC 4648, the Url Base64 encoding method used in JWT signature is different from regular Base 64 Encoding.
        /// </remarks>
        public static JwtSecurityToken CreateHmacShaToken(string base64Key, string algorithm, JwtPayload payload, out string token)
        {
            
            byte[] key = Base64UrlEncoder.DecodeBytes(base64Key);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key);
            var secToken = CreateSecurityToken(securityKey, algorithm, payload, out token);
            return secToken;
        }

        public static JwtSecurityToken CreateRsaToken(X509Certificate2 pkcs12Certificate, string algorithm, JwtPayload payload, out string token)
        {
            var securityKey = new X509SecurityKey(pkcs12Certificate);
            var secToken = CreateSecurityToken(securityKey, algorithm, payload, out token);
            return secToken;
        }

        public static JwtSecurityToken CreateEcdsaToken(X509Certificate2 x509Certificate, string algorithm, JwtPayload payload, out string token)
        {
            var ecdKey = x509Certificate.GetECDsaPrivateKey();
            var securityKey = new ECDsaSecurityKey(ecdKey);
            var secToken = CreateSecurityToken(securityKey, algorithm, payload, out token);
            return secToken;
        }


        public static string ExportPrivateKey(RSACryptoServiceProvider csp)
        {
            StringWriter outputStream = new StringWriter();
            if (csp.PublicOnly) throw new ArgumentException("CSP does not contain a private key", "csp");
            var parameters = csp.ExportParameters(true);
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
                    EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                    EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                    EncodeIntegerBigEndian(innerWriter, parameters.D);
                    EncodeIntegerBigEndian(innerWriter, parameters.P);
                    EncodeIntegerBigEndian(innerWriter, parameters.Q);
                    EncodeIntegerBigEndian(innerWriter, parameters.DP);
                    EncodeIntegerBigEndian(innerWriter, parameters.DQ);
                    EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                // WriteLine terminates with \r\n, we want only \n
                outputStream.Write("-----BEGIN RSA PRIVATE KEY-----\n");
                // Output as Base64 with lines cropped at 64 characters
                for (var i = 0; i < base64.Length; i += 64)
                {
                    outputStream.Write(base64, i, Math.Min(64, base64.Length - i));
                    outputStream.Write("\n");
                }
                outputStream.Write("-----END RSA PRIVATE KEY-----");
            }

            return outputStream.ToString();
        }

        public static string ExportPublicKey(RSACryptoServiceProvider csp)
        {
            StringWriter outputStream = new StringWriter();
            var parameters = csp.ExportParameters(false);
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    innerWriter.Write((byte)0x30); // SEQUENCE
                    EncodeLength(innerWriter, 13);
                    innerWriter.Write((byte)0x06); // OBJECT IDENTIFIER
                    var rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
                    EncodeLength(innerWriter, rsaEncryptionOid.Length);
                    innerWriter.Write(rsaEncryptionOid);
                    innerWriter.Write((byte)0x05); // NULL
                    EncodeLength(innerWriter, 0);
                    innerWriter.Write((byte)0x03); // BIT STRING
                    using (var bitStringStream = new MemoryStream())
                    {
                        var bitStringWriter = new BinaryWriter(bitStringStream);
                        bitStringWriter.Write((byte)0x00); // # of unused bits
                        bitStringWriter.Write((byte)0x30); // SEQUENCE
                        using (var paramsStream = new MemoryStream())
                        {
                            var paramsWriter = new BinaryWriter(paramsStream);
                            EncodeIntegerBigEndian(paramsWriter, parameters.Modulus); // Modulus
                            EncodeIntegerBigEndian(paramsWriter, parameters.Exponent); // Exponent
                            var paramsLength = (int)paramsStream.Length;
                            EncodeLength(bitStringWriter, paramsLength);
                            bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
                        }
                        var bitStringLength = (int)bitStringStream.Length;
                        EncodeLength(innerWriter, bitStringLength);
                        innerWriter.Write(bitStringStream.GetBuffer(), 0, bitStringLength);
                    }
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                // WriteLine terminates with \r\n, we want only \n
                outputStream.Write("-----BEGIN PUBLIC KEY-----\n");
                for (var i = 0; i < base64.Length; i += 64)
                {
                    outputStream.Write(base64, i, Math.Min(64, base64.Length - i));
                    outputStream.Write("\n");
                }
                outputStream.Write("-----END PUBLIC KEY-----");
            }

            return outputStream.ToString();
        }

        /// <summary>
        /// https://stackoverflow.com/a/23739932/2860309
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        /// <param name="forceUnsigned"></param>
        private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    // Add a prefix zero to force unsigned if the MSB is 1
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }

        /// <summary>
        /// https://stackoverflow.com/a/23739932/2860309
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        private static void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
            if (length < 0x80)
            {
                // Short form
                stream.Write((byte)length);
            }
            else
            {
                // Long form
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }


        public static JwtSecurityToken CreateSecurityToken(SecurityKey securityKey, string algorithm, JwtPayload payload, out string token)
        {
            var credentials = new SigningCredentials(securityKey, algorithm);
            var jwtHeader = new JwtHeader(credentials);
            var secToken = new JwtSecurityToken(jwtHeader, payload);
            var handler = new JwtSecurityTokenHandler();
            token = handler.WriteToken(secToken);
            return secToken;
        }

        internal static RSA GetPrivateKeyFromCertificate(string pfxFilePath, string pfxFilePassword)
        {
            X509Certificate2 cert = new X509Certificate2(pfxFilePath, pfxFilePassword, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            return GetPrivateKeyFromCertificate(cert);

        }
        internal static RSA GetPrivateKeyFromCertificate(X509Certificate2 certificate)
        {
            if (certificate.HasPrivateKey)
            {
                var privateKey = certificate.GetRSAPrivateKey();
                return privateKey;
            }
            else
            {
                throw new SecurityTokenEncryptionKeyNotFoundException("The given PKCS certificate does not contain a valid private key");
            }
        }
    }
}
