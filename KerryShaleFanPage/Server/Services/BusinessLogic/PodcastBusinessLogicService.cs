using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using KerryShaleFanPage.Server.Interfaces.BusinessLogic;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices;
using KerryShaleFanPage.Server.Interfaces.Repositories;
using KerryShaleFanPage.Server.Interfaces.MailAndSmsServices;
using KerryShaleFanPage.Server.Interfaces.Security;
using KerryShaleFanPage.Server.Services.HtmlAndApiServices;
using KerryShaleFanPage.Server.Services.Security;
using KerryShaleFanPage.Shared.Configuration;
using KerryShaleFanPage.Shared.Extensions;
using KerryShaleFanPage.Shared.Objects;
using KerryShaleFanPage.Shared.Objects.Acast;
using KerryShaleFanPage.Shared.Objects.ListenNotes;
using KerryShaleFanPage.Shared.Objects.Spotify;
using KerryShaleFanPage.Shared.Objects.ToDo.Twitter;
using KerryShaleFanPage.Server.Interfaces.Maintenance;

namespace KerryShaleFanPage.Server.Services.BusinessLogic
{
    public class PodcastBusinessLogicService : IPodcastBusinessLogicService
    {
        private readonly TimeSpan _sleepPeriod = TimeSpan.FromMinutes(15);  // Make configurable!
        private readonly IMailAndSmsService _mailAndSmsService;
        private readonly IGenericRepositoryService<PodcastEpisodeDto> _repositoryService;
        private readonly IGenericCrawlHtmlService<AcastEpisode> _acastCrawlService;
        private readonly IGenericCrawlHtmlService<ListenNotesEpisode> _listenNotesCrawlService;
        private readonly IGenericCrawlHtmlService<SpotifyEpisode> _spotifyCrawlService;
        // private readonly ITwitterCrawlApiService _twitterCrawlApiService;  // TODO: Obsolete: We will not use Twitter API anymore! It was tested and it was ok.
        // private readonly ITwitterTweetApiService _twitterTweetApiService;  // TODO: Obsolete: We will not use Twitter API anymore! Unfinished & untested.
        // private readonly IGenericCrawlHtmlService<TwitterEpisode> _twitterCrawlService;  // TODO: Unfinished & untested.
        private readonly ISecuredConfigurationService _securedConfigurationService;
        private readonly IMaintenanceNotificationService _maintenanceNotificationService;

        private readonly ILogger<PodcastBusinessLogicService> _logger;  // TODO: Implement logging!

        /// <summary>
        /// 
        /// </summary>
        public PodcastBusinessLogicService(ILogger<PodcastBusinessLogicService> logger, IMailAndSmsService mailAndSmsService, 
            IGenericRepositoryService<PodcastEpisodeDto> repositoryService, IGenericCrawlHtmlService<AcastEpisode> acastCrawlService,
            IGenericCrawlHtmlService<ListenNotesEpisode> listenNotesCrawlService, IGenericCrawlHtmlService<SpotifyEpisode> spotifyCrawlService
            /* , ITwitterCrawlApiService twitterCrawlApiService, ITwitterTweetApiService twitterTweetApiService */
            /* , IGenericCrawlHtmlService<TwitterEpisode> twitterCrawlService */, ISecuredConfigurationService securedConfigurationService,
            IMaintenanceNotificationService maintenanceNotificationService)
        {
            _logger = logger;
            _mailAndSmsService = mailAndSmsService;
            _repositoryService = repositoryService;
            _acastCrawlService= acastCrawlService;
            _listenNotesCrawlService = listenNotesCrawlService;
            _spotifyCrawlService = spotifyCrawlService;
            // _twitterCrawlApiService = twitterCrawlApiService;  // TODO: Obsolete: We will not use Twitter API anymore! It was tested and it was ok.
            // _twitterTweetApiService = twitterTweetApiService;  // TODO: Obsolete: We will not use Twitter API anymore! Unfinished & untested.
            // _twitterCrawlService = twitterCrawlService;  // TODO: Unfinished & untested.
            _securedConfigurationService = securedConfigurationService;
            _maintenanceNotificationService = maintenanceNotificationService;
        }

        /// <inheritdoc cref="IPodcastBusinessLogicService" />
        public async Task DoWorkAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Podcast Business Logic Service was called (execution every: {_sleepPeriod.TotalMinutes} min).");

                await _maintenanceNotificationService.NotifyAllConnectedClientsInCaseOfMaintenanceAsync(cancellationToken);

                var latestPodcastEpisodeDto = await StoreLatestPodcastEpisodeInDatabaseAsync(cancellationToken);
                if (latestPodcastEpisodeDto == null)
                {
                    // TODO: Information that there is obviously a problem fetching the latest episode!
                }
                else
                {
                    var success = _mailAndSmsService.SendSmsNotification("weschi@gmx.com", "weschi@gmx.com", "New podcast episode is out!", string.Empty, latestPodcastEpisodeDto);  // Make configurable and encrypt!
                    if (!success)
                    {
                        // TODO: Information that there is obviously a problem sending the notification!
                    }
                }

                // Other examples:
                // var latestPodcastEpisodeDto = await StoreLatestPodcastEpisodeInDatabaseAsync(cancellationToken);
                // latestPodcastEpisodeDto = FetchLatestPodcastEpisodeFromDatabase();

                await Task.Delay((int)_sleepPeriod.TotalMilliseconds, cancellationToken);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<PodcastEpisodeDto?> StoreLatestPodcastEpisodeInDatabaseAsync(CancellationToken cancellationToken = default)
        {
            var latestStoredPodcastEpisodeDto = _repositoryService.GetLast();
            var latestCrawledPodcastEpisodeDto = await GetLatestEpisodeFromCrawlServiceAsync(cancellationToken);

            if (latestCrawledPodcastEpisodeDto == null)
            {
                // TODO: Information that there is obviously a problem fetching the latest episode!
                return null;
            }

            if (latestStoredPodcastEpisodeDto == null)
            {
                return await _repositoryService.UpsertAsync(latestCrawledPodcastEpisodeDto, cancellationToken);
            }

            if (latestStoredPodcastEpisodeDto.Date >= latestCrawledPodcastEpisodeDto.Date 
                || (latestStoredPodcastEpisodeDto.Title ?? string.Empty).Equals(latestCrawledPodcastEpisodeDto.Title ?? string.Empty, StringComparison.InvariantCultureIgnoreCase))
            {
                return latestCrawledPodcastEpisodeDto;
            }

            return await _repositoryService.UpsertAsync(latestCrawledPodcastEpisodeDto, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<PodcastEpisodeDto?> GetLatestEpisodeFromCrawlServiceAsync(CancellationToken cancellationToken = default)
        {
            var latestPodcastEpisodeDto = await GetLatestEpisodeFromCrawlServiceMainAsync(cancellationToken);
            if (latestPodcastEpisodeDto != null)
            {
                if (latestPodcastEpisodeDto.Date.HasValue && latestPodcastEpisodeDto.Date.Value.Year < DateTime.UtcNow.Year)
                {
                    // TODO: Information that the data is obviously wrong!
                    return null;
                }
                return latestPodcastEpisodeDto;
            }

            // TODO: Information that there is obviously a problem fetching the latest episode! Backup 1 is called!
            // 2nd try aka Backup 1: 
            latestPodcastEpisodeDto = await GetLatestEpisodeFromCrawlServiceBackup1Async(cancellationToken);
            if (latestPodcastEpisodeDto != null)
            {
                if (latestPodcastEpisodeDto.Date.HasValue && latestPodcastEpisodeDto.Date.Value.Year < DateTime.UtcNow.Year)
                {
                    // TODO: Information that the data is obviously wrong!
                    return null;
                }
                return latestPodcastEpisodeDto;
            }

            // TODO: Information that there is obviously a problem fetching the latest episode! Backup 2 is called!
            // 3rd try aka Backup 2: 
            latestPodcastEpisodeDto = await GetLatestEpisodeFromCrawlServiceBackup2Async(cancellationToken);
            if (latestPodcastEpisodeDto == null)
            {
                // TODO: Information that there is obviously a problem fetching the latest episode!
                return null;
            }

            if (latestPodcastEpisodeDto.Date.HasValue && latestPodcastEpisodeDto.Date.Value.Year < DateTime.UtcNow.Year)
            {
                // TODO: Information that the data is obviously wrong!
                return null;
            }

            return latestPodcastEpisodeDto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<PodcastEpisodeDto?> GetLatestEpisodeFromCrawlServiceMainAsync(CancellationToken cancellationToken = default)
        {
            var latestAcastEpisode = await _acastCrawlService.GetLatestEpisodeAsync(cancellationToken);  // Acast is new leading podcast source
            if (latestAcastEpisode == null)
            {
                return null;
            }

            var latestAcastImageData = await _acastCrawlService.GetImageAsByteArrayAsync(latestAcastEpisode.ImageUrl ?? string.Empty, cancellationToken);
            var latestAcastImageBase64 = await _acastCrawlService.GetImageAsBase64StringAsync(latestAcastEpisode.ImageUrl ?? string.Empty, cancellationToken);

            return new PodcastEpisodeDto()
            {
                Id = latestAcastEpisode.Id,
                Title = (latestAcastEpisode?.Title ?? string.Empty).Length > 100 ? latestAcastEpisode?.Title?[..100] : latestAcastEpisode?.Title,
                Description = (latestAcastEpisode?.Description ?? string.Empty).Length > 1024 ? latestAcastEpisode?.Description?[..1024] : latestAcastEpisode?.Description,
                ImageUrl = (latestAcastEpisode?.ImageUrl ?? string.Empty).Length > 255 ? latestAcastEpisode?.ImageUrl?[..255] : latestAcastEpisode?.ImageUrl,
                ImageData = latestAcastImageData,
                ImageDataBase64 = (latestAcastImageBase64 ?? string.Empty).Length > 14821 ? latestAcastImageBase64?[..14821] : latestAcastImageBase64,
                Date = latestAcastEpisode?.Date.ToDateTime("M/d/yyyy"),  // e.g. "1/22/2023"
                Duration = latestAcastEpisode?.Duration,
                Checksum = latestAcastEpisode?.Checksum,
                FetchedExpectedNextDate = null
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<PodcastEpisodeDto?> GetLatestEpisodeFromCrawlServiceBackup1Async(CancellationToken cancellationToken = default)
        {
            var latestListenNotesEpisode = await _listenNotesCrawlService.GetLatestEpisodeAsync(cancellationToken);  // Backup 1, ListenNotes is obviously late
            if (latestListenNotesEpisode == null)
            {
                return null;
            }

            var latestListenNotesImageData = await _listenNotesCrawlService.GetImageAsByteArrayAsync(latestListenNotesEpisode.ImageUrl ?? string.Empty, cancellationToken);
            var latestListenNotesImageBase64 = await _listenNotesCrawlService.GetImageAsBase64StringAsync(latestListenNotesEpisode.ImageUrl ?? string.Empty, cancellationToken);

            return new PodcastEpisodeDto()
            {
                Id = latestListenNotesEpisode.Id,
                Title = (latestListenNotesEpisode?.Title ?? string.Empty).Length > 100 ? latestListenNotesEpisode?.Title?[..100] : latestListenNotesEpisode?.Title,
                Description = (latestListenNotesEpisode?.Description ?? string.Empty).Length > 1024 ? latestListenNotesEpisode?.Description?[..1024] : latestListenNotesEpisode?.Description,
                ImageUrl = (latestListenNotesEpisode?.ImageUrl ?? string.Empty).Length > 255 ? latestListenNotesEpisode?.ImageUrl?[..255] : latestListenNotesEpisode?.ImageUrl,
                ImageData = latestListenNotesImageData,
                ImageDataBase64 = (latestListenNotesImageBase64 ?? string.Empty).Length > 14821 ? latestListenNotesImageBase64?[..14821] : latestListenNotesImageBase64,
                Date = latestListenNotesEpisode?.Date.ToDateTime("MMM. dd, yyyy"),  // e.g. "Jan. 01, 2023"
                Duration = latestListenNotesEpisode?.Duration,
                Checksum = latestListenNotesEpisode?.Checksum,
                FetchedExpectedNextDate = null
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<PodcastEpisodeDto?> GetLatestEpisodeFromCrawlServiceBackup2Async(CancellationToken cancellationToken = default)
        {
            var latestSpotifyEpisode = await _spotifyCrawlService.GetLatestEpisodeAsync(cancellationToken);  // Backup 2, Spotify is obvioulsy faster than ListenNotes
            if (latestSpotifyEpisode == null)
            {
                return null;
            }

            var latestSpotifyImageData = await _spotifyCrawlService.GetImageAsByteArrayAsync(latestSpotifyEpisode.ImageUrl ?? string.Empty, cancellationToken);  // Backup
            var latestSpotifyImageBase64 = await _listenNotesCrawlService.GetImageAsBase64StringAsync(latestSpotifyEpisode.ImageUrl ?? string.Empty, cancellationToken);

            return new PodcastEpisodeDto()
            {
                Id = latestSpotifyEpisode.Id,
                Title = (latestSpotifyEpisode?.Title ?? string.Empty).Length > 100 ? latestSpotifyEpisode?.Title?[..100] : latestSpotifyEpisode?.Title,
                Description = (latestSpotifyEpisode?.Description ?? string.Empty).Length > 1024 ? latestSpotifyEpisode?.Description?[..1024] : latestSpotifyEpisode?.Description,
                ImageUrl = (latestSpotifyEpisode?.ImageUrl ?? string.Empty).Length > 255 ? latestSpotifyEpisode?.ImageUrl?[..255] : latestSpotifyEpisode?.ImageUrl,
                ImageData = latestSpotifyImageData,
                ImageDataBase64 = (latestSpotifyImageBase64 ?? string.Empty).Length > 14821 ? latestSpotifyImageBase64?[..14821] : latestSpotifyImageBase64,
                Date = latestSpotifyEpisode?.Date.ToDateTime("MMM yy"),  // e.g. "Jan 23"
                Duration = latestSpotifyEpisode?.Duration,
                Checksum= latestSpotifyEpisode?.Checksum,
                FetchedExpectedNextDate = null
            };

            // Backup 3 (TODO: Rest of implementation missing!):
            // var twitterUsers = await _twitterCrawlApiService.GetUsersAsync(new List<string> { "isitrollingpod" }, cancellationToken);  // Returns the needed UserId, TODO: Currently not used, I do not think we need this! It was tested and it was ok.
            // var twitterUserTweets = await _twitterCrawlApiService.GetTweetsAsync("984758353787260928", cancellationToken);  // TODO: Currently not used, I do not think we need this! It was tested and it was ok.
        }
    }
}
