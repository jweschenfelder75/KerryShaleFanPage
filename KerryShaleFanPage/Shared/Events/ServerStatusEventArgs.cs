using KerryShaleFanPage.Shared.Enums;
using System;

namespace KerryShaleFanPage.Shared.Events
{
    public class ServerStatusEventArgs : EventArgs
    {
        public ServerStatusEnum ServerStatus { get; set; }
    }
}
