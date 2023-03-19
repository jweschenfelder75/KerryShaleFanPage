using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Radzen;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class Log
    {
        [Inject]
        protected HttpClient Http { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        private IList<LogEntryDto>? _logEntries;

        bool _isLoading = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var data = await Http.GetFromJsonAsync<LogEntryDto[]>("webapi/Log");
            _logEntries = data?.ToList();
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
    }
}
