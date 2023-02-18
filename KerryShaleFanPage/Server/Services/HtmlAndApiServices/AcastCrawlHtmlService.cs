using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices;
using KerryShaleFanPage.Shared.Objects.Acast;

namespace KerryShaleFanPage.Server.Services.HtmlAndApiServices
{
    public class AcastCrawlHtmlService : IGenericCrawlHtmlService<AcastEpisode>
    {
        public string ShowId => "63d0e60777d9ee0011a4f45b";  // TODO: Make configurable!

        public string ShowTitle => @"Is It Rolling, Bob? Talking Dylan";  // TODO: Make configurable!

        private readonly ILogger<AcastCrawlHtmlService> _logger;  // TODO: Implement logging!

        private readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// 
        /// </summary>
        public AcastCrawlHtmlService(ILogger<AcastCrawlHtmlService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc cref="IGenericCrawlHtmlService{AcastEpisode}"/>
        public async Task<AcastEpisode?> GetLatestEpisodeAsync(CancellationToken cancellationToken = default)
        {
            var html = await GetHtmlAsync($"https://shows.acast.com/{ShowId}", cancellationToken);
            var doc = Supremes.Dcsoup.Parse(html);
            var episodeTitle = string.Empty;
            var imageSrc = string.Empty;
            var episodeDescription = string.Empty;
            var episodeDate = string.Empty;
            var episodeLength = string.Empty;

            var showTitleTag = doc.Select("h1");
            if (showTitleTag is not { HasText: true } || showTitleTag.Text != ShowTitle)
            {
                return null;
            }
            var latestEpisodeTag = doc.Select("div[class^=\"ant-row EpisodesGrid__FirstEpisode\"]");
            if (latestEpisodeTag == null)
            {
                return null;
            }
            var latestEpisodeSubTag = latestEpisodeTag.Select("span[class^=CardEpisode__Tag]");
            if (latestEpisodeSubTag == null || latestEpisodeSubTag is { HasText: false } || (!latestEpisodeSubTag.Text.Equals("Latest Episode", StringComparison.InvariantCultureIgnoreCase)))
            {
                return null;
            }
            var episodeDateTag = latestEpisodeTag.Select("span[class^=CardEpisode__DatePublishFeat]");
            if (episodeDateTag is { HasText: true })
            {
                episodeDate = episodeDateTag.Text;
            }
            var episodeTitleTag = latestEpisodeTag.Select("h2");
            if (episodeTitleTag is { HasText: true })
            {
                episodeTitle = episodeTitleTag.Text;
            }
            var episodeDescriptionTag = latestEpisodeTag.Select("div[class^=CardEpisode__FeatCardSummary]");
            if (episodeDescriptionTag is { HasText: true })
            {
                episodeDescription = episodeDescriptionTag.Text;
            }
            var imageSrcTag = latestEpisodeTag.Select("a[class^=Link__LinkElement]");
            if (imageSrcTag != null && imageSrcTag.HasAttr("href"))
            {
                imageSrc = imageSrcTag.Attr("href");
            }

            return new AcastEpisode()
            {
                BaseTitle = episodeTitle,
                Description = episodeDescription,
                Date = episodeDate,
                Duration = episodeLength,
                ImageBaseUrl = imageSrc,
                EpisodeShowId = ShowId
            };
        }

        /// <inheritdoc cref="IGenericCrawlHtmlService{AcastEpisode}"/>
        public async Task<byte[]> GetImageAsByteArrayAsync(string url, CancellationToken cancellationToken = default)
        {
            // using var _httpClient = new HttpClient();
            //_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");  // Roadrunner is greeting! ;-)

            var bytes = await _httpClient.GetByteArrayAsync(url, cancellationToken).ConfigureAwait(false);

            return bytes;
        }

        /// <inheritdoc cref="IGenericCrawlHtmlService{AcastEpisode}"/>
        public async Task<string> GetImageAsBase64StringAsync(string url, CancellationToken cancellationToken = default)
        {
            // using var _httpClient = new HttpClient();
            //_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");  // Roadrunner is greeting! ;-)

            var bytes = await GetImageAsByteArrayAsync(url, cancellationToken).ConfigureAwait(false);

            return $"image/jpeg;base64,{Convert.ToBase64String(bytes)}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<string> GetHtmlAsync(string url, CancellationToken cancellationToken = default)
        {
            // using var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");  // Roadrunner is greeting! ;-)

            var result = await _httpClient.GetStringAsync(url, cancellationToken);

            return string.IsNullOrWhiteSpace(result) 
                ? string.Empty 
                : result;
        }

    }
}
