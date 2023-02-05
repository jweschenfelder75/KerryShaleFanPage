using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using KerryShaleFanPage.Shared.Objects;
using System;

namespace KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices.ToDo
{
    public class GmxMailAndSmsService : IMailAndSmsService
    {
        public string UserName => "";
        public string UserPassword => "";

        private readonly ILogger<GmxMailAndSmsService> _logger;  // TODO: Implement logging!

        private const string _SMS_PREFIX = "[kerryshalefanpg-sms]";
        private const string _HOSTNAME = "mail.gmx.com";
        private const int _PORT = 587;

        /// <summary>
        /// 
        /// </summary>
        public GmxMailAndSmsService(ILogger<GmxMailAndSmsService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc cref="IMailAndSmsService"/>
        public bool SendMailNotification(string from, string to, string subject, string message, PodcastEpisodeDto? episode)
        {
            using var client = new SmtpClient
            {
                Host = _HOSTNAME,
                Port = _PORT,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(UserName, UserPassword)
            };

            var content = episode?.ToString() ?? string.Empty;
            using var mail = new MailMessage(from, to)
            {
                Subject = subject,
                Body = $"{message} {Environment.NewLine}{Environment.NewLine} {content}"
            };

            try
            {
                client.Send(mail);
                return true;

            }
            catch (SmtpException ex)
            {
                var smtpException = ex;  // TODO: Log exception!
            }

            return false;
        }

        /// <inheritdoc cref="IMailAndSmsService"/>
        public bool SendMailNotification(string from, string to, string subject, string message)
        {
            using var client = new SmtpClient
            {
                Host = _HOSTNAME,
                Port = _PORT,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(UserName, UserPassword)
            };

            using var mail = new MailMessage(from, to)
            {
                Subject = subject,
                Body = $"{message}"
            };

            try
            {
                client.Send(mail);
                return true;

            }
            catch (SmtpException ex)
            {
                var smtpException = ex;  // TODO: Log exception!
            }

            return false;
        }

        /// <inheritdoc cref="IMailAndSmsService"/>
        public bool SendSmsNotification(string from, string to, string subject, string message, PodcastEpisodeDto? episode)
        {
            // I cannot see at the moment that GMX supports it. Workaround: Send an e-mail and apply GMX filter rule then which sends an SMS.

            return SendMailNotification(from, to, $"{_SMS_PREFIX} {subject}", message, episode);
        }

        /// <inheritdoc cref="IMailAndSmsService"/>
        public bool SendSmsNotification(string from, string to, string subject, string message)
        {
            // I cannot see at the moment that GMX supports it. Workaround: Send an e-mail and apply GMX filter rule then which sends an SMS.

            return SendMailNotification(from, to, $"{_SMS_PREFIX} {subject}", message);
        }
    }
}
