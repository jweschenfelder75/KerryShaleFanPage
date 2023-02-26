using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class Log
    {
        [Inject]
        protected HttpClient Http { get; set; }

        private IList<LogEntryDto>? _logEntries;

        bool _isLoading = false;

        private async Task ShowLoading()
        {
            _isLoading = true;

            await Task.Yield();

            _isLoading = false;
        }
        
        protected override async Task OnInitializedAsync()
        {
            var data = await Http.GetFromJsonAsync<LogEntryDto[]>("Log");
            _logEntries = data?.ToList();
        }
    }
}
