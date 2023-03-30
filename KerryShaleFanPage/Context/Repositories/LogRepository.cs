using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using KerryShaleFanPage.Context.Contexts;
using KerryShaleFanPage.Context.Entities;
using KerryShaleFanPage.Shared.Repositories;

namespace KerryShaleFanPage.Context.Repositories
{
    public class LogRepository : IGenericRepository<LogEntry>
    {
        private readonly LogDbContext _dbContext;

        public LogRepository(LogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public string? GetConnectionString()
        {
            return _dbContext.Database.GetConnectionString();
        }

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public IList<LogEntry> GetAll()
        {
            if (_dbContext.LogEntries == null || !_dbContext.LogEntries.Any())
            {
                return new List<LogEntry>();
            }

            return _dbContext.LogEntries.OrderByDescending(entity => entity.TimeStamp).ToList();
        }

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public IList<LogEntry> GetAll(string connectionString)
        {
            using var dbContext = new LogDbContext(connectionString);
            if (dbContext.LogEntries == null || !dbContext.LogEntries.Any())
            {
                return new List<LogEntry>();
            }

            return dbContext.LogEntries.OrderByDescending(entity => entity.TimeStamp).ToList();
        }

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public LogEntry? GetLast()
        {
            if (_dbContext.LogEntries == null || !_dbContext.LogEntries.Any())
            {
                return null;
            }

            return _dbContext.LogEntries.OrderByDescending(entity => entity.TimeStamp).FirstOrDefault();
        }

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public LogEntry? GetLast(string connectionString)
        {
            using var dbContext = new LogDbContext(connectionString);
            if (dbContext.LogEntries == null || !dbContext.LogEntries.Any())
            {
                return null;
            }

            return dbContext.LogEntries.OrderByDescending(entity => entity.TimeStamp).FirstOrDefault();
        }

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public LogEntry? GetById(long id)
        {
            if (_dbContext.LogEntries == null || !_dbContext.LogEntries.Any())
            {
                return null;
            }

            return _dbContext.LogEntries.FirstOrDefault(entity => entity.Id == id);
        }

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public LogEntry? GetById(string connectionString, long id)
        {
            using var dbContext = new LogDbContext(connectionString);
            if (dbContext.LogEntries == null || !dbContext.LogEntries.Any())
            {
                return null;
            }

            return dbContext.LogEntries.FirstOrDefault(entity => entity.Id == id);
        }

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public IList<LogEntry> GetWhere(Expression<Func<LogEntry, bool>> expression)
        {
            if (_dbContext.LogEntries == null || !_dbContext.LogEntries.Any())
            {
                return new List<LogEntry>();
            }

            return _dbContext.LogEntries.Where(expression).ToList();
        }

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public IList<LogEntry> GetWhere(string connectionString, Expression<Func<LogEntry, bool>> expression)
        {
            using var dbContext = new LogDbContext(connectionString);
            if (dbContext.LogEntries == null || !dbContext.LogEntries.Any())
            {
                return new List<LogEntry>();
            }

            return dbContext.LogEntries.Where(expression).ToList();
        }

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public async Task<LogEntry?> UpsertAsync(LogEntry entity, CancellationToken cancellationToken = default)
        {
            if (_dbContext.LogEntries == null)
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
                    await _dbContext.LogEntries.AddAsync(entity, cancellationToken);
                }
                else // Update
                {
                    existing.TimeStamp = entity.TimeStamp;
                    existing.LogLevel = entity.LogLevel;
                    existing.Logger = entity.Logger;
                    existing.Message = entity.Exception;
                    existing.Exception = entity.Exception;
                    existing.Created = entity.Created;
                    existing.CreatedBy = entity.CreatedBy;
                    existing.Modified = DateTime.Now;
                    existing.ModifiedBy = entity.ModifiedBy;
                    _dbContext.LogEntries.Update(entity);
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

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public async Task<LogEntry?> UpsertAsync(string connectionString, LogEntry entity, CancellationToken cancellationToken = default)
        {
            using var dbContext = new LogDbContext(connectionString);
            if (dbContext.LogEntries == null)
            {
                return entity;
            }

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var existing = GetById(connectionString, entity.Id);
                if (existing == null)  // Insert
                {
                    entity.Created = DateTime.Now;
                    entity.Modified = DateTime.Now;
                    await dbContext.LogEntries.AddAsync(entity, cancellationToken);
                }
                else // Update
                {
                    existing.TimeStamp = entity.TimeStamp;
                    existing.LogLevel = entity.LogLevel;
                    existing.Logger = entity.Logger;
                    existing.Message = entity.Exception;
                    existing.Exception = entity.Exception;
                    existing.Created = entity.Created;
                    existing.CreatedBy = entity.CreatedBy;
                    existing.Modified = DateTime.Now;
                    existing.ModifiedBy = entity.ModifiedBy;
                    dbContext.LogEntries.Update(entity);
                }

                var success = (await dbContext.SaveChangesAsync(cancellationToken)) > 0;
                if (success)
                {
                    await transaction.CommitAsync(cancellationToken);
                    return GetById(connectionString, entity.Id);
                }
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
            }
            await transaction.RollbackAsync(cancellationToken);
            return entity;
        }

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public async Task<bool> DeleteByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            if (_dbContext.LogEntries == null || !_dbContext.LogEntries.Any())
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
                _dbContext.LogEntries.Remove(existing);
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

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public async Task<bool> DeleteByIdAsync(string connectionString, long id, CancellationToken cancellationToken = default)
        {
            using var dbContext = new LogDbContext(connectionString);
            if (dbContext.LogEntries == null || !dbContext.LogEntries.Any())
            {
                return false;
            }

            var existing = GetById(connectionString, id);
            if (existing == null)
            {
                return false;
            }

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                dbContext.LogEntries.Remove(existing);
                var success = (await dbContext.SaveChangesAsync(cancellationToken)) > 0;
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

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public async Task<bool> DeleteWhereAsync(Expression<Func<LogEntry, bool>> expression, CancellationToken cancellationToken = default)
        {
            if (_dbContext.LogEntries == null || !_dbContext.LogEntries.Any())
            {
                return false;
            }

            var existing = GetWhere(expression);
            if (existing == null || !existing.Any())
            {
                return false;
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                _dbContext.LogEntries.RemoveRange(existing);
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

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public async Task<bool> DeleteWhereAsync(string connectionString, Expression<Func<LogEntry, bool>> expression, CancellationToken cancellationToken = default)
        {
            using var dbContext = new LogDbContext(connectionString);
            if (dbContext.LogEntries == null || !dbContext.LogEntries.Any())
            {
                return false;
            }

            var existing = GetWhere(connectionString, expression);
            if (existing == null || !existing.Any())
            {
                return false;
            }

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                dbContext.LogEntries.RemoveRange(existing);
                var success = (await dbContext.SaveChangesAsync(cancellationToken)) > 0;
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
