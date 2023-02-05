using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices;
using KerryShaleFanPage.Server.Interfaces.Repositories;
using KerryShaleFanPage.Server.Interfaces.BusinessLogic;
using KerryShaleFanPage.Server.Services.HtmlAndApiServices;
using KerryShaleFanPage.Shared.Extensions;
using KerryShaleFanPage.Shared.Objects.ListenNotes;
using KerryShaleFanPage.Shared.Objects.Spotify;
using KerryShaleFanPage.Shared.Objects;
using KerryShaleFanPage.Shared.Objects.ToDo.Twitter;

namespace KerryShaleFanPage.Server.Services.BusinessLogic
{
    public class PodcastBusinessLogicService : IPodcastBusinessLogicService
    {
        private readonly ILogger<PodcastBusinessLogicService> _logger;  // TODO: Implement logging!
        private readonly IGenericRepositoryService<PodcastEpisodeDto> _repositoryService;
        private readonly IGenericCrawlHtmlService<ListenNotesEpisode> _listenNotesCrawlService;
        private readonly IGenericCrawlHtmlService<SpotifyEpisode> _spotifyCrawlService;
        // private readonly ITwitterCrawlApiService _twitterCrawlApiService;  // TODO: Obsolete: We will not use Twitter API anymore! It was tested and it was ok.
        // private readonly ITwitterTweetApiService _twitterTweetApiService;  // TODO: Obsolete: We will not use Twitter API anymore! Unfinished & untested.
        // private readonly IGenericCrawlHtmlService<TwitterEpisode> _twitterCrawlService;  // TODO: Unfinished & untested.

        /// <summary>
        /// 
        /// </summary>
        public PodcastBusinessLogicService(ILogger<PodcastBusinessLogicService> logger, IGenericRepositoryService<PodcastEpisodeDto> repositoryService,
            IGenericCrawlHtmlService<ListenNotesEpisode> listenNotesCrawlService, IGenericCrawlHtmlService<SpotifyEpisode> spotifyCrawlService
            /* , ITwitterCrawlApiService twitterCrawlApiService, ITwitterTweetApiService twitterTweetApiService */
            /* , IGenericCrawlHtmlService<TwitterEpisode> twitterCrawlService */)
        {
            _logger = logger;
            _repositoryService = repositoryService;
            _listenNotesCrawlService = listenNotesCrawlService;
            _spotifyCrawlService = spotifyCrawlService;
            // _twitterCrawlApiService = twitterCrawlApiService;  // TODO: Obsolete: We will not use Twitter API anymore! It was tested and it was ok.
            // _twitterTweetApiService = twitterTweetApiService;  // TODO: Obsolete: We will not use Twitter API anymore! Unfinished & untested.
            // _twitterCrawlService = twitterCrawlService;  // TODO: Unfinished & untested.
        }

        /// <inheritdoc cref="IPodcastBusinessLogicService" />
        public async Task DoWorkAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                _logger.LogInformation($"Podcast Business Logic Service (via Timed Hosted Service) was called.");

                var latestPodcastEpisodeDto = await GetLatestEpisodeFromCrawlServiceAsync(cancellationToken);

                // var latestPodcastEpisodeDto = await StoreLatestPodcastEpisodeInDatabaseAsync(cancellationToken);

                // latestPodcastEpisodeDto = FetchLatestPodcastEpisodeFromDatabase();
            }
            catch (OperationCanceledException ex)
            {
                var cancelledException = ex;  // TODO: Log exception!
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
            }

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<PodcastEpisodeDto?> StoreLatestPodcastEpisodeInDatabaseAsync(CancellationToken cancellationToken = default)
        {
            var latestStoredPodcastEpisodeDto = FetchLatestPodcastEpisodeFromDatabase();

            if (latestStoredPodcastEpisodeDto == null)
            {
                return await AddInitialLatestPodcastEpisodeToDatabaseAsync(cancellationToken);
            }

            return await AddLatestPodcastEpisodeToDatabaseAsync(latestStoredPodcastEpisodeDto, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private PodcastEpisodeDto? FetchLatestPodcastEpisodeFromDatabase()
        {
            return _repositoryService.GetLast();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<PodcastEpisodeDto?> AddInitialLatestPodcastEpisodeToDatabaseAsync(CancellationToken cancellationToken = default)
        {
            var latestPodcastEpisodeDto = await GetLatestEpisodeFromCrawlServiceAsync(cancellationToken);
            if (latestPodcastEpisodeDto != null)
            {
                if (latestPodcastEpisodeDto.Date.HasValue && latestPodcastEpisodeDto.Date.Value.Year < DateTime.UtcNow.Year)
                {
                    // TODO: Information that the data is obviously wrong!
                    return null;
                }
                return await _repositoryService.UpsertAsync(latestPodcastEpisodeDto, cancellationToken);
            }
            else
            {
                // TODO: Information that there is obviously a problem fetching the latest episode!
                // 2nd try aka Backup: 
                latestPodcastEpisodeDto = await GetLatestEpisodeFromCrawlServiceBackupAsync(cancellationToken);
                if (latestPodcastEpisodeDto != null)
                {
                    if (latestPodcastEpisodeDto.Date.HasValue && latestPodcastEpisodeDto.Date.Value.Year < DateTime.UtcNow.Year)
                    {
                        // TODO: Information that the data is obviously wrong!
                        return null;
                    }
                    return await _repositoryService.UpsertAsync(latestPodcastEpisodeDto, cancellationToken);
                }
                else
                {
                    // TODO: Information that there is obviously a problem fetching the latest episode!
                    return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<PodcastEpisodeDto?> AddLatestPodcastEpisodeToDatabaseAsync(PodcastEpisodeDto dto, CancellationToken cancellationToken = default)
        {
            var latestPodcastEpisodeDto = await GetLatestEpisodeFromCrawlServiceAsync(cancellationToken);
            if (latestPodcastEpisodeDto != null)
            {
                if (latestPodcastEpisodeDto.Date.HasValue && latestPodcastEpisodeDto.Date.Value.Year < DateTime.UtcNow.Year)
                {
                    // TODO: Information that the data is obviously wrong!
                    return null;
                }
                return await _repositoryService.UpsertAsync(latestPodcastEpisodeDto, cancellationToken);
            }
            else
            {
                // TODO: Information that there is obviously a problem fetching the latest episode!
                // 2nd try aka Backup: 
                latestPodcastEpisodeDto = await GetLatestEpisodeFromCrawlServiceBackupAsync(cancellationToken);
                if (latestPodcastEpisodeDto != null)
                {
                    if (latestPodcastEpisodeDto.Date.HasValue && latestPodcastEpisodeDto.Date.Value.Year < DateTime.UtcNow.Year)
                    {
                        // TODO: Information that the data is obviously wrong!
                        return null;
                    }
                    return await _repositoryService.UpsertAsync(latestPodcastEpisodeDto, cancellationToken);
                }
                else
                {
                    // TODO: Information that there is obviously a problem fetching the latest episode!
                    return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<PodcastEpisodeDto?> GetLatestEpisodeFromCrawlServiceAsync(CancellationToken cancellationToken = default)
        {
            var latestListenNotesEpisode = await _listenNotesCrawlService.GetLatestEpisodeAsync(cancellationToken);  // ListenNotes is obviously late
            if (latestListenNotesEpisode != null)
            {
                var latestListenNotesImageData = await _listenNotesCrawlService.GetImageAsByteArrayAsync(latestListenNotesEpisode.ImageUrl ?? string.Empty, cancellationToken);
                var latestListenNotesImageBase64 = await _listenNotesCrawlService.GetImageAsBase64StringAsync(latestListenNotesEpisode.ImageUrl ?? string.Empty, cancellationToken);

                return new PodcastEpisodeDto()
                {
                    Id = latestListenNotesEpisode.Id,
                    Title = latestListenNotesEpisode.Title,
                    Description = latestListenNotesEpisode.Description,
                    ImageUrl = latestListenNotesEpisode.ImageUrl,
                    ImageData = latestListenNotesImageData,
                    ImageDataBase64 = latestListenNotesImageBase64,
                    Date = latestListenNotesEpisode.Date.ToDateTime("MMM. dd, yyyy"),  // e.g. "Jan. 01, 2023"
                    Duration = latestListenNotesEpisode.Duration,
                    FetchedExpectedNextDate = null
                };
            }

            return null;
            // Backup 2 (TODO: Rest of implementation missing!):
            // var twitterUsers = await _twitterCrawlApiService.GetUsersAsync(new List<string> { "isitrollingpod" }, cancellationToken);  // Returns the needed UserId, TODO: Currently not used, I do not think we need this! It was tested and it was ok.
            // var twitterUserTweets = await _twitterCrawlApiService.GetTweetsAsync("984758353787260928", cancellationToken);  // TODO: Currently not used, I do not think we need this! It was tested and it was ok.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<PodcastEpisodeDto?> GetLatestEpisodeFromCrawlServiceBackupAsync(CancellationToken cancellationToken = default)
        {
            var latestSpotifyEpisode = await _spotifyCrawlService.GetLatestEpisodeAsync(cancellationToken);  // Backup 1, Spotify is obvioulsy faster than ListenNotes
            if (latestSpotifyEpisode != null)
            {
                var latestSpotifyImageData = await _spotifyCrawlService.GetImageAsByteArrayAsync(latestSpotifyEpisode.ImageUrl ?? string.Empty, cancellationToken);  // Backup
                var latestSpotifyImageBase64 = await _listenNotesCrawlService.GetImageAsBase64StringAsync(latestSpotifyEpisode.ImageUrl ?? string.Empty, cancellationToken);

                return new PodcastEpisodeDto()
                {
                    Id = latestSpotifyEpisode.Id,
                    Title = latestSpotifyEpisode.Title,
                    Description = latestSpotifyEpisode.Description,
                    ImageUrl = latestSpotifyEpisode.ImageUrl,
                    ImageData = latestSpotifyImageData,
                    ImageDataBase64 = latestSpotifyImageBase64,
                    Date = latestSpotifyEpisode.Date.ToDateTime("MMM yy"),  // e.g. "Jan 23"
                    Duration = latestSpotifyEpisode.Duration,
                    FetchedExpectedNextDate = null
                };
            }

            return null;
            // Backup 2 (TODO: Rest of implementation missing!):
            // var twitterUsers = await _twitterCrawlApiService.GetUsersAsync(new List<string> { "isitrollingpod" }, cancellationToken);  // Returns the needed UserId, TODO: Currently not used, I do not think we need this! It was tested and it was ok.
            // var twitterUserTweets = await _twitterCrawlApiService.GetTweetsAsync("984758353787260928", cancellationToken);  // TODO: Currently not used, I do not think we need this! It was tested and it was ok.
        }
    }
}
