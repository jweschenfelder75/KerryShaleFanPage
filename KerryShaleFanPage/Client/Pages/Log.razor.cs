using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using KerryShaleFanPage.Shared.Objects;
using Radzen;

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

        private async Task ShowDialog(string title, string message)
        {
            await DialogService.Alert(message, title, new AlertOptions() { OkButtonText = "OK", CssClass="w-100", Style="max-width: 50% !important;" });
        }

        private async Task ShowLoading()
        {
            _isLoading = true;

            await Task.Yield();

            _isLoading = false;
        }
    }
}
