using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;
using Radzen;
using KerryShaleFanPage.Client.Services;
using KerryShaleFanPage.Shared.Enums;
using KerryShaleFanPage.Shared.Events;

namespace KerryShaleFanPage.Client.Shared
{
    public partial class MainLayout : LayoutComponentBase, IAsyncDisposable
    {
        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        [Inject]
        protected SignalRClientService? SignalRClientService { get; set; }

        private bool _serverStatusAlertVisible = false;

        private string _serverStatusIcon = "sync";

        private IconStyle _serverStatusIconStyle = IconStyle.Primary;

        private string _serverStatusBlinkingCssClass = string.Empty;

        private bool _sidebarExpanded = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (SignalRClientService != null)
            {
                SignalRClientService.ServerStatusEvent += ServerStatusChanged;
            }
        }

        private async void ServerStatusChanged(object? sender, ServerStatusEventArgs e)
        {
            switch (e.ServerStatus)
            {
                case ServerStatusEnum.Critical:
                    {
                        _serverStatusAlertVisible = true;
                        _serverStatusIcon = "warning";
                        _serverStatusIconStyle = IconStyle.Danger;
                        _serverStatusBlinkingCssClass = string.Empty;
                        break;
                    }
                case ServerStatusEnum.Error:
                    {
                        _serverStatusAlertVisible = true;
                        _serverStatusIcon = "warning";
                        _serverStatusIconStyle = IconStyle.Danger;
                        _serverStatusBlinkingCssClass = "blink";
                        break;
                    }
                case ServerStatusEnum.Warning:
                    {
                        _serverStatusAlertVisible = true;
                        _serverStatusIcon = "sync";
                        _serverStatusIconStyle = IconStyle.Warning;
                        _serverStatusBlinkingCssClass = "blink";
                        break;
                    }
                default:
                    {
                        _serverStatusAlertVisible = false;
                        _serverStatusIcon = "sync";
                        _serverStatusIconStyle = IconStyle.Primary;
                        _serverStatusBlinkingCssClass = string.Empty;
                        break;
                    }
            }
            await InvokeAsync(StateHasChanged);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
