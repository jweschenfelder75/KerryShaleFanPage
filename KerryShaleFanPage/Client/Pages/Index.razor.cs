using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Client.Services;
using KerryShaleFanPage.Shared.Objects;
using KerryShaleFanPage.Client.Objects;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class Index : IAsyncDisposable
    {
        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        [Inject]
        protected BrowserService BrowserService { get; set; }


        private static IList<TileContent> _tileMainContents = new List<TileContent> {
            new TileContent {
                First = new TileContentItem("Frogwares", "Play Sherlock Holmes", "/img/frogwares.jpg"),
                Second = new TileContentItem(),
            },
            new TileContent {
                First = new TileContentItem("Bob Dylan podcast", "Hear the latest podcast episode", "/img/podcast.jpg"),
                Second = new TileContentItem(),
            },
            new TileContent {
                First = new TileContentItem("The Wild Gentlemen", "Play Chicken Police", "/img/thewildgentlemen.jpg"),
                Second = new TileContentItem(),
            },
        };

        private static IList<TileContent> _tileSubContents = new List<TileContent> {
            new TileContent {
                First = new TileContentItem("The Testament Of Sherlock Holmes", "https://store.steampowered.com/app/205650/The_Testament_of_Sherlock_Holmes/", "/img/frogwares-tosh.jpg"),
                Second = new TileContentItem("Sherlock Holmes: Crimes & Punishments", "https://store.steampowered.com/app/241260/Sherlock_Holmes_Crimes_and_Punishments/", "/img/frogwares-cap.jpg"),
            },
            new TileContent { 
                First = new TileContentItem("Bob Dylan podcast at Apple", "https://podcasts.apple.com/gb/podcast/is-it-rolling-bob-talking-dylan/id1437321669", "/img/listen-on-apple-podcasts-1.png"),
                Second = new TileContentItem("Bob Dylan podcast at Acast", "https://shows.acast.com/63d0e60777d9ee0011a4f45b", "/img/listen-on-acast.png"),
            },
            new TileContent {
                First = new TileContentItem("Chicken Police: Paint it RED", "https://store.steampowered.com/app/1084640/Chicken_Police__Paint_it_RED/", "/img/paint-it-red.jpg"),
                Second = new TileContentItem("Chicken Police: Into the HIVE", "https://store.steampowered.com/app/2362090/Chicken_Police_Into_the_HIVE/", "/img/into-the-hive.jpg"),
            },
        };

        private TileContent _currentTileMainContent = _tileMainContents.FirstOrDefault() ?? new TileContent();
        private TileContent _currentTileSubContent = _tileSubContents.FirstOrDefault() ?? new TileContent();

        private IList<NewsItemDto>? _newsItems;

        //private PodcastEpisodeDto? _latestPodcastEpisode;

        //private bool _showLatestPodcastEpisode => ShowLatestPodcastEpisode();

        //private string? _latestPodcastEpisodeImageBase64;

        private readonly string _currentCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

        private IList<string> _cssClass = new List<string>() { string.Empty, string.Empty, string.Empty, string.Empty };

        private int _windowHeight = 0;
        private int _windowWidth = 0;

        private Timer? _timer;
        private string _localTime = string.Empty;
        private string _shaleTime = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Tick(null);
            _timer = new Timer(Tick, new AutoResetEvent(false), 5000, 5000);
            await SetTileContentBasedOnCurrentDay();
            var newsData = await Http.GetFromJsonAsync<NewsItemDto[]>("webapi/News");
            _newsItems = newsData?.ToList();
            //var latestPodcastData = await Http.GetFromJsonAsync<PodcastEpisodeDto>("webapi/Podcast");
            //_latestPodcastEpisode = latestPodcastData;
            //if (_latestPodcastEpisode != null && !string.IsNullOrWhiteSpace(_latestPodcastEpisode.ImageDataBase64))
            //{
            //    _latestPodcastEpisodeImageBase64 = _latestPodcastEpisode.ImageDataBase64;
            //}
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var dimension = await BrowserService.GetDimensions();
            _windowHeight = dimension.Height;
            _windowWidth = dimension.Width;
            await base.OnAfterRenderAsync(firstRender);
        }

        //private bool ShowLatestPodcastEpisode()
        //{
        //    var dateNow = DateTime.UtcNow.Date;
        //    if (_latestPodcastEpisode == null || string.IsNullOrWhiteSpace(_latestPodcastEpisodeImageBase64)
        //        || !_latestPodcastEpisode.Date.HasValue || (dateNow > _latestPodcastEpisode.Date?.Date.AddDays(14)))
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        private Task FlipCardAsync(int cardNumber)
        {
            _cssClass[cardNumber] = (string.IsNullOrWhiteSpace(_cssClass[cardNumber])) 
                ? "flipped" 
                : string.Empty;
            return Task.CompletedTask;
        }

        private async Task SetTileContentBasedOnCurrentDay()
        {
            var day = DateTime.Now.Day;
            var currentItem = (day % 3);
            if (_tileMainContents.Count > currentItem)
            {
                _currentTileMainContent = _tileMainContents.ElementAtOrDefault(currentItem) ?? new TileContent();
            }
            if (_tileSubContents.Count > currentItem)
            {
                _currentTileSubContent = _tileSubContents.ElementAtOrDefault(currentItem) ?? new TileContent();
            }
            Console.WriteLine(currentItem);
            await InvokeAsync(StateHasChanged);
        }

        private async void Tick(object? obj)
        {
            var timeNow = DateTime.Now;
            var shaleTimeNow = timeNow.AddMinutes(10);
            var timeFormat = _currentCulture.Equals("de", StringComparison.InvariantCultureIgnoreCase) ? "HH:mm U\\hr" : "hh:mm tt";
            _localTime = $"{Translate["Local Time:"]} {timeNow.ToString(timeFormat)}";
            _shaleTime = $"{Translate["Shale Time:"]} {shaleTimeNow.ToString(timeFormat)}";
            await InvokeAsync(StateHasChanged);
        }

        public async ValueTask DisposeAsync()
        {
            if (_timer != null)
            {
                await _timer.DisposeAsync();
            }
        }
    }
}
