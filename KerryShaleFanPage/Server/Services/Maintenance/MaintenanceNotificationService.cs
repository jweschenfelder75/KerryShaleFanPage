using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using KerryShaleFanPage.Server.Hub;
using KerryShaleFanPage.Server.Interfaces.Maintenance;
using KerryShaleFanPage.Server.Interfaces.Security;
using KerryShaleFanPage.Shared.Events;

namespace KerryShaleFanPage.Server.Services.Maintenance
{
    public class MaintenanceNotificationService : IMaintenanceNotificationService
    {
        private readonly IHubContext<SignalRHub> _hubContext;

        private readonly ISecuredConfigurationService _securedConfigurationService;

        private readonly ILogger<MaintenanceNotificationService> _logger;  // TODO: Implement logging!

        /// <summary>
        /// 
        /// </summary>
        public MaintenanceNotificationService(ILogger<MaintenanceNotificationService> logger, IHubContext<SignalRHub> hubContext, 
            ISecuredConfigurationService securedConfigurationService)
        {
            _logger = logger;
            _hubContext = hubContext;
            _securedConfigurationService = securedConfigurationService;
        }

        /// <inheritdoc cref="IMaintenanceNotificationService" />
        public async Task NotifyAllConnectedClientsInCaseOfMaintenanceAsync(CancellationToken cancellationToken = default)
        {
            var currentMaintenanceSettings = _securedConfigurationService.GetCurrentAppSettingsConfigurationFromFile();
            var cachedMaintenanceSettings = _securedConfigurationService.GetCachedAppSettingsConfigurationFromFile();
            if ((currentMaintenanceSettings.GeneralMaintainance?.Enabled ?? false) || ((currentMaintenanceSettings.GeneralMaintainance?.Enabled ?? false) != (cachedMaintenanceSettings.GeneralMaintainance?.Enabled ?? false)))  // Configuration has changed
            {
                var message = new MaintenanceMessageEventArgs() { IsEnabled = (currentMaintenanceSettings.GeneralMaintainance?.Enabled ?? false), Message = (currentMaintenanceSettings.GeneralMaintainance?.Message ?? string.Empty) };
                await _hubContext.Clients.All.SendAsync("ReceiveMaintenanceMessage", JsonConvert.SerializeObject(message));
            }
        }
    }
}
