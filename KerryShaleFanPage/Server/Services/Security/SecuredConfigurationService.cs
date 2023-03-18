using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using Newtonsoft.Json;
using KerryShaleFanPage.Server.Interfaces.Security;
using KerryShaleFanPage.Shared.Configuration;

namespace KerryShaleFanPage.Server.Services.Security
{
    /// <summary>
    /// Remarks: This class uses Reflection! Reflection is very gerneric and can do magical things, but it costs performance.
    /// </summary>

    public class SecuredConfigurationService : ISecuredConfigurationService
    {
        private readonly ISecurityService _securityService;
        private readonly IOptions<GeneralSettings> _cachedSettings;
        private readonly IOptionsMonitor<GeneralSettings> _currentSettings;
        private readonly IOptions<AppSettings> _cachedAppSettings;
        private readonly IOptionsMonitor<AppSettings> _currentAppSettings;

        private readonly ILogger<SecuredConfigurationService> _logger;  // TODO: Implement logging!

        /// <summary>
        /// 
        /// </summary>
        public SecuredConfigurationService(ILogger<SecuredConfigurationService> logger, ISecurityService securityService, IOptions<GeneralSettings> cachedSettings, 
            IOptionsMonitor<GeneralSettings> currentSettings, IOptions<AppSettings> cachedAppSettings, IOptionsMonitor<AppSettings> currentAppSettings) 
        { 
            _logger = logger;
            _securityService = securityService;
            _cachedSettings = cachedSettings;
            _currentSettings = currentSettings;
            _cachedAppSettings = cachedAppSettings;
            _currentAppSettings = currentAppSettings;
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public GeneralSettings GetCachedSettingsConfigurationFromFile()
        {
            return _cachedSettings.Value;
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public GeneralSettings GetCurrentSettingsConfigurationFromFile()
        {
            return _currentSettings.CurrentValue;
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public AppSettings GetCachedAppSettingsConfigurationFromFile()
        {
            return _cachedAppSettings.Value;
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public AppSettings GetCurrentAppSettingsConfigurationFromFile()
        {
            return _currentAppSettings.CurrentValue;
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public bool GetEncryptedConfigurationForEncryptedFileFromSettings()
        {
            var basePath = AppContext.BaseDirectory;
            var encryptedFilePath = Path.Combine(basePath, "Certificate/generalsettings.conf");  // TODO: Make configurable!
            var currentSettings = GetCurrentSettingsConfigurationFromFile();
            var serializedCurrentSettings = JsonConvert.SerializeObject(currentSettings);
            return _securityService.EncryptDataAndFile(encryptedFilePath, serializedCurrentSettings, true);  // TODO: Make configurable!
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public GeneralSettings GetDecryptedConfigurationForSettingsFromEncryptedFile()
        {
            var basePath = AppContext.BaseDirectory;
            var encryptedFilePath = Path.Combine(basePath, "Certificate/generalsettings.conf");  // TODO: Make configurable!
            var serializedCurrentSettings = _securityService.DecryptFileAndData(encryptedFilePath);
            if (serializedCurrentSettings == null)
            {
                return GetCurrentSettingsConfigurationFromFile();  // Fallback
            }
            var currentSettings = JsonConvert.DeserializeObject<GeneralSettings>(serializedCurrentSettings);
            if (currentSettings == null)
            {
                return GetCurrentSettingsConfigurationFromFile();  // Fallback
            }
            return currentSettings;
        }
    }
}
