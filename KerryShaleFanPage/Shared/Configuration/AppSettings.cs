using System;

namespace KerryShaleFanPage.Shared.Configuration
{
    [Serializable]
    public class AppSettings
    {
        public GeneralWebService? GeneralWebService { get; set; }
        public MobileWebService? MobileWebService { get; set; }
        public GeneralMaintainance? GeneralMaintainance { get; set; }
        public MobileMaintainance? MobileMaintainance { get; set; }
        public GeneralLogging? GeneralLogging { get; set; }
        public LoggingWebpage? LoggingWebpage { get; set; }
        public ContactFormWebpage? ContactFormWebpage { get; set; }
        public IsitrollingpodAtAcast? IsitrollingpodAtAcast { get; set; }
        public IsitrollingpodAtListenNotes? IsitrollingpodAtListenNotes { get; set; }
        public IsitrollingpodAtSpotify? IsitrollingpodAtSpotify { get; set; }
        public IsitrollingpodAtTwitter? IsitrollingpodAtTwitter { get; set; }
    }

    /// <summary>
    /// TODO: Put classes below in extra files.
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
