using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using KerryShaleFanPage.Server.Interfaces.MailAndSmsServices;
using KerryShaleFanPage.Server.Interfaces.Security;
using KerryShaleFanPage.Shared.Configuration;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Server.Services.MailAndSmsServices
{
    public class GmailMailAndSmsService : IGmailMailAndSmsService
    {
        private readonly ISecuredConfigurationService _securedConfigurationService;
        private readonly GeneralSettings _settings;
        private readonly AppSettings _appSettings;

        private const string _SMS_PREFIX = "[kerryshalefanpg-sms]";
        private const string _HOSTNAME = "smtp.gmail.com";
        private const int _PORT = 587;

        private readonly ILogger<GmailMailAndSmsService> _logger;  // TODO: Implement logging!

        /// <summary>
        /// 
        /// </summary>
        public GmailMailAndSmsService(ILogger<GmailMailAndSmsService> logger, ISecuredConfigurationService securedConfigurationService)
        {
            _logger = logger;
            _securedConfigurationService = securedConfigurationService;
            _settings = _securedConfigurationService.GetDecryptedConfigurationForSettingsFromEncryptedFile();
            _appSettings = _securedConfigurationService.GetCurrentAppSettingsConfigurationFromFile();
        }

        /// <inheritdoc cref="IMailAndSmsService"/>
        public bool SendMailNotification(string from, string to, string subject, string? message, PodcastEpisodeDto? episode)
        {
            if (string.IsNullOrWhiteSpace(from))
            {
                from = _settings.EMailProviderConfiguration.GMail.EMailUsername ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(to))
            {
                to = _appSettings.ContactFormWebpage.MailAddressesSemikolonSeparated ?? string.Empty;
            }

            using var client = new SmtpClient
            {
                Timeout = 60000,
                Host = _HOSTNAME,
                Port = _PORT,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.EMailProviderConfiguration.GMail.EMailUsername, _settings.EMailProviderConfiguration.GMail.EMailPassword)
            };

            var content = episode?.ToString() ?? string.Empty;
            using var mail = new MailMessage(from, to)
            {
                Subject = subject,
                Body = (!string.IsNullOrWhiteSpace(message))
                    ? $"{message}{Environment.NewLine}{Environment.NewLine}{content}"
                    : content
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
        public bool SendMailNotification(string from, string to, string subject, string? message)
        {
            if (string.IsNullOrWhiteSpace(from))
            {
                from = _settings.EMailProviderConfiguration.GMail.EMailUsername ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(to))
            {
                to = _appSettings.ContactFormWebpage.MailAddressesSemikolonSeparated ?? string.Empty;
            }

            using var client = new SmtpClient
            {
                Timeout = 60000,
                Host = _HOSTNAME,
                Port = _PORT,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.EMailProviderConfiguration.GMail.EMailUsername, _settings.EMailProviderConfiguration.GMail.EMailPassword)
            };

            using var mail = new MailMessage(from, to)
            {
                Subject = subject,
                Body = message ?? string.Empty
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
        public bool SendSmsNotification(string from, string to, string subject, string? message, PodcastEpisodeDto? episode)
        {
            // I cannot see at the moment that GMX supports it. Workaround: Send an e-mail and apply GMX filter rule then which sends an SMS.

            return SendMailNotification(from, to, $"{_SMS_PREFIX} {subject}", message, episode);
        }

        /// <inheritdoc cref="IMailAndSmsService"/>
        public bool SendSmsNotification(string from, string to, string subject, string? message)
        {
            // I cannot see at the moment that GMX supports it. Workaround: Send an e-mail and apply GMX filter rule then which sends an SMS.

            return SendMailNotification(from, to, $"{_SMS_PREFIX} {subject}", message);
        }
    }
}
