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

        public MySqlConnection GetConnection => new MySqlConnection(ConnectionString);

        public string ConnectionString { get; private set; }

        public DbSet<PodcastEpisode>? PodcastEpisodes { get; set; }
    }
}
