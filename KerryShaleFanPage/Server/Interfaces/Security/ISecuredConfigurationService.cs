﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Shared.Configuration;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Server.Interfaces.Security
{
    public interface ISecuredConfigurationService
    {
        /// <summary>
        /// Returns the old cached AppSettings data. Implementation hint: You can compare single properties with GetCurrentAppSettingsConfigurationFromFile() to see if something particular has changed.
        /// </summary>
        /// <returns></returns>
        public AppSettings GetCachedAppSettingsConfigurationFromFile();

        /// <summary>
        /// Returns the current AppSettings data. Implementation hint: You can compare single properties with GetCachedAppSettingsConfigurationFromFile() to see if something particular has changed.
        /// </summary>
        /// <returns></returns>
        public AppSettings GetCurrentAppSettingsConfigurationFromFile();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AppSettings GetEncryptedConfigurationForAppSettingsFromAppSettings();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AppSettings GetEncryptedConfigurationForAppSettingsFromDatabase();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<IList<ConfigurationEntryDto>> GetEncryptedConfigurationForDatabaseFromAppSettingsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AppSettings GetDecryptedConfigurationForAppSettingsFromAppSettings();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AppSettings GetDecryptedConfigurationForAppSettingsFromDatabase();
    }
}
