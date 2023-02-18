using Microsoft.Extensions.Logging;
using KerryShaleFanPage.Server.Interfaces.Security;
using KerryShaleFanPage.Shared.Security;
using System.Security.Cryptography.X509Certificates;

namespace KerryShaleFanPage.Server.Services.Security
{
    public class SecurityService : ISecurityService
    {
        private readonly ILogger<SecurityService> _logger;  // TODO: Implement logging!
        private readonly SecurityProvider _securityProvider;

        private X509Certificate2 Certficate => GetCertificate();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="securityProvider"></param>
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
            var encryptedData = _securityProvider.EncryptDataSha2(Certficate, dataBytes);
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
            var decryptedData = _securityProvider.DecryptDataSha2(Certficate, encryptedDataBase64);
            return _securityProvider.ToString(decryptedData);
        }

        private X509Certificate2 GetCertificate()
        {
            return Certficate;
        }
    }
}
