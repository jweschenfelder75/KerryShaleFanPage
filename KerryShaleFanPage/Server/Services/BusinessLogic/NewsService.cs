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
    public class NewsService : IGenericService<NewsItemDto>
    {
        private readonly ISecuredConfigurationService _securedConfigurationService;
        private readonly Mapper _mapper;

        private readonly ILogger<NewsService> _logger;  // TODO: Implement logging!

        public NewsService(ILogger<NewsService> logger, ISecuredConfigurationService securedConfigurationService)
        {
            _logger = logger;
            _securedConfigurationService = securedConfigurationService;
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<NewsItemDto, NewsItem>();
                cfg.CreateMap<NewsItem, NewsItemDto>();
            });
            _mapper = new Mapper(config);  // Maps properties between Dto and Object and the other way around
        }

        public IList<NewsItemDto> GetAll()
        {
            var result = _securedConfigurationService.GetCurrentNewsSettingsConfigurationFromFile();

            return (!result.NewsItems.Any())
            ? new List<NewsItemDto>()
                : _mapper.Map<List<NewsItemDto>>(result.NewsItems);
        }
    }
}
