using System;
using Microsoft.EntityFrameworkCore;
using KerryShaleFanPage.Context.Entities;

namespace KerryShaleFanPage.Context.Contexts
{
    public class ConfigurationDbContext : DbContext
    {
        private readonly string? _connectionString;

        public ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options)
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

        public ConfigurationDbContext(string? connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_connectionString ?? "server=127.0.0.1;database=kerryshalefanpg;uid={username};pwd={password};");  // TODO: Make configurable!
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ConfigurationEntry>? ConfigurationEntries { get; set; }
    }
}
