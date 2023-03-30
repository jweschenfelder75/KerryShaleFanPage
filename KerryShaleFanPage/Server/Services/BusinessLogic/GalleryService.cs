using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KerryShaleFanPage.Server.Interfaces.BusinessLogic;
using KerryShaleFanPage.Server.Interfaces.Security;
using KerryShaleFanPage.Shared.Objects;
using KerryShaleFanPage.Shared.Configuration;

namespace KerryShaleFanPage.Server.Services.BusinessLogic
{
    public class GalleryService : IGenericService<GalleryItemDto>
    {
        private readonly ISecuredConfigurationService _securedConfigurationService;
        private readonly Mapper _mapper;

        private readonly ILogger<GalleryService> _logger;  // TODO: Implement logging!

        public GalleryService(ILogger<GalleryService> logger, ISecuredConfigurationService securedConfigurationService)
        {
            _logger = logger;
            _securedConfigurationService = securedConfigurationService;
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<GalleryItemDto, GalleryItem>();
                cfg.CreateMap<GalleryItem, GalleryItemDto>();
            });
            _mapper = new Mapper(config);  // Maps properties between Dto and Object and the other way around
        }

        public IList<GalleryItemDto> GetAll()
        {
            var result = _securedConfigurationService.GetCachedGallerySettingsConfigurationFromFile();

            return (!result.GalleryItems.Any())
            ? new List<GalleryItemDto>()
                : _mapper.Map<List<GalleryItemDto>>(result.GalleryItems);
        }
    }
}
