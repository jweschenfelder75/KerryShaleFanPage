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
    public class PodcastEpisodeRepository : IGenericRepository<PodcastEpisode>
    {
        private readonly PodcastEpisodeDbContext _dbContext;

        public PodcastEpisodeRepository(PodcastEpisodeDbContext dbContext)
        {
            _dbContext = dbContext;
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
        public PodcastEpisode? GetById(long id)
        {
            if (_dbContext.PodcastEpisodes == null || !_dbContext.PodcastEpisodes.Any())
            {
                return null;
            }

            return _dbContext.PodcastEpisodes.FirstOrDefault(entity => entity.Id == id);
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
    }
}
