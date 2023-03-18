using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Client.Shared
{
    public partial class NavMenu
    {
        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        private bool _sidebarExpanded = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
    }
}
