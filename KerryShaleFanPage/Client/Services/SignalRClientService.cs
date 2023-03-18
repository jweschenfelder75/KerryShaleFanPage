using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using KerryShaleFanPage.Shared.Enums;
using KerryShaleFanPage.Shared.Events;

namespace KerryShaleFanPage.Client.Services
{
    public class SignalRClientService
    {
        private readonly NavigationManager _navigationManager;
        private HubConnection? _hubConnection;
        private const int _MAX_TRIES = 360;

        public event EventHandler<ServerStatusEventArgs>? ServerStatusEvent;
        public event EventHandler<MaintenanceMessageEventArgs>? MaintenanceMessageEvent;

        public SignalRClientService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            OpenSignalRHubAsync();
        }

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public async void OpenSignalRHubAsync(CancellationToken cancellationToken = default)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/signalrhub"))
                .WithAutomaticReconnect()
                .Build();
            try
            {
                await _hubConnection.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hub connection could not be established, {ex.Message}, please contact a web administrator.");
            }

            _hubConnection.On<string>("ReceiveMaintenanceMessage", (message) => OnReceiveMaintenanceMessage(message));

            _hubConnection.Closed += async (error) =>
            {
                // Do what you need to do ...
                // e.g. 1) Inject this service into your razor component
                //      2) Raise an event from here that connection closed
                //      3) Listen for event in razor component
                //      4) Tell user that connection is closed.

                // You could then try to reinitialize the connection here
                // and raise and event that connection is reestablished.

                Console.WriteLine("Hub connection interrupted.");
                ServerStatusEvent?.Invoke(this, new ServerStatusEventArgs() { ServerStatus = ServerStatusEnum.Error });

                var currentTry = 0;
                while (!IsConnected && currentTry < _MAX_TRIES) 
                {
                    try
                    {
                        await _hubConnection.StartAsync(cancellationToken);
                        if (IsConnected)
                        {
                            Console.WriteLine("Hub connection reestablished.");
                            ServerStatusEvent?.Invoke(this, new ServerStatusEventArgs() { ServerStatus = ServerStatusEnum.Ok });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Try {(currentTry + 1)} of {_MAX_TRIES}: Hub connection could not be reestablished, {ex.Message}.");
                        ServerStatusEvent?.Invoke(this, new ServerStatusEventArgs() { ServerStatus = ServerStatusEnum.Error });
                    }
                    await Task.Delay(10000);
                    currentTry++;
                }

                if (currentTry == _MAX_TRIES)
                {
                    Console.WriteLine("Hub connection lost, web server seems to be down, please try a refresh or contact a web administrator.");
                    ServerStatusEvent?.Invoke(this, new ServerStatusEventArgs() { ServerStatus = ServerStatusEnum.Critical });
                }
            };

            _hubConnection.Reconnecting += (connectionId) =>
            {
                Console.WriteLine("Trying to reestablish Hub connection.");
                ServerStatusEvent?.Invoke(this, new ServerStatusEventArgs() { ServerStatus = ServerStatusEnum.Warning });
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += (connectionId) =>
            {
                Console.WriteLine("Hub connection reestablished.");
                ServerStatusEvent?.Invoke(this, new ServerStatusEventArgs() { ServerStatus = ServerStatusEnum.Ok });
                return Task.CompletedTask;
            };

            _hubConnection.Closed += (connectionId) =>
            {
                Console.WriteLine("Hub connection closed, please try a refresh or contact a web administrator.");
                ServerStatusEvent?.Invoke(this, new ServerStatusEventArgs() { ServerStatus = ServerStatusEnum.Critical });
                return Task.CompletedTask;
            };
        }

        private void OnReceiveMaintenanceMessage(string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            var args = JsonConvert.DeserializeObject<MaintenanceMessageEventArgs>(message);
            if (args == null)
            {
                return;
            }
            MaintenanceMessageEvent?.Invoke(this, new MaintenanceMessageEventArgs() { IsEnabled = args.IsEnabled, Message = args.Message });
        }
    }
}
