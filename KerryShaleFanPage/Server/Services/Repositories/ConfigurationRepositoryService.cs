using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KerryShaleFanPage.Context.Entities;
using KerryShaleFanPage.Shared.Objects;
using KerryShaleFanPage.Shared.Repositories;
using KerryShaleFanPage.Server.Interfaces.Repositories;

namespace KerryShaleFanPage.Server.Services.Repositories
{
    public class ConfigurationRepositoryService : IGenericRepositoryService<ConfigurationEntryDto>
    {
        private readonly ILogger<ConfigurationRepositoryService> _logger;  // TODO: Implement logging!
        private readonly IGenericRepository<ConfigurationEntry> _repository;

        private readonly Mapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        public ConfigurationRepositoryService(ILogger<ConfigurationRepositoryService> logger, IGenericRepository<ConfigurationEntry> repository)
        {
            _logger = logger;
            _repository = repository;
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ConfigurationEntryDto, ConfigurationEntry>();
                cfg.CreateMap<ConfigurationEntry, ConfigurationEntryDto>();
            });
            _mapper = new Mapper(config);  // Maps between Dto and Entity and the other way around
        }

        /// <inheritdoc cref="IGenericRepositoryService{ConfigurationDto}" />
        public IList<ConfigurationEntryDto> GetAll()
        {
            var result = _repository.GetAll();

            return !result.Any()
                ? new List<ConfigurationEntryDto>()
                : _mapper.Map<List<ConfigurationEntryDto>>(result);
        }

        /// <inheritdoc cref="IGenericRepositoryService{ConfigurationDto}" />
        public ConfigurationEntryDto? GetLast()
        {
            var result = _repository.GetLast();

            return result == null
                ? null
                : _mapper.Map<ConfigurationEntryDto>(result);
        }

        /// <inheritdoc cref="IGenericRepositoryService{ConfigurationDto}" />
        public ConfigurationEntryDto? GetById(long id)
        {
            var result = _repository.GetById(id);

            return result == null
                ? null
                : _mapper.Map<ConfigurationEntryDto>(result);
        }

        /// <inheritdoc cref="IGenericRepositoryService{ConfigurationDto}" />
        public async Task<ConfigurationEntryDto?> UpsertAsync(ConfigurationEntryDto? dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
            {
                return null;
            }

            var entity = _mapper.Map<ConfigurationEntry>(dto);
            if (entity == null)
            {
                return null;
            }

            var result = await _repository.UpsertAsync(entity, cancellationToken);
            return _mapper.Map<ConfigurationEntryDto>(result);
        }

        /// <inheritdoc cref="IGenericRepositoryService{ConfigurationDto}" />
        public async Task<bool> DeleteByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _repository.DeleteByIdAsync(id, cancellationToken);
        }
    }
}
