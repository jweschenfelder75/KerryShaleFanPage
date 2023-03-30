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
    public class PodcastEpisodeRepository : IGenericRepository<PodcastEpisode>
    {
        private readonly PodcastEpisodeDbContext _dbContext;

        public PodcastEpisodeRepository(PodcastEpisodeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc cref="IGenericRepository{PodcastEpisode}" />
        public string? GetConnectionString()
        {
            return _dbContext.Database.GetConnectionString();
        }

        /// <inheritdoc cref="IGenericRepository{PodcastEpisode}" />
        public IList<PodcastEpisode> GetAll()
        {
            if (_dbContext.PodcastEpisodes == null || !_dbContext.PodcastEpisodes.Any())
            {
                return new List<PodcastEpisode>();
            }

            return _dbContext.PodcastEpisodes.OrderByDescending(entity => entity.Date).ToList();
        }

        /// <inheritdoc cref="IGenericRepository{LogEntry}" />
        public IList<PodcastEpisode> GetAll(string connectionString)
        {
            using var dbContext = new PodcastEpisodeDbContext(connectionString);
            if (dbContext.PodcastEpisodes == null || !dbContext.PodcastEpisodes.Any())
            {
                return new List<PodcastEpisode>();
            }

            return dbContext.PodcastEpisodes.OrderByDescending(entity => entity.Date).ToList();
        }

        /// <inheritdoc cref="IGenericRepository{PodcastEpisode}" />
        public PodcastEpisode? GetLast()
        {
            if (_dbContext.PodcastEpisodes == null || !_dbContext.PodcastEpisodes.Any())
            {
                return null;
            }

            return _dbContext.PodcastEpisodes.OrderByDescending(entity => entity.Date).FirstOrDefault();
        }

        /// <inheritdoc cref="IGenericRepository{PodcastEpisode}" />
        public PodcastEpisode? GetLast(string connectionString)
        {
            using var dbContext = new PodcastEpisodeDbContext(connectionString);
            if (dbContext.PodcastEpisodes == null || !dbContext.PodcastEpisodes.Any())
            {
                return null;
            }

            return dbContext.PodcastEpisodes.OrderByDescending(entity => entity.Date).FirstOrDefault();
        }

        /// <inheritdoc cref="IGenericRepository{PodcastEpisode}" />
        public PodcastEpisode? GetById(long id)
        {
            if (_dbContext.PodcastEpisodes == null || !_dbContext.PodcastEpisodes.Any())
            {
                return null;
            }

            return _dbContext.PodcastEpisodes.FirstOrDefault(entity => entity.Id == id);
        }

        /// <inheritdoc cref="IGenericRepository{PodcastEpisode}" />
        public PodcastEpisode? GetById(string connectionString, long id)
        {
            using var dbContext = new PodcastEpisodeDbContext(connectionString);
            if (dbContext.PodcastEpisodes == null || !dbContext.PodcastEpisodes.Any())
            {
                return null;
            }

            return dbContext.PodcastEpisodes.FirstOrDefault(entity => entity.Id == id);
        }

        /// <inheritdoc cref="IGenericRepository{PodcastEpisode}" />
        public IList<PodcastEpisode> GetWhere(Expression<Func<PodcastEpisode, bool>> expression)
        {
            if (_dbContext.PodcastEpisodes == null || !_dbContext.PodcastEpisodes.Any())
            {
                return new List<PodcastEpisode>();
            }

            return _dbContext.PodcastEpisodes.Where(expression).ToList();
        }

        /// <inheritdoc cref="IGenericRepository{PodcastEpisode}" />
        public IList<PodcastEpisode> GetWhere(string connectionString, Expression<Func<PodcastEpisode, bool>> expression)
        {
            using var dbContext = new PodcastEpisodeDbContext(connectionString);
            if (dbContext.PodcastEpisodes == null || !dbContext.PodcastEpisodes.Any())
            {
                return new List<PodcastEpisode>();
            }

            return dbContext.PodcastEpisodes.Where(expression).ToList();
        }

        /// <inheritdoc cref="IGenericRepository{PodcastEpisode}" />
        public async Task<PodcastEpisode?> UpsertAsync(PodcastEpisode entity, CancellationToken cancellationToken = default)
        {
            if (_dbContext.PodcastEpisodes == null)
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
                    await _dbContext.PodcastEpisodes.AddAsync(entity, cancellationToken);
                }
                else // Update
                {
                    existing.Title = entity.Title;
                    existing.Description = entity.Description;
                    existing.ImageUrl = entity.ImageUrl;
                    existing.ImageData = entity.ImageData;
                    existing.ImageDataBase64 = entity.ImageDataBase64;
                    existing.Date = entity.Date;
                    existing.Duration = entity.Duration;
                    existing.Checksum = entity.Checksum;
                    existing.FetchedExpectedNextDate = entity.FetchedExpectedNextDate;
                    existing.CalculatedExpectedNextDate = entity.CalculatedExpectedNextDate;
                    existing.Created = entity.Created;
                    existing.CreatedBy = entity.CreatedBy;
                    existing.Modified = DateTime.Now;
                    existing.ModifiedBy = entity.ModifiedBy;
                    _dbContext.PodcastEpisodes.Update(entity);
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

        /// <inheritdoc cref="IGenericRepository{PodcastEpisode}" />
        public async Task<PodcastEpisode?> UpsertAsync(string connectionString, PodcastEpisode entity, CancellationToken cancellationToken = default)
        {
            using var dbContext = new PodcastEpisodeDbContext(connectionString);
            if (dbContext.PodcastEpisodes == null)
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
                    await dbContext.PodcastEpisodes.AddAsync(entity, cancellationToken);
                }
                else // Update
                {
                    existing.Title = entity.Title;
                    existing.Description = entity.Description;
                    existing.ImageUrl = entity.ImageUrl;
                    existing.ImageData = entity.ImageData;
                    existing.ImageDataBase64 = entity.ImageDataBase64;
                    existing.Date = entity.Date;
                    existing.Duration = entity.Duration;
                    existing.Checksum = entity.Checksum;
                    existing.FetchedExpectedNextDate = entity.FetchedExpectedNextDate;
                    existing.CalculatedExpectedNextDate = entity.CalculatedExpectedNextDate;
                    existing.Created = entity.Created;
                    existing.CreatedBy = entity.CreatedBy;
                    existing.Modified = DateTime.Now;
                    existing.ModifiedBy = entity.ModifiedBy;
                    dbContext.PodcastEpisodes.Update(entity);
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

        /// <inheritdoc cref="IGenericRepository{PodcastEpisode}" />
        public async Task<bool> DeleteByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            if (_dbContext.PodcastEpisodes == null || !_dbContext.PodcastEpisodes.Any())
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
                _dbContext.PodcastEpisodes.Remove(existing);
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

        /// <inheritdoc cref="IGenericRepository{PodcastEpisode}" />
        public async Task<bool> DeleteByIdAsync(string connectionString, long id, CancellationToken cancellationToken = default)
        {
            using var dbContext = new PodcastEpisodeDbContext(connectionString);
            if (dbContext.PodcastEpisodes == null || !dbContext.PodcastEpisodes.Any())
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
                dbContext.PodcastEpisodes.Remove(existing);
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


        /// <inheritdoc cref="IGenericRepository{PodcastEpisode}" />
        public async Task<bool> DeleteWhereAsync(Expression<Func<PodcastEpisode, bool>> expression, CancellationToken cancellationToken = default)
        {
            if (_dbContext.PodcastEpisodes == null || !_dbContext.PodcastEpisodes.Any())
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
                _dbContext.PodcastEpisodes.RemoveRange(existing);
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

        /// <inheritdoc cref="IGenericRepository{PodcastEpisode}" />
        public async Task<bool> DeleteWhereAsync(string connectionString, Expression<Func<PodcastEpisode, bool>> expression, CancellationToken cancellationToken = default)
        {
            using var dbContext = new PodcastEpisodeDbContext(connectionString);
            if (dbContext.PodcastEpisodes == null || !dbContext.PodcastEpisodes.Any())
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
                dbContext.PodcastEpisodes.RemoveRange(existing);
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
