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
    public partial class Gallery
    {
        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        [Inject]
        protected HttpClient Http { get; set; }

        private IList<GalleryItemDto>? _galleryItems;

        private const int _MAX_IMG = 5;

        private int _imgNumber = 0;

        private readonly string _currentCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

        private GalleryItemDto? _img => _galleryItems?.ElementAt(_imgNumber);

        private string? _imgAlt => (_currentCulture.Equals("de", StringComparison.InvariantCultureIgnoreCase)) ? _img?.ImageAltDe : _img?.ImageAltEn;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var data = await Http.GetFromJsonAsync<GalleryItemDto[]>("webapi/Gallery");
            _galleryItems = data?.ToList();
            await ImageGalleryAsync();
        }

        private async Task ImageGalleryAsync()
        {
            while (_imgNumber < _MAX_IMG) 
            {
                _imgNumber++;
                if (_imgNumber == _MAX_IMG)
                {
                    _imgNumber = 0;
                }
                StateHasChanged();
                await Task.Delay(10000);
            }
        }

        private Task BackAsync()
        {
            if (_imgNumber == 0)
            {
                _imgNumber = (_MAX_IMG - 1);
            } 
            else
            {
                _imgNumber--;
            }
            StateHasChanged();
            return Task.CompletedTask;
        }

        private Task NextAsync()
        {
            if (_imgNumber == (_MAX_IMG - 1))
            {
                _imgNumber = 0;
            } 
            else
            {
                _imgNumber++;
            }
            StateHasChanged();
            return Task.CompletedTask;
        }

    }
}
