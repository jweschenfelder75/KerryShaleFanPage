using System;

namespace KerryShaleFanPage.Shared.Events
{
    public class MaintenanceMessageEventArgs : EventArgs
    {
        public bool IsEnabled { get; set; } = false;

        public string Message { get; set; } = string.Empty;
    }
}
