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

        private IList<NewsItemDto>? _newsItems;

        private PodcastEpisodeDto? _latestPodcastEpisode;

        private bool _showLatestPodcastEpisode => ShowLatestPodcastEpisode();

        private string? _latestPodcastEpisodeImageBase64;

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
            var newsData = await Http.GetFromJsonAsync<NewsItemDto[]>("webapi/News");
            _newsItems = newsData?.ToList();
            var latestPodcastData = await Http.GetFromJsonAsync<PodcastEpisodeDto>("webapi/Podcast");
            _latestPodcastEpisode = latestPodcastData;
            if (_latestPodcastEpisode != null && !string.IsNullOrWhiteSpace(_latestPodcastEpisode.ImageDataBase64))
            {
                _latestPodcastEpisodeImageBase64 = _latestPodcastEpisode.ImageDataBase64;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var dimension = await BrowserService.GetDimensions();
            _windowHeight = dimension.Height;
            _windowWidth = dimension.Width;
            await base.OnAfterRenderAsync(firstRender);
        }

        private bool ShowLatestPodcastEpisode()
        {
            var dateNow = DateTime.UtcNow.Date;
            if (_latestPodcastEpisode == null || string.IsNullOrWhiteSpace(_latestPodcastEpisodeImageBase64)
                || !_latestPodcastEpisode.Date.HasValue || (dateNow > _latestPodcastEpisode.Date?.Date.AddDays(14)))
            {
                return false;
            }

            return true;
        }

        private Task FlipCardAsync(int cardNumber)
        {
            _cssClass[cardNumber] = (string.IsNullOrWhiteSpace(_cssClass[cardNumber])) 
                ? "flipped" 
                : string.Empty;
            return Task.CompletedTask;
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
