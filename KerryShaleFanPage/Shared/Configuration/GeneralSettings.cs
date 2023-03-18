using System;

namespace KerryShaleFanPage.Shared.Configuration
{
    [Serializable]
    public class GeneralSettings
    {
        public bool OverrideCurrentGeneralSettings { get; set; }
        public string? DbUsername { get; set; }
        public string? DbPassword { get; set; }
        public EMailProviderConfiguration EMailProviderConfiguration { get; set; } = new EMailProviderConfiguration();
    }

    /// <summary>
    /// TODO: Put classes below in extra files. Let ReSharper do that job.
    /// </summary>

    [Serializable]
    public class EMailProviderConfiguration
    {
        public GMail GMail { get; set; } = new GMail();
        public Gmx Gmx { get; set; } = new Gmx();
    }

    [Serializable]
    public class GMail
    {
        public string? EMailUsername { get; set; }
        public string? EMailPassword { get; set; }
    }

    [Serializable]
    public class Gmx
    {
        public string? EMailUsername { get; set; }
        public string? EMailPassword { get; set; }
    }
}
