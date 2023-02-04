using System.Threading;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices
{
    public interface IGenericCrawlHtmlService<TEntity> where TEntity : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TEntity?> GetLatestEpisodeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">Image url</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<byte[]> GetImageAsByteArrayAsync(string url, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">Image url</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetImageAsBase64StringAsync(string url, CancellationToken cancellationToken = default);
    }
}
