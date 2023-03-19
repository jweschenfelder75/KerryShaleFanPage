using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using KerryShaleFanPage.Server.Interfaces.BusinessLogic;
using KerryShaleFanPage.Shared.Objects;

namespace KerryShaleFanPage.Server.Controllers
{
    [ApiController]
    [Route("webapi/[controller]")]
    public class GalleryController : ControllerBase
    {
        private readonly IGenericService<GalleryItemDto> _galleryService;

        private readonly ILogger<GalleryController> _logger;

        public GalleryController(ILogger<GalleryController> logger, IGenericService<GalleryItemDto> galleryService)
        {
            _galleryService = galleryService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<GalleryItemDto> Get()
        {
            return _galleryService.GetAll();
        }
    }
}