using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class Index
    {
        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        private string? _test { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _test = Translate["Index.Welcome"];
        }
    }
}
