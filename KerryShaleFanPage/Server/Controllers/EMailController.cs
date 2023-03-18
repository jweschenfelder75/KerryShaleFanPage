using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using KerryShaleFanPage.Server.Interfaces.MailAndSmsServices;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Server.Controllers
{
    [ApiController]
    [Route("webapi/[controller]")]
    public class EMailController : ControllerBase
    {
        private readonly IMailAndSmsService _mailAndSmsService;

        private readonly ILogger<EMailController> _logger;

        public EMailController(ILogger<EMailController> logger, IMailAndSmsService mailAndSmsService)
        {
            _mailAndSmsService = mailAndSmsService;
            _logger = logger;
        }

        [HttpPost]
        public bool Post([FromBody] ContactDataDto contactData)
        {
            return _mailAndSmsService.SendMailNotification(contactData.EMail, "<MailAddress>", $"[kerryshalefanpage] {contactData.Subject}",  // TODO: Make configurable and encrypt!
                $"Name: {contactData.Name}{Environment.NewLine}E-mail: {contactData.EMail}{Environment.NewLine}Category: {contactData.Category}{Environment.NewLine}Subject: {contactData.Subject}{Environment.NewLine}Body: {contactData.Message}{Environment.NewLine}Timestamp: {DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)}");
        }
    }
}