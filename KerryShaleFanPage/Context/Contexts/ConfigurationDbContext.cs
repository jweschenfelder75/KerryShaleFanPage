using Microsoft.EntityFrameworkCore;
using KerryShaleFanPage.Shared.Objects;
using MySql.Data.MySqlClient;

namespace KerryShaleFanPage.Shared.Contexts
{
    public class ConfigurationDbContext : DbContext
    {
        public ConfigurationDbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public MySqlConnection GetConnection => new MySqlConnection(ConnectionString);

        public string ConnectionString { get; private set; }

        public DbSet<PodcastEpisode>? PodcastEpisodes { get; set; }
    }
}
