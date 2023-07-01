using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;
using KerryShaleFanPage.Client.Services;
using KerryShaleFanPage.Shared.Events;

namespace KerryShaleFanPage.Client.Shared
{
    public partial class MaintenanceHeader : IAsyncDisposable
    {
        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        [Inject]
        protected SignalRClientService? SignalRClientService { get; set; }

        private string? _text;

        private bool _isEnabled;

        private bool _isMessageScrollEnabled;

        private bool _isVisible => (_isEnabled && !string.IsNullOrWhiteSpace(_text));

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (SignalRClientService != null)
            {
                SignalRClientService.MaintenanceMessageEvent += MaintenanceMessageReceived;
            }
        }

        private async void MaintenanceMessageReceived(object? sender, MaintenanceMessageEventArgs e)
        {
            _isEnabled = e.IsEnabled;
            _isMessageScrollEnabled = e.IsMessageScrollEnabled;
            _text = e.Message;
            await InvokeAsync(StateHasChanged);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
