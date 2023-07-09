using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Server.Interfaces.BusinessLogic;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices;
using KerryShaleFanPage.Server.Interfaces.Repositories;
using KerryShaleFanPage.Server.Interfaces.MailAndSmsServices;
using KerryShaleFanPage.Shared.Extensions;
using KerryShaleFanPage.Shared.Objects;
using KerryShaleFanPage.Shared.Objects.Acast;
using KerryShaleFanPage.Shared.Objects.ListenNotes;
using KerryShaleFanPage.Shared.Objects.Spotify;

namespace KerryShaleFanPage.Server.Services.BusinessLogic
{
    public class PodcastBusinessLogicService : IPodcastBusinessLogicService
    {
        private readonly IGmxMailAndSmsService _mailAndSmsService;
        private readonly IGenericRepositoryService<PodcastEpisodeDto> _repositoryService;
        private readonly IGenericCrawlHtmlService<AcastEpisode> _acastCrawlService;
        private readonly IGenericCrawlHtmlService<ListenNotesEpisode> _listenNotesCrawlService;
        private readonly IGenericCrawlHtmlService<SpotifyEpisode> _spotifyCrawlService;

        private readonly ILogger<PodcastBusinessLogicService> _logger;  // TODO: Implement logging!

        /// <summary>
        /// 
        /// </summary>
        public PodcastBusinessLogicService(ILogger<PodcastBusinessLogicService> logger, IGmxMailAndSmsService mailAndSmsService, 
            IGenericRepositoryService<PodcastEpisodeDto> repositoryService, IGenericCrawlHtmlService<AcastEpisode> acastCrawlService,
            IGenericCrawlHtmlService<ListenNotesEpisode> listenNotesCrawlService, IGenericCrawlHtmlService<SpotifyEpisode> spotifyCrawlService)
        {
            _logger = logger;
            _mailAndSmsService = mailAndSmsService;
            _repositoryService = repositoryService;
            _acastCrawlService= acastCrawlService;
            _listenNotesCrawlService = listenNotesCrawlService;
            _spotifyCrawlService = spotifyCrawlService;
        }

        /// <inheritdoc cref="IBusinessLogicService" />
        public async Task DoWorkAsync(CancellationToken cancellationToken = default)
        {
            var nowDate = DateTime.UtcNow.Date;

            while (!cancellationToken.IsCancellationRequested)
            {
                var sleepPeriod = GetSleepPeriod(nowDate);

                _logger.LogInformation($"Podcast Business Logic Service was called (execution every: {sleepPeriod.TotalMinutes} min).");

                var latestPodcastEpisodeDto = await StoreLatestPodcastEpisodeInDatabaseAsync(cancellationToken);
                if (latestPodcastEpisodeDto == null)
                {
                    _logger.LogInformation("No new podcast episode found (yet).");
                }
                else
                {
                    _logger.LogInformation($"New episode fetched successfully: {latestPodcastEpisodeDto.Title} ({latestPodcastEpisodeDto.Date})");
                    var success = _mailAndSmsService.SendSmsNotification(string.Empty, string.Empty, "New podcast episode is out!", string.Empty, latestPodcastEpisodeDto);
                    if (success)
                    {
                        _logger.LogInformation($"Notification for new podcast episode sent successfully: {latestPodcastEpisodeDto.Title} ({latestPodcastEpisodeDto.Date})");
                    }
                    else
                    {
                        _logger.LogError($"Notification for new podcast episode could not be sent: {latestPodcastEpisodeDto.Title} ({latestPodcastEpisodeDto.Date})");
                    }
                }

                await Task.Delay((int)sleepPeriod.TotalMilliseconds, cancellationToken);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        private TimeSpan GetSleepPeriod(DateTime nowDate)
        {
            var latestStoredPodcastEpisodeDto = _repositoryService.GetLast();
            var expectedNextEpisodeDate = latestStoredPodcastEpisodeDto?.CalculatedExpectedNextDate.Date ?? nowDate;

            var sleepPeriod = TimeSpan.FromMinutes(15);
            if (nowDate <= expectedNextEpisodeDate.AddDays(-21)) sleepPeriod = TimeSpan.FromHours(12);
            else if (nowDate <= expectedNextEpisodeDate.AddDays(-14)) sleepPeriod = TimeSpan.FromHours(6);
            else if (nowDate <= expectedNextEpisodeDate.AddDays(-7)) sleepPeriod = TimeSpan.FromHours(2);
            else if (nowDate <= expectedNextEpisodeDate.AddDays(-1)) sleepPeriod = TimeSpan.FromHours(1);
            else if (nowDate == expectedNextEpisodeDate) sleepPeriod = TimeSpan.FromMinutes(15);
            else if (nowDate > expectedNextEpisodeDate) sleepPeriod = TimeSpan.FromHours(3);
            return sleepPeriod;
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
                if (DateTime.UtcNow.Date > latestStoredPodcastEpisodeDto.CalculatedExpectedNextDate.Date)
                {
                    _logger.LogWarning($"New podcast episode is overdue, it was expected on: {latestStoredPodcastEpisodeDto.CalculatedExpectedNextDate.Date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}");
                }
                return null;
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

            var latestAcastImageData = await _acastCrawlService.GetImageAsBase64Async(latestAcastEpisode.ImageUrl ?? string.Empty, cancellationToken);
            var latestAcastImageBase64 = await _acastCrawlService.GetImageAsBase64StringAsync(latestAcastEpisode.ImageUrl ?? string.Empty, cancellationToken);

            return new PodcastEpisodeDto()
            {
                Id = latestAcastEpisode.Id,
                Title = (latestAcastEpisode?.Title ?? string.Empty).Length > 100 ? latestAcastEpisode?.Title?[..100] : latestAcastEpisode?.Title,
                Description = (latestAcastEpisode?.Description ?? string.Empty).Length > 1024 ? latestAcastEpisode?.Description?[..1024] : latestAcastEpisode?.Description,
                ImageUrl = (latestAcastEpisode?.ImageUrl ?? string.Empty).Length > 255 ? latestAcastEpisode?.ImageUrl?[..255] : latestAcastEpisode?.ImageUrl,
                ImageData = latestAcastImageData,
                ImageDataBase64 = latestAcastImageBase64,
                Date = latestAcastEpisode?.Date.ToDateTime("dd.MM.yyyy"),  // e.g. "Sunday, May 14, 2023"
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

            var latestListenNotesImageData = await _listenNotesCrawlService.GetImageAsBase64Async(latestListenNotesEpisode.ImageUrl ?? string.Empty, cancellationToken);
            var latestListenNotesImageBase64 = await _listenNotesCrawlService.GetImageAsBase64StringAsync(latestListenNotesEpisode.ImageUrl ?? string.Empty, cancellationToken);

            return new PodcastEpisodeDto()
            {
                Id = latestListenNotesEpisode.Id,
                Title = (latestListenNotesEpisode?.Title ?? string.Empty).Length > 100 ? latestListenNotesEpisode?.Title?[..100] : latestListenNotesEpisode?.Title,
                Description = (latestListenNotesEpisode?.Description ?? string.Empty).Length > 1024 ? latestListenNotesEpisode?.Description?[..1024] : latestListenNotesEpisode?.Description,
                ImageUrl = (latestListenNotesEpisode?.ImageUrl ?? string.Empty).Length > 255 ? latestListenNotesEpisode?.ImageUrl?[..255] : latestListenNotesEpisode?.ImageUrl,
                ImageData = latestListenNotesImageData,
                ImageDataBase64 = latestListenNotesImageBase64,
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

            var latestSpotifyImageData = await _spotifyCrawlService.GetImageAsBase64Async(latestSpotifyEpisode.ImageUrl ?? string.Empty, cancellationToken);  // Backup
            var latestSpotifyImageBase64 = await _listenNotesCrawlService.GetImageAsBase64StringAsync(latestSpotifyEpisode.ImageUrl ?? string.Empty, cancellationToken);

            return new PodcastEpisodeDto()
            {
                Id = latestSpotifyEpisode.Id,
                Title = (latestSpotifyEpisode?.Title ?? string.Empty).Length > 100 ? latestSpotifyEpisode?.Title?[..100] : latestSpotifyEpisode?.Title,
                Description = (latestSpotifyEpisode?.Description ?? string.Empty).Length > 1024 ? latestSpotifyEpisode?.Description?[..1024] : latestSpotifyEpisode?.Description,
                ImageUrl = (latestSpotifyEpisode?.ImageUrl ?? string.Empty).Length > 255 ? latestSpotifyEpisode?.ImageUrl?[..255] : latestSpotifyEpisode?.ImageUrl,
                ImageData = latestSpotifyImageData,
                ImageDataBase64 = latestSpotifyImageBase64,
                Date = latestSpotifyEpisode?.Date.ToDateTime("MMM yy"),  // e.g. "Jan 23"
                Duration = latestSpotifyEpisode?.Duration,
                Checksum= latestSpotifyEpisode?.Checksum,
                FetchedExpectedNextDate = null
            };
        }
    }
}
