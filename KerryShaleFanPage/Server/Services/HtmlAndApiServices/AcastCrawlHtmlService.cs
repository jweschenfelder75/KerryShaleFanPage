using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices;
using KerryShaleFanPage.Shared.Objects.Acast;
using System.Linq;

namespace KerryShaleFanPage.Server.Services.HtmlAndApiServices
{
    public class AcastCrawlHtmlService : IGenericCrawlHtmlService<AcastEpisode>
    {
        public string ShowId => "63d0e60777d9ee0011a4f45b";  // TODO: Make configurable!

        public string ShowTitle => @"Is It Rolling, Bob? Talking Dylan";  // TODO: Make configurable!

        private readonly HttpClient _httpClient = new HttpClient();

        private readonly ILogger<AcastCrawlHtmlService> _logger;  // TODO: Implement logging!

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
            try
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
                var latestEpisodeTag = doc.Select("div[class^=\"ant-spin-container\"]");
                if (latestEpisodeTag == null)
                {
                    return null;
                }
                var latestEpisodeSubTag = latestEpisodeTag.Select("li[class^=ant-list-item]")?[0];
                if (latestEpisodeSubTag == null || latestEpisodeSubTag is { HasText: false })
                {
                    return null;
                }
                var episodeTitleTag = latestEpisodeTag.Select("h4[class^=Typography__SubTitle]")?[0];
                if (episodeTitleTag is { HasText: true })
                {
                    episodeTitle = episodeTitleTag.Text.Substring(4).Trim();
                }
                episodeDate = DateTime.UtcNow.ToShortDateString();
                var episodeLengthTag = latestEpisodeTag.Select("span[class^=EpisodeListItem__Duration]")?[0];
                if (episodeLengthTag is { HasText: true })
                {
                    episodeLength = episodeLengthTag.Text;
                } 
                else
                {
                    episodeLengthTag = latestEpisodeTag.Select("span[class^=EpisodeMobileListItem__Duration]")?[0];
                    if (episodeLengthTag is { HasText: true })
                    {
                        episodeLength = episodeLengthTag.Text;
                    }
                }
                var episodeDescriptionTag = latestEpisodeTag.Select("div[class^=EpisodeListItem__FeatCardSummary]")?[0];
                if (episodeDescriptionTag is { HasText: true })
                {
                    episodeDescription = episodeDescriptionTag.Text;
                } 
                else
                {
                    episodeDescriptionTag = latestEpisodeTag.Select("div[class^=EpisodeMobileListItem__FeatCardSummary]")?[0];
                    if (episodeDescriptionTag is { HasText: true })
                    {
                        episodeDescription = episodeDescriptionTag.Text;
                    }
                }
                var imageSrcTag = latestEpisodeTag.Select("img")?[0];
                if (imageSrcTag != null && imageSrcTag.HasAttr("src"))
                {
                    imageSrc = imageSrcTag.Attr("src");
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
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
                return null;
            }
        }

        /// <inheritdoc cref="IGenericCrawlHtmlService{AcastEpisode}"/>
        public async Task<string> GetImageAsBase64Async(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                // using var _httpClient = new HttpClient();
                //_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");  // Roadrunner is greeting! ;-)

                var bytes = await _httpClient.GetByteArrayAsync(url, cancellationToken).ConfigureAwait(false);

                return Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
                return string.Empty;
            }
        }

        /// <inheritdoc cref="IGenericCrawlHtmlService{AcastEpisode}"/>
        public async Task<string> GetImageAsBase64StringAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                // using var _httpClient = new HttpClient();
                //_httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");  // Roadrunner is greeting! ;-)

                var bytes = await _httpClient.GetByteArrayAsync(url, cancellationToken).ConfigureAwait(false);

                return $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";
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
            // using var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");  // Roadrunner is greeting! ;-)

            var result = await _httpClient.GetStringAsync(url, cancellationToken);

            return string.IsNullOrWhiteSpace(result) 
                ? string.Empty 
                : result;
        }

    }
}
