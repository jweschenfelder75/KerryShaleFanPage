using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using KerryShaleFanPage.Client.Services;
using KerryShaleFanPage.Shared.Objects;
using System.Threading;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class Index
    {
        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        [Inject]
        protected BrowserService BrowserService { get; set; }

        private IList<NewsItemDto>? _newsItems;

        private readonly string _currentCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

        private IList<string> _cssClass = new List<string>() { string.Empty, string.Empty, string.Empty, string.Empty };

        private int _windowHeight = 0;
        private int _windowWidth = 0;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var data = await Http.GetFromJsonAsync<NewsItemDto[]>("webapi/News");
            _newsItems = data?.ToList();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var dimension = await BrowserService.GetDimensions();
            _windowHeight = dimension.Height;
            _windowWidth = dimension.Width;
            await base.OnAfterRenderAsync(firstRender);
        }

        private Task FlipCardAsync(int cardNumber)
        {
            _cssClass[cardNumber] = (string.IsNullOrWhiteSpace(_cssClass[cardNumber])) 
                ? "flipped" 
                : string.Empty;
            return Task.CompletedTask;
        }
    }
}
