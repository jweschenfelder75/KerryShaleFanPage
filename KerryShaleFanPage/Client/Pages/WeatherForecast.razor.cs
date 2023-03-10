using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Client.Pages
{
    public partial class WeatherForecast
    {
        [Inject]
        protected HttpClient Http { get; set; }

        private WeatherForecastDto[]? forecasts;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            forecasts = await Http.GetFromJsonAsync<WeatherForecastDto[]>("webapi/WeatherForecast");
        }
    }
}
