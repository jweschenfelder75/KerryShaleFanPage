using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class Gallery : IAsyncDisposable
    {
        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        [Inject]
        protected HttpClient Http { get; set; }

        private IList<GalleryItemDto>? _galleryItems;

        private int _maxImages = 0;

        private int _imgNumber = 0;

        private readonly string _currentCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

        private GalleryItemDto? _img => _galleryItems?.ElementAt(_imgNumber);

        private string? _imgAlt => (_currentCulture.Equals("de", StringComparison.InvariantCultureIgnoreCase)) ? _img?.ImageAltDe : _img?.ImageAltEn;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var data = await Http.GetFromJsonAsync<GalleryItemDto[]>("webapi/Gallery");
            _galleryItems = data?.ToList();
            _maxImages = _galleryItems?.Count() ?? 0;
            await ImageGalleryAsync();
        }

        private async Task ImageGalleryAsync()
        {
            while (_imgNumber < _maxImages) 
            {
                _imgNumber++;
                if (_imgNumber == _maxImages)
                {
                    _imgNumber = 0;
                }
                await InvokeAsync(StateHasChanged);
                await Task.Delay(10000);
            }
        }

        private async Task BackAsync()
        {
            if (_imgNumber == 0)
            {
                _imgNumber = (_maxImages - 1);
            } 
            else
            {
                _imgNumber--;
            }
            await InvokeAsync(StateHasChanged);
        }

        private async Task NextAsync()
        {
            if (_imgNumber == (_maxImages - 1))
            {
                _imgNumber = 0;
            } 
            else
            {
                _imgNumber++;
            }
            await InvokeAsync(StateHasChanged);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
