using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Client.Services
{
    public class BrowserService
    {
        private readonly IJSRuntime _jsRuntime;

        public BrowserService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<BrowserDimension> GetDimensions()
        {
            return await _jsRuntime.InvokeAsync<BrowserDimension>("getDimensions");
        }

    }

    public class BrowserDimension
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
