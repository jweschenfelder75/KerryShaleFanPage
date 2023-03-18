using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KerryShaleFanPage.Server.Interfaces.Repositories;
using KerryShaleFanPage.Server.Interfaces.Security;
using KerryShaleFanPage.Shared.Configuration;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Server.Services.Security
{
    /// <summary>
    /// Remarks: This class uses Reflection! Reflection is very gerneric and can do magical things, but it costs performance.
    /// </summary>

    public class SecuredConfigurationService : ISecuredConfigurationService
    {
        private readonly ISecurityService _securityService;
        private readonly IOptions<AppSettings> _cachedAppSettings;
        private readonly IOptionsMonitor<AppSettings> _currentAppSettings;
        private readonly IGenericRepositoryService<ConfigurationEntryDto> _configurationRepositoryService;

        private readonly ILogger<SecuredConfigurationService> _logger;  // TODO: Implement logging!

        /// <summary>
        /// 
        /// </summary>
        public SecuredConfigurationService(ILogger<SecuredConfigurationService> logger, ISecurityService securityService, IOptions<AppSettings> cachedAppSettings,
            IOptionsMonitor<AppSettings> currentAppSettings, IGenericRepositoryService<ConfigurationEntryDto> configurationRepositoryService) 
        { 
            _logger = logger;
            _securityService = securityService;
            _cachedAppSettings = cachedAppSettings;
            _currentAppSettings = currentAppSettings;
            _configurationRepositoryService = configurationRepositoryService;
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
        public AppSettings GetEncryptedConfigurationForAppSettingsFromAppSettings()
        {
            var currentAppSettings = GetCurrentAppSettingsConfigurationFromFile();
            var type = typeof(AppSettings);  // Reflection
            var properties = type.GetProperties();  // Reflection
            foreach (var property in properties)
            {
                var mainType = property.PropertyType;
                var mainValue = property.GetValue(currentAppSettings);  // Reflection

                var subProperties = mainType.GetProperties();  // Reflection
                foreach (var subProperty in subProperties)
                {
                    var subValue = subProperty.GetValue(mainValue);  // Reflection
                    if (subValue is string)
                    {
                        var subValueAsString = JsonConvert.SerializeObject(subValue);
                        var subEncryptedValue = _securityService.EncryptData(subValueAsString);
                        subProperty.SetValue(mainValue, subEncryptedValue);  // Reflection
                    }
                }
            }

            return currentAppSettings;
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public AppSettings GetEncryptedConfigurationForAppSettingsFromDatabase()
        {
            var target = new AppSettings();

            var configurations = _configurationRepositoryService.GetAll();

            foreach (var configuration in configurations)
            {
                var keys = configuration.Key?.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? new string[0];
                if (keys.Length == 2)
                {
                    var propertyToGet = target?.GetType().GetProperty(keys.First());  // Reflection
                    if (propertyToGet == null)
                    {
                        continue;
                    }

                    var subTarget = propertyToGet.GetValue(target);  // Reflection
                    var propertyToSet = subTarget?.GetType().GetProperty(keys.Last());  // Reflection
                    if (propertyToSet != null)
                    {
                        var valueType = Nullable.GetUnderlyingType(propertyToSet.PropertyType) ?? propertyToSet.PropertyType;
                        object? safeValue = (configuration.Value == null) ? null : Convert.ChangeType(configuration.Value, valueType);
                        propertyToSet.SetValue(subTarget, safeValue, null);  // Reflection
                    }
                }
                else
                {
                    // ToDo: Log that something else is not supported yet, we do only support 2 levels so far.
                }
            }

            return target;
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public async Task<IList<ConfigurationEntryDto>> GetEncryptedConfigurationForDatabaseFromAppSettingsAsync(CancellationToken cancellationToken = default)
        {
            var result = new List<ConfigurationEntryDto>();

            var type = typeof(AppSettings);  // Reflection
            var properties = type.GetProperties();  // Reflection
            foreach (var property in properties)
            {
                var mainKey = property.Name;
                var mainType = property.PropertyType;
                var mainTypeName = property.PropertyType.Name;
                var mainValue = property.GetValue(GetCurrentAppSettingsConfigurationFromFile());  // Reflection

                var subProperties = mainType.GetProperties();  // Reflection
                foreach (var subProperty in subProperties)
                {
                    var subKey = subProperty.Name;
                    var subTypeName = subProperty.PropertyType.Name;
                    var subValue = subProperty.GetValue(mainValue);  // Reflection
                    var subValueAsString = JsonConvert.SerializeObject(subValue);
                    if (subValue is string)
                    {
                        var subEncryptedValue = _securityService.EncryptData(subValueAsString);
                        subValueAsString = subEncryptedValue;
                    }
                    result.Add(new ConfigurationEntryDto()
                    {
                        Key = $"{mainKey}:{subKey}",
                        Value = subValueAsString,
                        DataType = $"{mainTypeName}:{subTypeName}",
                        CreatedBy = nameof(SecuredConfigurationService),
                        ModifiedBy = nameof(SecuredConfigurationService)
                    });
                }
            }

            foreach (var entry in result)
            {
                var resultEntry = await _configurationRepositoryService.UpsertAsync(entry, cancellationToken);
                if (resultEntry == null || resultEntry.Id == 0)
                {
                    // ToDo: Log that upsert went wrong.
                } 
                else
                {
                    entry.Id = resultEntry.Id;
                    entry.Created = resultEntry.Created;
                    entry.CreatedBy = resultEntry.CreatedBy;
                    entry.Modified = resultEntry.Modified;
                    entry.ModifiedBy = resultEntry.ModifiedBy;
                }
            }

            return result;
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public AppSettings GetDecryptedConfigurationForAppSettingsFromAppSettings()
        {
            var currentAppSettings = GetCurrentAppSettingsConfigurationFromFile();
            var type = typeof(AppSettings);  // Reflection
            var properties = type.GetProperties();  // Reflection
            foreach (var property in properties)
            {
                var mainType = property.PropertyType;
                var mainValue = property.GetValue(currentAppSettings);  // Reflection

                var subProperties = mainType.GetProperties();  // Reflection
                foreach (var subProperty in subProperties)
                {
                    var subValue = subProperty.GetValue(mainValue);  // Reflection
                    if (subValue is string)
                    {
                        var subValueAsString = JsonConvert.SerializeObject(subValue);
                        var subEncryptedValue = _securityService.DecryptData(subValueAsString);
                        subProperty.SetValue(mainValue, subEncryptedValue);  // Reflection
                    }
                }
            }

            return currentAppSettings;
        }

        /// <inheritdoc cref="ISecuredConfigurationService"/>
        public AppSettings GetDecryptedConfigurationForAppSettingsFromDatabase()
        {
            var target = new AppSettings();

            var configurations = _configurationRepositoryService.GetAll();

            foreach (var configuration in configurations)
            {
                var keys = configuration.Key?.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? new string[0];
                if (keys.Length == 2)
                {
                    var propertyToGet = target?.GetType().GetProperty(keys.First());  // Reflection
                    if (propertyToGet == null)
                    {
                        continue;
                    }

                    var subTarget = propertyToGet.GetValue(target);  // Reflection
                    var propertyToSet = subTarget?.GetType().GetProperty(keys.Last());  // Reflection
                    if (propertyToSet != null)
                    {
                        var configurationValue = configuration.Value;
                        var valueType = Nullable.GetUnderlyingType(propertyToSet.PropertyType) ?? propertyToSet.PropertyType;
                        if (valueType == typeof(string))
                        {
                            configurationValue = (configurationValue == null) ? null : _securityService.DecryptData(configurationValue);
                        }
                        object? safeValue = (configurationValue == null) ? null : Convert.ChangeType(configurationValue, valueType);
                        propertyToSet.SetValue(subTarget, safeValue, null);  // Reflection
                    }
                }
                else
                {
                    // ToDo: Log that something else is not supported yet, we do only support 2 levels so far.
                }
            }

            return target;
        }
    }
}
