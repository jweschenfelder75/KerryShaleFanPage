using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KerryShaleFanPage.Server.Interfaces.Repositories;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Server.Controllers
{
    [ApiController]
    [Route("webapi/[controller]")]
    public class PodcastController : ControllerBase
    {
        private readonly IGenericRepositoryService<PodcastEpisodeDto> _podcastRepositoryService;

        private readonly ILogger<PodcastController> _logger;

        public PodcastController(ILogger<PodcastController> logger, IGenericRepositoryService<PodcastEpisodeDto> podcastRepositoryService)
        {
            _podcastRepositoryService = podcastRepositoryService;
            _logger = logger;
        }

        [HttpGet]
        public PodcastEpisodeDto? Get()
        {
            return _podcastRepositoryService.GetLast();
        }
    }
}