using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using Radzen;
using KerryShaleFanPage.Client.Services;
using KerryShaleFanPage.Shared.Enums;
using KerryShaleFanPage.Shared.Events;

namespace KerryShaleFanPage.Client.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject]
        protected IStringLocalizer<Resources.Translations> Translate { get; set; }

        [Inject]
        protected SignalRClientService? SignalRClientService { get; set; }

        private bool _serverStatusAlertVisible = true;

        private IconStyle _serverStatusIconStyle = IconStyle.Light;

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

        private void ServerStatusChanged(object? sender, ServerStatusEventArgs e)
        {
            switch (e.ServerStatus)
            {
                case ServerStatusEnum.Critical:
                    {
                        _serverStatusAlertVisible = true;
                        _serverStatusIconStyle = IconStyle.Danger;
                        _serverStatusBlinkingCssClass = string.Empty;
                        break;
                    }
                case ServerStatusEnum.Error:
                    {
                        _serverStatusAlertVisible = true;
                        _serverStatusIconStyle = IconStyle.Danger;
                        _serverStatusBlinkingCssClass = "blink";
                        break;
                    }
                case ServerStatusEnum.Warning:
                    {
                        _serverStatusAlertVisible = true;
                        _serverStatusIconStyle = IconStyle.Warning;
                        _serverStatusBlinkingCssClass = "blink";
                        break;
                    }
                default:
                    {
                        _serverStatusAlertVisible = true;
                        _serverStatusIconStyle = IconStyle.Light;
                        _serverStatusBlinkingCssClass = string.Empty;
                        break;
                    }
            }
            StateHasChanged();
        }
    }
}
