using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using KerryShaleFanPage.Server.Interfaces.Security;
using KerryShaleFanPage.Shared.Security;

namespace KerryShaleFanPage.Server.Services.Security
{
    public class SecurityService : ISecurityService
    {
        private readonly ILogger<SecurityService> _logger;  // TODO: Implement logging!
        private readonly SecurityProvider _securityProvider;

        public X509Certificate2? Certificate => GetCertificate();

        /// <summary>
        /// 
        /// </summary>
        public SecurityService(ILogger<SecurityService> logger, SecurityProvider securityProvider) 
        {
            _logger = logger;
            _securityProvider = securityProvider;
        }

        /// <inheritdoc cref="ISecurityService"/>
        public string EncryptData(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) 
            {
                return plainText;
            }

            var dataBytes = _securityProvider.ToByteArray(plainText);
            var encryptedData = _securityProvider.EncryptDataSha2(Certificate, dataBytes);
            var encryptedDataBase64 = _securityProvider.ToBase64(encryptedData);
            return _securityProvider.ToHexString(encryptedDataBase64);
        }

        /// <inheritdoc cref="ISecurityService"/>
        public string DecryptData(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return encryptedText;
            }

            var dataBytes = _securityProvider.FromHexString(encryptedText);
            var encryptedDataBase64 = _securityProvider.FromBase64(dataBytes);
            var decryptedData = _securityProvider.DecryptDataSha2(Certificate, encryptedDataBase64);
            return _securityProvider.ToString(decryptedData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static X509Certificate2? GetCertificate()
        {
            var basePath = AppContext.BaseDirectory;
            var certPemPath = Path.Combine(basePath, "Certificate/cert.pem");  // TODO: Make configurable!
            var eccPemPath = Path.Combine(basePath, "Certificate/key.pem");  // TODO: Make configurable!
            if (File.Exists(certPemPath))
            {
                var certPem = File.ReadAllText(certPemPath);
                if (File.Exists(eccPemPath))
                {
                    var eccPem = File.ReadAllText(eccPemPath);
                    return X509Certificate2.CreateFromPem(certPem, eccPem);
                }
            }
            return null;
        }
    }
}
