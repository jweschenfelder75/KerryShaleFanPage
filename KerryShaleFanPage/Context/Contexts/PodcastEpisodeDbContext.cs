using Microsoft.EntityFrameworkCore;
using System;
using KerryShaleFanPage.Context.Entities;

namespace KerryShaleFanPage.Context.Contexts
{
    public class PodcastEpisodeDbContext : DbContext
    {
        private readonly string? _connectionString;

        public PodcastEpisodeDbContext(DbContextOptions<PodcastEpisodeDbContext> options)
            : base(options)
        {
            try
            {
                Database.Migrate();
            }
            catch (Exception ex)
            {
            }
        }

        public PodcastEpisodeDbContext(string? connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_connectionString ?? "server=127.0.0.1;database=kerryshalefanpg;uid={username};pwd={password};");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<PodcastEpisode>? PodcastEpisodes { get; set; }
    }
}
