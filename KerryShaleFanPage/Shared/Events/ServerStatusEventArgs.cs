using System;
using KerryShaleFanPage.Shared.Enums;

namespace KerryShaleFanPage.Shared.Events
{
    public class ServerStatusEventArgs : EventArgs
    {
        public ServerStatusEnum ServerStatus { get; set; } = ServerStatusEnum.None;
    }
}
