using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Radzen;
using KerryShaleFanPage.Shared.Enums;
using KerryShaleFanPage.Shared.Extensions;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class Log
    {
        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        private string _pagingSummaryFormat => Translate["Page {0} of {1} ({2} records)"];

        IEnumerable<int> _pageSizeOptions = new int[] { 5, 15, 30, 50 };

        private IList<LogEntryDto>? _logEntries;

        private IList<ChartDataItem>? _logChartSeries;

        private readonly string _currentCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

        private string _utcNow = DateTime.UtcNow.ToString();

        private Timer _timer;

        bool _isLoading = false;

        class ChartDataItem
        {
            public LogLevelEnum LogLevel { get; private set; }
            public DateTime MonthYear { get; private set; }
            public int Count { get; private set; }

            public ChartDataItem(LogLevelEnum logLevel, DateTime monthYear, int count) 
            {
                LogLevel = logLevel;
                MonthYear = monthYear;
                Count = count;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            SetClockAndReloadLog(null);

            _timer = new Timer(SetClockAndReloadLog, new AutoResetEvent(false), 60000, 60000);
        }

        private async Task ShowDialogAsync(string title, string message)
        {
            await DialogService.Alert(message, title, new AlertOptions() { OkButtonText = "OK", CssClass="w-100", Style="max-width: 50% !important;" });
        }

        private async Task ShowLoadingAsync()
        {
            _isLoading = true;

            await Task.Yield();

            _isLoading = false;
        }

        private string FormatAsMonthYear(object value)
        {
            if (value != null)
            {
                return Convert.ToDateTime(value).ToString("MMM yyyy");
            }

            return string.Empty;
        }

        private string GetFrontColor(LogLevelEnum logLevel)
        {
            return logLevel.GetFrontColorAttributeFrom(typeof(LogLevelEnum)) ?? string.Empty;
        }

        private string GetBackColor(LogLevelEnum logLevel)
        {
            return logLevel.GetBackColorAttributeFrom(typeof(LogLevelEnum)) ?? string.Empty;
        }

        private async void SetClockAndReloadLog(object? stateInfo)
        {
            _utcNow = (_currentCulture.Equals("de", StringComparison.InvariantCultureIgnoreCase)) 
                ? DateTime.UtcNow.ToString("d.M.yyyy H:mm")
                : DateTime.UtcNow.ToString("M/d/yyyy h:mm tt");

            var data = await Http.GetFromJsonAsync<LogEntryDto[]>("webapi/Log");
            _logEntries = data?.ToList();
            _logChartSeries = _logEntries?
                .GroupBy(e => new { e.LogLevel, MonthYear = new DateTime(e.TimeStamp.Year, e.TimeStamp.Month, 1) })
                .OrderBy(e => e.Key.LogLevel).ThenBy(e => e.Key.MonthYear)
                .Select(e => new ChartDataItem(e.Key.LogLevel, e.Key.MonthYear, e.Count()))
                .ToList();

            StateHasChanged();
        }
    }
}
