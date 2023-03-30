using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using KerryShaleFanPage.Context.Entities;
using KerryShaleFanPage.Shared.Objects;
using KerryShaleFanPage.Shared.Repositories;
using KerryShaleFanPage.Server.Interfaces.Repositories;
using KerryShaleFanPage.Server.Interfaces.Security;
using KerryShaleFanPage.Shared.Configuration;
using System;

namespace KerryShaleFanPage.Server.Services.Repositories
{
    public class LogRepositoryService : IGenericRepositoryService<LogEntryDto>
    {
        private readonly IGenericRepository<LogEntry> _repository;
        private readonly ISecuredConfigurationService _securedConfigurationService;
        private readonly GeneralSettings _settings;
        private readonly Mapper _mapper;

        private readonly ILogger<LogRepositoryService> _logger;  // TODO: Implement logging!

        /// <summary>
        /// 
        /// </summary>
        public LogRepositoryService(ILogger<LogRepositoryService> logger, IGenericRepository<LogEntry> repository,
            ISecuredConfigurationService securedConfigurationService)
        {
            _logger = logger;
            _repository = repository;
            _securedConfigurationService = securedConfigurationService;
            _settings = _securedConfigurationService.GetDecryptedConfigurationForSettingsFromEncryptedFile();
            var config = new MapperConfiguration(cfg => {
                cfg.AddExpressionMapping();
                cfg.CreateMap<LogEntryDto, LogEntry>();
                cfg.CreateMap<LogEntry, LogEntryDto>();
            });
            _mapper = new Mapper(config);  // Maps properties between Dto and Entity and the other way around
        }

        /// <inheritdoc cref="IGenericRepositoryService{LogEntryDto}" />
        public IList<LogEntryDto> GetAll()
        {
            var connectionString = GetConnectionString();

            var result = (!string.IsNullOrEmpty(connectionString))
                ? _repository.GetAll(connectionString)
                : _repository.GetAll();

            return !result.Any() 
                ? new List<LogEntryDto>() 
                : _mapper.Map<List<LogEntryDto>>(result);
        }

        /// <inheritdoc cref="IGenericRepositoryService{LogEntryDto}" />
        public LogEntryDto? GetLast()
        {
            var connectionString = GetConnectionString();

            var result = (!string.IsNullOrEmpty(connectionString))
                ? _repository.GetLast(connectionString)
                : _repository.GetLast();

            return (result != null) 
                ? _mapper.Map<LogEntryDto>(result)
                : null;
        }

        /// <inheritdoc cref="IGenericRepositoryService{LogEntryDto}" />
        public LogEntryDto? GetById(long id)
        {
            var connectionString = GetConnectionString();

            var result = (!string.IsNullOrEmpty(connectionString))
                ? _repository.GetById(connectionString, id)
                : _repository.GetById(id);

            return (result != null) 
                ? _mapper.Map<LogEntryDto>(result)
                : null;
        }

        /// <inheritdoc cref="IGenericRepositoryService{LogEntryDto}" />
        public IList<LogEntryDto> GetWhere(Expression<Func<LogEntryDto, bool>> expression)
        {
            if (expression == null)
            {
                return new List<LogEntryDto>();
            }

            var mappedExpression = _mapper.Map<Expression<Func<LogEntry, bool>>>(expression);

            var connectionString = GetConnectionString();
            return (!string.IsNullOrEmpty(connectionString))
            ? _mapper.Map<IList<LogEntryDto>>(_repository.GetWhere(connectionString, mappedExpression))
                : _mapper.Map<IList<LogEntryDto>>(_repository.GetWhere(mappedExpression));
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

            var connectionString = GetConnectionString();

            var result = (!string.IsNullOrEmpty(connectionString))
                ? await _repository.UpsertAsync(connectionString, entity, cancellationToken)
                : await _repository.UpsertAsync(entity, cancellationToken);

            return _mapper.Map<LogEntryDto>(result);
        }

        /// <inheritdoc cref="IGenericRepositoryService{LogEntryDto}" />
        public async Task<bool> DeleteByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var connectionString = GetConnectionString();

            return (!string.IsNullOrEmpty(connectionString))
                ? await _repository.DeleteByIdAsync(connectionString, id, cancellationToken)
                : await _repository.DeleteByIdAsync(id, cancellationToken);
        }

        /// <inheritdoc cref="IGenericRepositoryService{LogEntryDto}" />
        public async Task<bool> DeleteWhereAsync(Expression<Func<LogEntryDto, bool>> expression, CancellationToken cancellationToken = default)
        {
            if (expression == null)
            {
                return false;
            }

            var mappedExpression = _mapper.Map<Expression<Func<LogEntry, bool>>>(expression);

            var connectionString = GetConnectionString();
            return (!string.IsNullOrEmpty(connectionString))
            ? await _repository.DeleteWhereAsync(connectionString, mappedExpression, cancellationToken)
                : await _repository.DeleteWhereAsync(mappedExpression, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string? GetConnectionString()
        {
            if (string.IsNullOrWhiteSpace(_repository.GetConnectionString()) || string.IsNullOrWhiteSpace(_settings.DbUsername) || string.IsNullOrWhiteSpace(_settings.DbPassword))
            {
                return null;
            }

            return (_repository.GetConnectionString() ?? string.Empty)
                .Replace("{username}", _settings.DbUsername)
                .Replace("{password}", _settings.DbPassword);
        }
    }
}
