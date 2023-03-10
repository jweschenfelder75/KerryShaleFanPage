using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices;
using KerryShaleFanPage.Shared.Objects.ToDo.Twitter;

namespace KerryShaleFanPage.Server.Services.HtmlAndApiServices.ToDo
{
    [Obsolete("TODO: Not finally implemented yet! Do not use!")]
    public class TwitterCrawlHtmlService : IGenericCrawlHtmlService<TwitterEpisode>
    {
        public string UserName => "isitrollingpod";  // TODO: Make configurable!

        public string ProfileName => @"Is It Rolling, Bob? Talking Dylan";  // TODO: Make configurable!

        public string ApiKey => "";  // TODO: Make configurable and HIDE!

        public string FromDateAsStr => DateTime.Now.AddDays(-7).Date.ToString("yyyy-mm-dd");  // TODO: Make configurable!

        public string ToDateAsStr => DateTime.Now.Date.ToString("yyyy-mm-dd");  // TODO: Make configurable!

        public string ToUserName => "PantheonPods";  // TODO: Make configurable!

        public string Keywords => "episode";  // TODO: Make configurable!

        private readonly HttpClient _httpClient = new HttpClient();

        private readonly ILogger<TwitterCrawlHtmlService> _logger;  // TODO: Implement logging!

        /// <summary>
        /// 
        /// </summary>
        public TwitterCrawlHtmlService(ILogger<TwitterCrawlHtmlService> logger)
        {
            _logger = logger;
        }

        /// TODO: UNFINISHED METHOD!
        /// <inheritdoc cref="IGenericCrawlHtmlService{TweetEpisode}"/>
        public async Task<TwitterEpisode?> GetLatestEpisodeAsync(CancellationToken cancellationToken = default)
        {
            // var html = await GetHtmlAsync($"https://twitter.com/search?q={Keywords} (from%3A{UserName}) (%40{ToUserName}) until%3A{ToDateAsStr} since%3A{FromDateAsStr} filter%3Alinks -filter%3Areplies&src=typed_query", cancellationToken);
            var html = await GetHtmlAsync($"https://twitter.com/{UserName}", cancellationToken);
            var doc = Supremes.Dcsoup.Parse(html);
            var episodeTitle = string.Empty;
            var imageSrc = string.Empty;
            var episodeDescription = string.Empty;
            var episodeDate = string.Empty;

            var showTitleTag = doc.Select("div[data-testid=UserName]");
            if (showTitleTag is not { HasText: true } || showTitleTag.Text != ProfileName)
            {
                return null;
            }
            var latestEpisodesTag = doc.Select("div[data-testid=tweet]");
            if (latestEpisodesTag == null || latestEpisodesTag.Count < 1)
            {
                return null;
            }

            foreach (var latestEpisodeTag in latestEpisodesTag)
            {
                var episodeDateTag = latestEpisodeTag.Select("time");
                if (episodeDateTag != null && episodeDateTag.HasAttr("datetime"))
                {
                    episodeDate = episodeDateTag.Attr("datetime");
                }
                var episodeDescriptionTag = latestEpisodeTag.Select("div[data-testid=tweetText]");
                if (episodeDescriptionTag is { HasText: true })
                {
                    episodeTitle = episodeDescriptionTag.Text;
                }
                var episodeImageTag = latestEpisodeTag.Select("div[data-testid=tweetPhoto]");
                if (episodeImageTag != null)
                {
                    var imageSrcTag = latestEpisodeTag.Select("img");
                    if (imageSrcTag != null && imageSrcTag.HasAttr("src"))
                    {
                        imageSrc = imageSrcTag.Attr("src");
                    }
                }
            }

            return new TwitterEpisode()
            {
                Title = episodeTitle,
                Description = episodeDescription,
                Date = episodeDate,
                ImageBaseUrl = imageSrc
            };
        }

        /// <inheritdoc cref="IGenericCrawlHtmlService{TweetEpisode}"/>
        public async Task<byte[]> GetImageAsByteArrayAsync(string url, CancellationToken cancellationToken = default)
        {
            var bytes = await _httpClient.GetByteArrayAsync(url, cancellationToken);

            return bytes ?? Array.Empty<byte>();
        }

        /// <inheritdoc cref="IGenericCrawlHtmlService{TweetEpisode}"/>
        public async Task<string> GetImageAsBase64StringAsync(string url, CancellationToken cancellationToken = default)
        {
            var bytes = await GetImageAsByteArrayAsync(url, cancellationToken);

            return $"image/jpeg;base64,{Convert.ToBase64String(bytes)}";
        }

        /// <summary>
        /// TODO: UNFINISHED METHOD!
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<string> GetHtmlAsync(string url, CancellationToken cancellationToken = default)
        {
            // using var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");  // Roadrunner is greeting! ;-)

            var pageRequestJson = new StringContent($@"{{ 'url':'{url}','renderType':'html','outputAsJson':false }}");  // does not work with Twitter, is blocked
            // var pageRequestJson = new StringContent($@"{{ 'url':'{url}','renderType':'png','outputAsJson':false }}");  // works with Twitter, but is very unhandy
            // var pageRequestJson = new StringContent($@"{{ 'url':'{url}','renderType':'pdf','outputAsJson':false }}");  // works with Twitter, but is very unhandy
            var response = await _httpClient.PostAsync($"https://PhantomJsCloud.com/api/browser/v2/{ApiKey}", pageRequestJson, cancellationToken);
            var result = await response.Content.ReadAsStringAsync(cancellationToken);

            return string.IsNullOrWhiteSpace(result)
                ? string.Empty
                : result;
        }
    }
}
