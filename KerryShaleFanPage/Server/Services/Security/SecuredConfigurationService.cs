using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using Newtonsoft.Json;
using KerryShaleFanPage.Server.Interfaces.Security;
using KerryShaleFanPage.Shared.Configuration;

namespace KerryShaleFanPage.Server.Services.Security
{
    public class SecuredConfigurationService : ISecuredConfigurationService
    {
        private readonly ISecurityService _securityService;
        private readonly IOptions<GeneralSettings> _cachedSettings;
        private readonly IOptionsMonitor<GeneralSettings> _currentSettings;
        private readonly IOptions<AppSettings> _cachedAppSettings;
        private readonly IOptionsMonitor<AppSettings> _currentAppSettings;
        private readonly IOptions<NewsSettings> _cachedNewsSettings;
        private readonly IOptionsMonitor<NewsSettings> _currentNewsSettings;
        private readonly IOptions<GallerySettings> _cachedGallerySettings;
        private readonly IOptionsMonitor<GallerySettings> _currentGallerySettings;

        private readonly ILogger<SecuredConfigurationService> _logger;  // TODO: Implement logging!

        /// <summary>
        /// 
        /// </summary>
        public SecuredConfigurationService(ILogger<SecuredConfigurationService> logger, ISecurityService securityService, IOptions<GeneralSettings> cachedSettings, 
            IOptionsMonitor<GeneralSettings> currentSettings, IOptions<AppSettings> cachedAppSettings, IOptionsMonitor<AppSettings> currentAppSettings, 
            IOptions<NewsSettings> cachedNewsSettings, IOptionsMonitor<NewsSettings> currentNewsSettings, IOptions<GallerySettings> cachedGallerySettings, 
            IOptionsMonitor<GallerySettings> currentGallerySettings) 
        { 
            _logger = logger;
            _securityService = securityService;
            _cachedSettings = cachedSettings;
            _currentSettings = currentSettings;
            _cachedAppSettings = cachedAppSettings;
            _currentAppSettings = currentAppSettings;
            _cachedNewsSettings = cachedNewsSettings;
            _currentNewsSettings = currentNewsSettings;
            _cachedGallerySettings = cachedGallerySettings;
            _currentGallerySettings = currentGallerySettings;
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
        public NewsSettings GetCachedNewsSettingsConfigurationFromFile()
        {
            return _cachedNewsSettings.Value;
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public NewsSettings GetCurrentNewsSettingsConfigurationFromFile()
        {
            return _currentNewsSettings.CurrentValue;
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public GallerySettings GetCachedGallerySettingsConfigurationFromFile()
        {
            return _cachedGallerySettings.Value;
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public GallerySettings GetCurrentGallerySettingsConfigurationFromFile()
        {
            return _currentGallerySettings.CurrentValue;
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public bool GetEncryptedConfigurationForEncryptedFileFromSettings()
        {
            var basePath = AppContext.BaseDirectory;
            var encryptedFilePath = Path.Combine(basePath, "Security/generalsettings.conf");
            var currentSettings = GetCurrentSettingsConfigurationFromFile();
            var serializedCurrentSettings = JsonConvert.SerializeObject(currentSettings);
            return _securityService.EncryptFile(encryptedFilePath, serializedCurrentSettings, true);
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public GeneralSettings GetDecryptedConfigurationForSettingsFromEncryptedFile()
        {
            var basePath = AppContext.BaseDirectory;
            var encryptedFilePath = Path.Combine(basePath, "Security/generalsettings.conf");
            var serializedCurrentSettings = _securityService.DecryptFile(encryptedFilePath);
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
