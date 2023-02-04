using System.Threading;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Server.Interfaces.BusinessLogic
{
    public interface IPodcastBusinessLogicService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task DoWorkAsync(CancellationToken cancellationToken);
    }
}
