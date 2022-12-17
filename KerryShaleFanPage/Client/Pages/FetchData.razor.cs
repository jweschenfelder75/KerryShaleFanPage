using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using KerryShaleFanPage.Shared;
using Microsoft.AspNetCore.Components;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class FetchData
    {
        [Inject]
        protected HttpClient Http { get; set; }

        private WeatherForecast[]? forecasts;

        protected override async Task OnInitializedAsync()
        {
            forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
        }
    }
}
