using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class FunStuff : IAsyncDisposable
    {
        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        private readonly string _currentCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

        private Timer? _timer;
        private DateTime _timeNow;
        private string _localTime = string.Empty;
        private string _shaleTime = string.Empty;
        private NumberFormatInfo _nfi;

        string hr1, min1, sec1;
        string hr2, min2, sec2;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _timeNow = DateTime.Now;

            _nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = ","
            };

            SetClockAsync(null);

            _timer = new Timer(SetClockAsync, new AutoResetEvent(false), 1000, 1000); // 50 milliseconds
        }

        private async void SetClockAsync(object? stateInfo)
        {
            _timeNow = DateTime.Now;
            var shaleTimeNow = _timeNow.AddMinutes(10);
            var threshold = 89.996;
            hr1 = ((360.0 * _timeNow.Hour / 12) + (30.0 * _timeNow.Minute / 60.0) + threshold).ToString(CultureInfo.InvariantCulture);
            min1 = ((360.0 * _timeNow.Minute / 60) + (6.0 * _timeNow.Second / 60.0) + threshold).ToString(CultureInfo.InvariantCulture);
            sec1 = ((360.0 * _timeNow.Second / 60) + (6.0 * _timeNow.Millisecond / 1000.0) + threshold).ToString(CultureInfo.InvariantCulture);
            hr2 = ((360.0 * shaleTimeNow.Hour / 12) + (30.0 * shaleTimeNow.Minute / 60.0) + threshold).ToString(CultureInfo.InvariantCulture);
            min2 = ((360.0 * shaleTimeNow.Minute / 60) + (6.0 * shaleTimeNow.Second / 60.0) + threshold).ToString(CultureInfo.InvariantCulture);
            sec2 = ((360.0 * shaleTimeNow.Second / 60) + (6.0 * shaleTimeNow.Millisecond / 1000.0) + threshold).ToString(CultureInfo.InvariantCulture);
            var timeFormat = _currentCulture.Equals("de", StringComparison.InvariantCultureIgnoreCase) ? "HH:mm:ss U\\hr" : "hh:mm:ss tt";
            _localTime = $"{Translate["Local Time:"]} {_timeNow.ToString(timeFormat)}";
            _shaleTime = $"{Translate["Shale Time:"]} {shaleTimeNow.ToString(timeFormat)}";
            await InvokeAsync(StateHasChanged);
        }

        public async ValueTask DisposeAsync()
        {
            if (_timer != null) 
            {
                await _timer.DisposeAsync();
            }
        }
    }
}
