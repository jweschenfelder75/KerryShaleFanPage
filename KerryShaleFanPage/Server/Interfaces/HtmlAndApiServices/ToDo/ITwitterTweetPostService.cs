using System;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices.ToDo
{
    [Obsolete("TODO: Not finally implemented yet! Do not use!")]
    public interface ITwitterTweetPostService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="episode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> SendTweetAsync(PodcastEpisodeDto? episode, CancellationToken cancellationToken = default);
    }
}
