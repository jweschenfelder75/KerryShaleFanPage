using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KerryShaleFanPage.Shared.Objects;
using KerryShaleFanPage.Shared.Repositories;
using KerryShaleFanPage.Server.Interfaces.Repositories;

namespace KerryShaleFanPage.Server.Services.Repositories
{
    public class PodcastEpisodeRepositoryService : IGenericRepositoryService<PodcastEpisodeDto>
    {
        private readonly ILogger<PodcastEpisodeRepositoryService> _logger;  // TODO: Implement logging!
        private readonly IGenericRepository<PodcastEpisode> _repository;

        private readonly Mapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        public PodcastEpisodeRepositoryService(ILogger<PodcastEpisodeRepositoryService> logger, IGenericRepository<PodcastEpisode> repository)
        {
            _logger = logger;
            _repository = repository;
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<PodcastEpisodeDto, PodcastEpisode>();
                cfg.CreateMap<PodcastEpisode, PodcastEpisodeDto>();
            });
            _mapper = new Mapper(config);  // Maps between Dto and Entity and the other way around
        }

        /// <inheritdoc cref="IGenericRepositoryService{PodcastEpisodeDto}" />
        public IList<PodcastEpisodeDto> GetAll()
        {
            var result = _repository.GetAll();

            return !result.Any() 
                ? new List<PodcastEpisodeDto>() 
                : _mapper.Map<List<PodcastEpisodeDto>>(result);
        }

        /// <inheritdoc cref="IGenericRepositoryService{PodcastEpisodeDto}" />
        public PodcastEpisodeDto? GetLast()
        {
            var result = _repository.GetLast();

            return result == null 
                ? null 
                : _mapper.Map<PodcastEpisodeDto>(result);
        }

        /// <inheritdoc cref="IGenericRepositoryService{PodcastEpisodeDto}" />
        public PodcastEpisodeDto? GetById(long id)
        {
            var result = _repository.GetById(id);

            return result == null 
                ? null 
                : _mapper.Map<PodcastEpisodeDto>(result);
        }

        /// <inheritdoc cref="IGenericRepositoryService{PodcastEpisodeDto}" />
        public async Task<PodcastEpisodeDto?> UpsertAsync(PodcastEpisodeDto? dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
            {
                return null;
            }

            var entity = _mapper.Map<PodcastEpisode>(dto);
            if (entity == null)
            {
                return null;
            }

            var result = await _repository.UpsertAsync(entity, cancellationToken);
            return _mapper.Map<PodcastEpisodeDto>(result);
        }

        /// <inheritdoc cref="IGenericRepositoryService{PodcastEpisodeDto}" />
        public async Task<bool> DeleteByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _repository.DeleteByIdAsync(id, cancellationToken);
        }
    }
}
