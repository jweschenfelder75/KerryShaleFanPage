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
                var imageSrcTag = latestEpisodeTag.Select("div[class^=CardEpisode__Image]");
                if (imageSrcTag != null)
                {
                    var imageSrcTagClasses = imageSrcTag.Attr("class");
                    if (!string.IsNullOrWhiteSpace(imageSrcTagClasses))
                    {
                        var lastClassName = imageSrcTagClasses.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).LastOrDefault();
                        if (!string.IsNullOrWhiteSpace(lastClassName))
                        {
                            var cssPosStart = doc.Head.Html.IndexOf($"\n.{lastClassName}{{background-position:center;", StringComparison.InvariantCultureIgnoreCase);
                            if (cssPosStart > 0)
                            {
                                var cssPosEnd = doc.Head.Html.IndexOf("/*!sc*/\n", cssPosStart, StringComparison.InvariantCultureIgnoreCase);
                                if (cssPosEnd > cssPosStart && doc.Head.Html.Length >= cssPosEnd)
                                {
                                    var relevantCssPart = doc.Head.Html[cssPosStart..cssPosEnd];
                                    if (!string.IsNullOrWhiteSpace(relevantCssPart))
                                    {
                                        const string imageSrcCssTag = "background-image:url(";
                                        var imgPosStart = relevantCssPart.IndexOf(imageSrcCssTag, StringComparison.InvariantCultureIgnoreCase) + imageSrcCssTag.Length;
                                        if (imgPosStart > 0)
                                        {
                                            var imgPosEnd = relevantCssPart.IndexOf(");width:100%;", imgPosStart, StringComparison.InvariantCultureIgnoreCase);
                                            if (imgPosEnd > imgPosStart && relevantCssPart.Length >= imgPosEnd)
                                            {
                                                var relevantImgPart = relevantCssPart[imgPosStart..imgPosEnd];
                                                if (!string.IsNullOrWhiteSpace(relevantImgPart))
                                                {
                                                    imageSrc = relevantImgPart;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
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

        /// <inheritdoc cref="IGenericCrawlHtmlService{AcastEpisode}"/>
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
            // using var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");  // Roadrunner is greeting! ;-)

            var result = await _httpClient.GetStringAsync(url, cancellationToken);

            return string.IsNullOrWhiteSpace(result) 
                ? string.Empty 
                : result;
        }

    }
}
