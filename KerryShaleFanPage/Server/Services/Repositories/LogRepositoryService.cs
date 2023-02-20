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
    public class LogRepositoryService : IGenericRepositoryService<LogEntryDto>
    {
        private readonly ILogger<LogRepositoryService> _logger;  // TODO: Implement logging!
        private readonly IGenericRepository<LogEntry> _repository;

        private readonly Mapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        public LogRepositoryService(ILogger<LogRepositoryService> logger, IGenericRepository<LogEntry> repository)
        {
            _logger = logger;
            _repository = repository;
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<LogEntryDto, LogEntry>();
                cfg.CreateMap<LogEntry, LogEntryDto>();
            });
            _mapper = new Mapper(config);  // Maps between Dto and Entity and the other way around
        }

        /// <inheritdoc cref="IGenericRepositoryService{LogEntryDto}" />
        public IList<LogEntryDto> GetAll()
        {
            var result = _repository.GetAll();

            return !result.Any() 
                ? new List<LogEntryDto>() 
                : _mapper.Map<List<LogEntryDto>>(result);
        }

        /// <inheritdoc cref="IGenericRepositoryService{LogEntryDto}" />
        public LogEntryDto? GetLast()
        {
            var result = _repository.GetLast();

            return result == null 
                ? null 
                : _mapper.Map<LogEntryDto>(result);
        }

        /// <inheritdoc cref="IGenericRepositoryService{LogEntryDto}" />
        public LogEntryDto? GetById(long id)
        {
            var result = _repository.GetById(id);

            return result == null 
                ? null 
                : _mapper.Map<LogEntryDto>(result);
        }

        /// <inheritdoc cref="IGenericRepositoryService{LogEntryDto}" />
        public async Task<LogEntryDto?> UpsertAsync(LogEntryDto? dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
            {
                return null;
            }

            var entity = _mapper.Map<LogEntry>(dto);
            if (entity == null)
            {
                return null;
            }

            var result = await _repository.UpsertAsync(entity, cancellationToken);
            return _mapper.Map<LogEntryDto>(result);
        }

        /// <inheritdoc cref="IGenericRepositoryService{LogEntryDto}" />
        public async Task<bool> DeleteByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _repository.DeleteByIdAsync(id, cancellationToken);
        }
    }
}
