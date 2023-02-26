using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using KerryShaleFanPage.Server.Interfaces.Repositories;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        private readonly IGenericRepositoryService<LogEntryDto> _logRepositoryService;

        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger, IGenericRepositoryService<LogEntryDto> logRepositoryService)
        {
            _logRepositoryService = logRepositoryService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<LogEntryDto> Get()
        {
            return _logRepositoryService.GetAll();
        }
    }
}