using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;

namespace KerryShaleFanPage.Shared.Security
{
    public class SecurityProvider
    {
        private readonly ILogger<SecurityProvider> _logger;  // TODO: Implement logging!

        /// <summary>
        /// 
        /// </summary>
        public SecurityProvider(ILogger<SecurityProvider> logger) 
        { 
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cert"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] EncryptDataSha2(X509Certificate2? cert, byte[]? data)
        {
            if (cert == null || data == null)
            {
                return Array.Empty<byte>();
            }

            using var rsa = cert.GetRSAPublicKey();
            return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA512);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cert"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] DecryptDataSha2(X509Certificate2? cert, byte[]? data)
        {
            if (cert == null || data == null)
            {
                return Array.Empty<byte>();
            }

            using var rsa = cert.GetRSAPrivateKey();
            return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA512);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public byte[] CreateSalt(int size = 64)
        {
            if (size < 32)
            {
                size = 64;
            }

            byte[] salt;
            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(salt = new byte[size]);
            return salt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static byte[] HashDataSha2(string? data, byte[]? salt)
        {
            if (string.IsNullOrEmpty(data) || salt == null)
            {
                return Array.Empty<byte>();
            }

            return HashDataSha2(Encoding.UTF8.GetBytes(data), salt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static byte[] HashDataSha2(byte[]? dataBytes, byte[]? salt)
        {
            if (dataBytes == null || salt == null)
            {
                return Array.Empty<byte>();
            }

            var saltedValue = dataBytes.Concat(salt).ToArray();
            return new SHA512Managed().ComputeHash(saltedValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public bool ConfirmPassword(string password, byte[]? salt)
        {
            if (string.IsNullOrEmpty(password) || salt == null)
            {
                return false;
            }

            var passwordHash = HashDataSha2(password, salt);
            return passwordHash.SequenceEqual(passwordHash);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string ToBase64(byte[]? bytes)
        {
            if (bytes == null)
            {
                return string.Empty;
            }

            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public byte[] FromBase64(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData))
            {
                return Array.Empty<byte>();
            }

            return Convert.FromBase64String(base64EncodedData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string ToBase64String(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            var plainTextBytes = ToByteArray(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public string FromBase64String(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData))
            {
                return base64EncodedData;
            }

            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return ToString(base64EncodedBytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string ToHexString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            var sb = new StringBuilder();

            var bytes = ToByteArray(str);
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public string FromHexString(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
            {
                return hexString;
            }

            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return ToString(bytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public byte[] ToByteArray(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Array.Empty<byte>();
            }

            return Encoding.UTF8.GetBytes(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public string ToString(byte[]? bytes)
        {
            if (bytes == null)
            {
                return string.Empty;
            }

            return Encoding.UTF8.GetString(bytes);
        }
    }
}
