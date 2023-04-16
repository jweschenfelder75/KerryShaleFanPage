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
        private readonly IGmailMailAndSmsService _gmailMailAndSmsService;
        private readonly IGmxMailAndSmsService _gmxMailAndSmsService;

        private readonly ILogger<EMailController> _logger;

        public EMailController(ILogger<EMailController> logger, IGmailMailAndSmsService mailAndSmsService, IGmxMailAndSmsService gmxMailAndSmsService)
        {
            _gmailMailAndSmsService = mailAndSmsService;
            _gmxMailAndSmsService = gmxMailAndSmsService;
            _logger = logger;
        }

        [HttpPost]
        public bool Post([FromBody] ContactDataDto contactData)
        {
            return _gmailMailAndSmsService.SendMailNotification(string.Empty, string.Empty, $"[kerryshalefanpage] {contactData.Subject}",
                $"Name: {contactData.Name}{Environment.NewLine}E-mail: {contactData.EMail}{Environment.NewLine}Category: {contactData.Category}{Environment.NewLine}Subject: {contactData.Subject}{Environment.NewLine}Body: {contactData.Message}{Environment.NewLine}Timestamp: {DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)}");
        }


        [HttpGet, Route("/webapi/[controller]/mailtest")]
        public bool Test()
        {
            var success1 = _gmxMailAndSmsService.SendMailNotification(string.Empty, string.Empty, $"[kerryshalefanpage] Test",
                $"Name: Test Name{Environment.NewLine}E-mail: Test Mail{Environment.NewLine}Category: Test Category{Environment.NewLine}Subject: Test Subject{Environment.NewLine}Body: Test Body{Environment.NewLine}Timestamp: {DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)}");

            var success2 = _gmailMailAndSmsService.SendMailNotification(string.Empty, string.Empty, $"[kerryshalefanpage] Test",
                $"Name: Test Name{Environment.NewLine}E-mail: Test Mail{Environment.NewLine}Category: Test Category{Environment.NewLine}Subject: Test Subject{Environment.NewLine}Body: Test Body{Environment.NewLine}Timestamp: {DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)}");

            return success1 && success2;
        }
    }
}