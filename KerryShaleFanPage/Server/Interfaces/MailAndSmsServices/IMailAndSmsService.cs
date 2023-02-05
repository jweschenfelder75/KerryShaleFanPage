using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Server.Interfaces.MailAndSmsServices;

public interface IMailAndSmsService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <param name="episode"></param>
    /// <returns></returns>
    public bool SendMailNotification(string from, string to, string subject, string? message, PodcastEpisodeDto? episode);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public bool SendMailNotification(string from, string to, string subject, string? message);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <param name="episode"></param>
    /// <returns></returns>
    public bool SendSmsNotification(string from, string to, string subject, string? message, PodcastEpisodeDto? episode);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="subject"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public bool SendSmsNotification(string from, string to, string subject, string? message);
}
