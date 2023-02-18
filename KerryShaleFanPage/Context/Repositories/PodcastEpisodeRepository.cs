using KerryShaleFanPage.Shared.Contexts;
using KerryShaleFanPage.Shared.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KerryShaleFanPage.Shared.Repositories
{
    /// <summary>
    /// TODO: This is EF Core logic, it needs to be adjusted for MySQL!
    /// </summary>
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

            return _dbContext.PodcastEpisodes.ToList();
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
            if (_dbContext.PodcastEpisodes == null || !_dbContext.PodcastEpisodes.Any())
            {
                return entity;
            }

            var existing = GetById(entity.Id);
            if (existing == null)  // Insert
            {
                await _dbContext.PodcastEpisodes.AddAsync(entity, cancellationToken);
            }
            else  // Update
            {
                existing.Title = entity.Title;
                existing.Description = entity.Description;
                existing.ImageUrl = entity.ImageUrl;
                existing.ImageData = entity.ImageData;
                existing.ImageDataBase64 = entity.ImageDataBase64;
                existing.Date = entity.Date;
                existing.Duration = entity.Duration;
                existing.FetchedExpectedNextDate = entity.FetchedExpectedNextDate;
                _dbContext.PodcastEpisodes.Update(entity);
            }
            var success = (await _dbContext.SaveChangesAsync(cancellationToken)) > 0;
            if (!success)
            {
                return entity;
            }

            return GetById(entity.Id);
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

            _dbContext.PodcastEpisodes.Remove(existing);
            return (await _dbContext.SaveChangesAsync(cancellationToken)) > 0;
        }
    }
}
