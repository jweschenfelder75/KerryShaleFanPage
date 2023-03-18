using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Client.Shared
{
    public partial class CultureSelector
    {
        [Inject]
        protected IJSRuntime? JsRuntime { get; set; }

        [Inject]
        protected NavigationManager? Navigation { get; set; }

        private CultureInfo[] _supportedCultures = new[]
        {
            new CultureInfo("en"),
            new CultureInfo("de"),
        };

        private CultureInfo _culture
        {
            get => CultureInfo.CurrentCulture;
            set
            {
                if (CultureInfo.CurrentCulture != value)
                {
                    var js = (IJSInProcessRuntime?)JsRuntime;
                    js?.InvokeVoid("blazorCulture.set", value.Name);

                    Navigation?.NavigateTo(Navigation.Uri, forceLoad: true);
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
    }
}
