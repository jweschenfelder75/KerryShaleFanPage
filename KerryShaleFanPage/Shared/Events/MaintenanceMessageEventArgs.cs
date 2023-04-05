using System;

namespace KerryShaleFanPage.Shared.Events
{
    public class MaintenanceMessageEventArgs : EventArgs
    {
        public bool IsEnabled { get; set; }

        public bool IsMessageScrollEnabled { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
