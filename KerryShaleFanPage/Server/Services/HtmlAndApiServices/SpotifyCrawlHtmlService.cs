using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices;
using KerryShaleFanPage.Shared.Objects.Spotify;

namespace KerryShaleFanPage.Server.Services.HtmlAndApiServices
{
    public class SpotifyCrawlHtmlService : IGenericCrawlHtmlService<SpotifyEpisode>
    {
        public string ShowId => "5wlxQqMAIgsPu4OMp0yG0j";  // TODO: Make configurable!

        public string ShowTitle => @"Is It Rolling, Bob? Talking Dylan";  // TODO: Make configurable!

        private readonly ILogger<SpotifyCrawlHtmlService> _logger;  // TODO: Implement logging!

        private readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// 
        /// </summary>
        public SpotifyCrawlHtmlService(ILogger<SpotifyCrawlHtmlService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc cref="IGenericCrawlHtmlService{SpotifyEpisode}"/>
        public async Task<SpotifyEpisode?> GetLatestEpisodeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var html = await GetHtmlAsync($"https://open.spotify.com/show/{ShowId}", cancellationToken);
                var doc = Supremes.Dcsoup.Parse(html);
                var episodeTitle = string.Empty;
                var imageSrc = string.Empty;
                var episodeDescription = string.Empty;
                var episodeDate = string.Empty;
                var episodeLength = string.Empty;

                var showTitleTag = doc.Select("h1[data-testid=showTitle]");
                if (showTitleTag is not { HasText: true } || showTitleTag.Text != ShowTitle)
                {
                    return null;
                }
                var latestEpisodeTag = doc.Select("div[data-testid=episode-0]");
                if (latestEpisodeTag == null)
                {
                    return null;
                }
                var episodeTitleTag = latestEpisodeTag.Select("h4[data-testid=episodeTitle]");
                if (episodeTitleTag is { HasText: true })
                {
                    episodeTitle = episodeTitleTag.Text;
                }
                var imageSrcTag = latestEpisodeTag.Select("img");
                if (imageSrcTag != null && imageSrcTag.HasAttr("src"))
                {
                    imageSrc = imageSrcTag.Attr("src");
                }
                var typeTags = latestEpisodeTag.Select("p[data-encore-id=type]");
                if (typeTags is { Count: 3 })
                {
                    episodeDescription = typeTags[0].Text;
                    episodeDate = typeTags[1].Text;
                    episodeLength = typeTags[2].Text;
                }

                return new SpotifyEpisode()
                {
                    Title = episodeTitle,
                    Description = episodeDescription,
                    Date = episodeDate,
                    Duration = episodeLength,
                    ImageBaseUrl = imageSrc
                };
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
                return null;
            }
        }

        /// <inheritdoc cref="IGenericCrawlHtmlService{SpotifyEpisode}"/>
        public async Task<byte[]> GetImageAsByteArrayAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                // using var _httpClient = new HttpClient();
                //_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");  // Roadrunner is greeting! ;-)

                var bytes = await _httpClient.GetByteArrayAsync(url, cancellationToken).ConfigureAwait(false);

                return bytes;
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
                return Array.Empty<byte>();
            }
        }

        /// <inheritdoc cref="IGenericCrawlHtmlService{SpotifyEpisode}"/>
        public async Task<string> GetImageAsBase64StringAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                // using var _httpClient = new HttpClient();
                //_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");  // Roadrunner is greeting! ;-)

                var bytes = await GetImageAsByteArrayAsync(url, cancellationToken).ConfigureAwait(false);

                return $"image/jpeg;base64,{Convert.ToBase64String(bytes)}";
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<string> GetHtmlAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                // using var _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");  // Roadrunner is greeting! ;-)

                var result = await _httpClient.GetStringAsync(url, cancellationToken);

                return string.IsNullOrWhiteSpace(result)
                    ? string.Empty
                    : result;
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
                return string.Empty;
            }
        }
    }
}
