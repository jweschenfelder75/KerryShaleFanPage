using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class Rules : IAsyncDisposable
    {
        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
