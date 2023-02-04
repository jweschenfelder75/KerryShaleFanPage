using Microsoft.EntityFrameworkCore;
using KerryShaleFanPage.Context.Entities;
using MySql.Data.MySqlClient;

namespace KerryShaleFanPage.Context.Contexts
{
    public class PodcastEpisodeDbContext : DbContext
    {
        public PodcastEpisodeDbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public string ConnectionString { get; set; }

        public DbSet<PodcastEpisode>? PodcastEpisodes { get; set; }
    }
}
