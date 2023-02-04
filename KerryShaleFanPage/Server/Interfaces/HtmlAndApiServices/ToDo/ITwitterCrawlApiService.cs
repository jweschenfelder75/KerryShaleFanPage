using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Shared.Objects.ToDo.Twitter;

namespace KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices.ToDo
{
    [Obsolete("Obsolete: We will not use Twitter API anymore!")]
    public interface ITwitterCrawlApiService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="usernames"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Users?> GetUsersAsync(IList<string>? usernames, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Tweets?> GetTweetsAsync(string? userId, CancellationToken cancellationToken = default);
    }
}
