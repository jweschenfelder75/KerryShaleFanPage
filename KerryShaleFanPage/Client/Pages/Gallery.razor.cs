using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class Gallery
    {
        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        private const int _MAX_IMG = 5;

        private int _imgNumber = 0;

        private string _img => $"/img/KS00{(_imgNumber + 1)}.png";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
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
