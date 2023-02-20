using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices;
using KerryShaleFanPage.Shared.Objects.ListenNotes;

namespace KerryShaleFanPage.Server.Services.HtmlAndApiServices
{
    public class ListenNotesCrawlHtmlService : IGenericCrawlHtmlService<ListenNotesEpisode>
    {
        public string ShowId => "is-it-rolling-bob-talking-dylan-lucas-hare-Fc6aE4N8pHR";  // TODO: Make configurable!

        public string ShowTitle => @"Is It Rolling, Bob? Talking Dylan";  // TODO: Make configurable!

        private readonly ILogger<ListenNotesCrawlHtmlService> _logger;  // TODO: Implement logging!

        private readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// 
        /// </summary>
        public ListenNotesCrawlHtmlService(ILogger<ListenNotesCrawlHtmlService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc cref="IGenericCrawlHtmlService{ListenNotesEpisode}"/>
        public async Task<ListenNotesEpisode?> GetLatestEpisodeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var html = await GetHtmlAsync($"https://www.listennotes.com/podcasts/{ShowId}", cancellationToken);
                var doc = Supremes.Dcsoup.Parse(html);
                var episodeTitle = string.Empty;
                var imageSrc = string.Empty;
                var episodeDescription = string.Empty;
                var episodeDate = string.Empty;
                var episodeLength = string.Empty;

                var showTitleTag = doc.Select("h1[class*=ln-l1-text] > a");
                if (showTitleTag == null || !showTitleTag.HasText || showTitleTag.Text != ShowTitle)
                {
                    return null;
                }
                var latestEpisodeTag = doc.Select("div[id=ln-episode-list]");
                if (latestEpisodeTag == null)
                {
                    return null;
                }
                var episodeTitleTag = latestEpisodeTag.Select("h3");
                if (episodeTitleTag is { HasText: true })
                {
                    episodeTitle = episodeTitleTag.Text;
                }
                var imageSrcTag = latestEpisodeTag.Select($"a[title={episodeTitle}]");
                if (imageSrcTag != null && imageSrcTag.HasAttr("href"))
                {
                    imageSrc = imageSrcTag.Attr("href");
                    if (string.IsNullOrWhiteSpace(imageSrc))  // Backup
                    {
                        imageSrcTag = latestEpisodeTag.Select("img");
                        if (imageSrcTag != null && imageSrcTag.HasAttr("src"))
                        {
                            imageSrc = imageSrcTag.Attr("src");
                        }
                    }
                }
                var episodeDescriptionTag = latestEpisodeTag.Select("div[class=text-sm line-clamp-2 ln-wrap-line leading-normal]");
                if (episodeDescriptionTag is { HasText: true })
                {
                    episodeDescription = episodeDescriptionTag.Text;
                }
                var episodeDateTag = latestEpisodeTag.Select("time");
                if (episodeDateTag is { HasText: true })
                {
                    episodeDate = episodeDateTag.Text;
                }
                var episodeLengthTag = latestEpisodeTag.Select("div[data-type=episode-audio-player]");
                if (episodeLengthTag != null && episodeLengthTag.HasAttr("data-duration"))
                {
                    episodeLength = episodeLengthTag.Attr("data-duration");
                }

                return new ListenNotesEpisode()
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

        /// <inheritdoc cref="IGenericCrawlHtmlService{ListenNotesEpisode}"/>
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

        /// <inheritdoc cref="IGenericCrawlHtmlService{ListenNotesEpisode}"/>
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
