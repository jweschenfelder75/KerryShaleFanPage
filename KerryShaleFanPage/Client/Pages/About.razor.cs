using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class About
    {
        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        private int GetCurrentAge()
        {
            DateTime zeroDate = new DateTime(1, 1, 1);

            DateTime nowDate = DateTime.UtcNow.Date;
            DateTime birthDate = new DateTime(1975, 11, 6).Date;

            TimeSpan span = nowDate - birthDate;
            return (zeroDate + span).Year - 1;
        }
    }
}
