using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Context.Entities;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices.ToDo;

namespace KerryShaleFanPage.Server.Services.HtmlAndApiServices.ToDo
{
    [Obsolete("TODO: Not finally implemented yet! Do not use!")]
    public class TwitterTweetPostService : ITwitterTweetPostService
    {
        public string UserName => "kerryshalefanpg";  // TODO: Make configurable!

        public string ProfileName => @"Kerry Shale Fan Page";  // TODO: Make configurable!

        private readonly ILogger<TwitterCrawlHtmlService> _logger;  // TODO: Implement logging!

        /// <summary>
        /// 
        /// </summary>
        public TwitterTweetPostService(ILogger<TwitterCrawlHtmlService> logger)
        {
            _logger = logger;
        }

        /// TODO: UNFINISHED METHOD!
        /// <inheritdoc cref="ITwitterTweetPostService"/>
        public async Task<bool> SendTweet(PodcastEpisode? episode, CancellationToken cancellationToken = default)
        {
            // Current idea: Store all data from episode in files (TXT and JPG/PNG), then let some Monkey script read the data and send the tweet.
            // Question: Can a monkey script access a database directly? https://gist.github.com/n-bell/b375c80b638d3a59a250e903afb4a36b?
            // For Monkey script: candidates are e.g. Tampermonkey, Greasemonkey, Violentmonkey

            return false;
        }
    }
}
