using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Context.Contexts;
using KerryShaleFanPage.Context.Entities;
using KerryShaleFanPage.Shared.Repositories;

namespace KerryShaleFanPage.Context.Repositories
{
    public class ConfigurationRepository : IGenericRepository<ConfigurationEntry>
    {
        private readonly ConfigurationDbContext _dbContext;

        public ConfigurationRepository(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc cref="IGenericRepository{ConfigurationEntry}" />
        public IList<ConfigurationEntry> GetAll()
        {
            if (_dbContext.ConfigurationEntries == null || !_dbContext.ConfigurationEntries.Any())
            {
                return new List<ConfigurationEntry>();
            }

            return _dbContext.ConfigurationEntries.ToList();
        }

        /// <inheritdoc cref="IGenericRepository{ConfigurationEntry}" />
        public ConfigurationEntry? GetLast()
        {
            if (_dbContext.ConfigurationEntries == null || !_dbContext.ConfigurationEntries.Any())
            {
                return null;
            }

            return _dbContext.ConfigurationEntries.OrderByDescending(entity => entity.Modified).FirstOrDefault();
        }

        /// <inheritdoc cref="IGenericRepository{ConfigurationEntry}" />
        public ConfigurationEntry? GetById(long id)
        {
            if (_dbContext.ConfigurationEntries == null || !_dbContext.ConfigurationEntries.Any())
            {
                return null;
            }

            return _dbContext.ConfigurationEntries.FirstOrDefault(entity => entity.Id == id);
        }

        /// <inheritdoc cref="IGenericRepository{ConfigurationEntry}" />
        public async Task<ConfigurationEntry?> UpsertAsync(ConfigurationEntry entity, CancellationToken cancellationToken = default)
        {
            if (_dbContext.ConfigurationEntries == null)
            {
                return entity;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var existing = GetById(entity.Id);
                if (existing == null)  // Insert
                {
                    entity.Created = DateTime.Now;
                    entity.Modified = DateTime.Now;
                    await _dbContext.ConfigurationEntries.AddAsync(entity, cancellationToken);
                }
                else // Update
                {
                    existing.Key = entity.Key;
                    existing.Value = entity.Value;
                    existing.IsPassword = entity.IsPassword;
                    existing.Salt = entity.Salt;
                    existing.Created = entity.Created;
                    existing.CreatedBy = entity.CreatedBy;
                    existing.Modified = DateTime.Now;
                    existing.ModifiedBy = entity.ModifiedBy;
                    _dbContext.ConfigurationEntries.Update(entity);
                }

                var success = (await _dbContext.SaveChangesAsync(cancellationToken)) > 0;
                if (success)
                {
                    await transaction.CommitAsync(cancellationToken);
                    return GetById(entity.Id);
                }
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
            }
            await transaction.RollbackAsync(cancellationToken);
            return entity;
        }

        /// <inheritdoc cref="IGenericRepository{ConfigurationEntry}" />
        public async Task<bool> DeleteByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            if (_dbContext.ConfigurationEntries == null || !_dbContext.ConfigurationEntries.Any())
            {
                return false;
            }

            var existing = GetById(id);
            if (existing == null)
            {
                return false;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                _dbContext.ConfigurationEntries.Remove(existing);
                var success = (await _dbContext.SaveChangesAsync(cancellationToken)) > 0;
                if (success)
                {
                    await transaction.CommitAsync(cancellationToken);
                    return true;
                }
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
            }
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }
    }
}
