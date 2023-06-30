using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class FunStuff
    {
        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        Timer? timer;

        double hr1, min1, sec1;
        double hr2, min2, sec2;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            SetClock(null);

            timer = new Timer(SetClock, new AutoResetEvent(false), 10, 10); // 10 milliseconds
        }

        // NOTE: this math can be simplified!!!
        private void SetClock(object? stateInfo)
        {
            var timeNow = DateTime.Now;
            var britishZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
            var londonTimeNow = TimeZoneInfo.ConvertTime(timeNow, TimeZoneInfo.Local, britishZone);
            var shaleTimeNow = londonTimeNow.AddMinutes(10);
            hr1 = 360.0 * londonTimeNow.Hour / 12 + 30.0 * londonTimeNow.Minute / 60.0;
            min1 = 360.0 * londonTimeNow.Minute / 60 + 6.0 * londonTimeNow.Second / 60.0;
            sec1 = 360.0 * londonTimeNow.Second / 60 + 6.0 * londonTimeNow.Millisecond / 1000.0;
            hr2 = 360.0 * shaleTimeNow.Hour / 12 + 30.0 * shaleTimeNow.Minute / 60.0;
            min2 = 360.0 * shaleTimeNow.Minute / 60 + 6.0 * shaleTimeNow.Second / 60.0;
            sec2 = 360.0 * shaleTimeNow.Second / 60 + 6.0 * shaleTimeNow.Millisecond / 1000.0;
            StateHasChanged(); // MUST CALL StateHasChanged() BECAUSE THIS IS TRIGGERED BY A TIMER INSTEAD OF A USER EVENT
        }
    }
}
