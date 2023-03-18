using System.Threading;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Server.Interfaces.Maintenance
{
    public interface IMaintenanceNotificationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task NotifyAllConnectedClientsInCaseOfMaintenanceAsync(CancellationToken cancellationToken = default);
    }
}
