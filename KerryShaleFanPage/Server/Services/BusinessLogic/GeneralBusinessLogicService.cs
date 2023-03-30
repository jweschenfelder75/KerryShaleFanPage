using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Server.Interfaces.BusinessLogic;
using KerryShaleFanPage.Server.Interfaces.Repositories;
using KerryShaleFanPage.Server.Interfaces.Security;
using KerryShaleFanPage.Shared.Objects;
using KerryShaleFanPage.Server.Interfaces.Maintenance;

namespace KerryShaleFanPage.Server.Services.BusinessLogic
{
    public class GeneralBusinessLogicService : IGeneralBusinessLogicService
    {
        private readonly TimeSpan _sleepPeriod = TimeSpan.FromMinutes(15);
        private readonly IGenericRepositoryService<PodcastEpisodeDto> _podcastRepositoryService;
        private readonly IGenericRepositoryService<LogEntryDto> _logRepositoryService;
        private readonly ISecuredConfigurationService _securedConfigurationService;
        private readonly IMaintenanceNotificationService _maintenanceNotificationService;

        private readonly ILogger<GeneralBusinessLogicService> _logger;  // TODO: Implement logging!

        /// <summary>
        /// 
        /// </summary>
        public GeneralBusinessLogicService(ILogger<GeneralBusinessLogicService> logger, 
            IGenericRepositoryService<PodcastEpisodeDto> podcastRepositoryService, IGenericRepositoryService<LogEntryDto> logRepositoryService,
            ISecuredConfigurationService securedConfigurationService, IMaintenanceNotificationService maintenanceNotificationService)
        {
            _logger = logger;
            _podcastRepositoryService = podcastRepositoryService;
            _logRepositoryService = logRepositoryService;
            _securedConfigurationService = securedConfigurationService;
            _maintenanceNotificationService = maintenanceNotificationService;
        }

        /// <inheritdoc cref="IBusinessLogicService" />
        public async Task DoWorkAsync(CancellationToken cancellationToken = default)
        {
            var nowDate = DateTime.UtcNow.Date;

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"General Business Logic Service was called (execution every: {_sleepPeriod.TotalMinutes} min).");

                await _maintenanceNotificationService.NotifyAllConnectedClientsInCaseOfMaintenanceAsync(cancellationToken);

                var settings = _securedConfigurationService.GetCurrentSettingsConfigurationFromFile();
                if (settings.OverrideCurrentGeneralSettings && settings.OverrideCurrentGeneralSettingsFile)
                {
                    settings.OverrideCurrentGeneralSettings = false;
                    settings.OverrideCurrentGeneralSettingsFile = false;
                    _securedConfigurationService.GetEncryptedConfigurationForEncryptedFileFromSettings();
                } 
                else
                {
                    settings = _securedConfigurationService.GetDecryptedConfigurationForSettingsFromEncryptedFile();
                }

                var appSettings = _securedConfigurationService.GetCurrentAppSettingsConfigurationFromFile();
                if (appSettings.GeneralLogging.DeleteLogsEnabled && appSettings.GeneralLogging.DeleteLogsAfterDays > 0)
                {
                    var success = await _logRepositoryService.DeleteWhereAsync(l => l.Created.HasValue 
                                    && (l.Created.Value.Date < DateTime.UtcNow.Date.AddDays(-1 * appSettings.GeneralLogging.DeleteLogsAfterDays)));
                    if (!success)
                    {
                        // TODO: Log that deletion of log entries was not successful.
                    }
                }

                await Task.Delay((int)_sleepPeriod.TotalMilliseconds, cancellationToken);
            }
        }
    }
}
