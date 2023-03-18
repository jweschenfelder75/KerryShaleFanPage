using System;

namespace KerryShaleFanPage.Shared.Configuration
{
    [Serializable]
    public class AppSettings
    {
        public GeneralWebService? GeneralWebService { get; set; } = new GeneralWebService();
        public MobileWebService? MobileWebService { get; set; } = new MobileWebService();
        public GeneralMaintainance? GeneralMaintainance { get; set; } = new GeneralMaintainance();
        public MobileMaintainance? MobileMaintainance { get; set; } = new MobileMaintainance();
        public GeneralLogging? GeneralLogging { get; set; } = new GeneralLogging();
        public LoggingWebpage? LoggingWebpage { get; set; } = new LoggingWebpage();
        public ContactFormWebpage? ContactFormWebpage { get; set; } = new ContactFormWebpage();
        public IsitrollingpodAtAcast? IsitrollingpodAtAcast { get; set; } = new IsitrollingpodAtAcast();
        public IsitrollingpodAtListenNotes? IsitrollingpodAtListenNotes { get; set; } = new IsitrollingpodAtListenNotes();
        public IsitrollingpodAtSpotify? IsitrollingpodAtSpotify { get; set; } = new IsitrollingpodAtSpotify();
        public IsitrollingpodAtTwitter? IsitrollingpodAtTwitter { get; set; } = new IsitrollingpodAtTwitter();
    }

    /// <summary>
    /// TODO: Put classes below in extra files. Let ReSharper do that job.
    /// </summary>

    [Serializable]
    public class ContactFormWebpage
    {
        public bool Enabled { get; set; }
    }

    [Serializable]
    public class GeneralLogging
    {
        public bool DeleteLogsEnabled { get; set; }
        public int DeleteLogsAfterDays { get; set; }
    }

    [Serializable]
    public class GeneralMaintainance
    {
        public bool Enabled { get; set; }
        public string? Message { get; set; }
        public bool MessageScrollEnabled { get; set; }
    }

    [Serializable]
    public class GeneralWebService
    {
        public bool Enabled { get; set; }
        public bool SwaggerEnabled { get; set; }
    }

    [Serializable]
    public class IsitrollingpodAtAcast
    {
        public bool Backup { get; set; }
        public bool CrawlEnabled { get; set; }
        public int CrawlEveryHours { get; set; }
        public bool MailNotificationEnabled { get; set; }
        public string? MailAddressesSemikolonSeparated { get; set; }
        public bool ImageDownloadUploadEnabled { get; set; }
    }

    [Serializable]
    public class IsitrollingpodAtListenNotes
    {
        public bool Backup { get; set; }
        public bool CrawlEnabled { get; set; }
        public int CrawlEveryHours { get; set; }
        public bool MailNotificationEnabled { get; set; }
        public string? MailAddressesSemikolonSeparated { get; set; }
        public bool ImageDownloadUploadEnabled { get; set; }
    }

    [Serializable]
    public class IsitrollingpodAtSpotify
    {
        public bool Backup { get; set; }
        public bool CrawlEnabled { get; set; }
        public int CrawlEveryHours { get; set; }
        public bool MailNotificationEnabled { get; set; }
        public string? MailAddressesSemikolonSeparated { get; set; }
        public bool ImageDownloadUploadEnabled { get; set; }
    }

    [Serializable]
    public class IsitrollingpodAtTwitter
    {
        public bool Backup { get; set; }
        public bool CrawlEnabled { get; set; }
        public int CrawlEveryHours { get; set; }
        public bool MailNotificationEnabled { get; set; }
        public string? MailAddressesSemikolonSeparated { get; set; }
        public bool ImageDownloadUploadEnabled { get; set; }
    }

    [Serializable]
    public class LoggingWebpage
    {
        public bool Enabled { get; set; }
        public bool MaximumLogEntriesEnabled { get; set; }
        public int MaximumLogEntriesCount { get; set; }
    }

    [Serializable]
    public class MobileMaintainance
    {
        public bool Enabled { get; set; }
        public string? Message { get; set; }
        public bool MessageScrollEnabled { get; set; }
    }

    [Serializable]
    public class MobileWebService
    {
        public bool Enabled { get; set; }
        public bool SwaggerEnabled { get; set; }
    }
}
