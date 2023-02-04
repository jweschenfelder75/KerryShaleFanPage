using System;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Context.Entities;

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
        public Task<bool> SendTweet(PodcastEpisode? episode, CancellationToken cancellationToken = default);
    }
}
