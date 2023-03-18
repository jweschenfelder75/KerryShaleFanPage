using System;

namespace KerryShaleFanPage.Shared.Objects
{
    [Serializable]
    public class ContactDataDto
    {
        public string Name { get; set; } = string.Empty;

        public string EMail { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}
