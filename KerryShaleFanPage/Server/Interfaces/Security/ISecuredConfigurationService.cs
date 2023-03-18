using KerryShaleFanPage.Shared.Configuration;

namespace KerryShaleFanPage.Server.Interfaces.Security
{
    public interface ISecuredConfigurationService
    {
        /// <summary>
        /// Returns the old cached GeneralSettings data. Implementation hint: You can compare single properties with GetCurrentSettingsConfigurationFromFile() to see if something particular has changed.
        /// </summary>
        /// <returns></returns>
        public GeneralSettings GetCachedSettingsConfigurationFromFile();

        /// <summary>
        /// Returns the current GeneralSettings data. Implementation hint: You can compare single properties with GetCachedSettingsConfigurationFromFile() to see if something particular has changed.
        /// </summary>
        /// <returns></returns>
        public GeneralSettings GetCurrentSettingsConfigurationFromFile();

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
        public bool GetEncryptedConfigurationForEncryptedFileFromSettings();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GeneralSettings GetDecryptedConfigurationForSettingsFromEncryptedFile();
    }
}
