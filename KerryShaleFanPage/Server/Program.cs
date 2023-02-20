using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.IO;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Web;
using NLog.Targets;
using KerryShaleFanPage.Context.Contexts;
using KerryShaleFanPage.Context.Entities;
using KerryShaleFanPage.Context.Repositories;
using KerryShaleFanPage.Server.Interfaces.BusinessLogic;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices.ToDo;
using KerryShaleFanPage.Server.Interfaces.MailAndSmsServices;
using KerryShaleFanPage.Server.Interfaces.Repositories;
using KerryShaleFanPage.Server.Interfaces.Security;
using KerryShaleFanPage.Server.Services;
using KerryShaleFanPage.Server.Services.BusinessLogic;
using KerryShaleFanPage.Server.Services.HtmlAndApiServices;
using KerryShaleFanPage.Server.Services.HtmlAndApiServices.ToDo;
using KerryShaleFanPage.Server.Services.MailAndSmsServices;
using KerryShaleFanPage.Server.Services.Repositories;
using KerryShaleFanPage.Server.Services.Security;
using KerryShaleFanPage.Shared.Configuration;
using KerryShaleFanPage.Shared.Extensions;
using KerryShaleFanPage.Shared.Objects;
using KerryShaleFanPage.Shared.Objects.Acast;
using KerryShaleFanPage.Shared.Objects.ListenNotes;
using KerryShaleFanPage.Shared.Objects.Spotify;
using KerryShaleFanPage.Shared.Objects.ToDo.Twitter;
using KerryShaleFanPage.Shared.Repositories;
using KerryShaleFanPage.Shared.Security;

namespace KerryShaleFanPage.Server
{
    public class Program
    {
        private const string _NLOG_SECTION_NAME = "NLog";

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection(_NLOG_SECTION_NAME));
            LogManager.ThrowExceptions = true;
            LogManager.ThrowConfigExceptions = true;
            ConfigureDbTarget();

            var logger = NLogBuilder.ConfigureNLog(LogManager.Configuration).GetCurrentClassLogger();
            try
            {
                logger.Info("Start app");
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                ConfigureServices(builder.Services, builder.Configuration);

                var app = builder.Build();

                // Configure app.
                app = ConfigureApplication(app);

                app.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped app because of exception");
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        /// <summary>
        /// Configures the given ServiceCollection
        /// </summary>
        /// <param name="services">ServiceCollection</param>
        /// <param name="configuration"></param>
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddLogging(logging => 
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddNLog();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning);
            });

            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            services.AddDbContext<ConfigurationDbContext>(options => options.UseMySQL(configuration.GetConnectionString("Storage")));
            services.AddDbContext<LogDbContext>(options => options.UseMySQL(configuration.GetConnectionString("Storage")));
            services.AddDbContext<PodcastEpisodeDbContext>(options => options.UseMySQL(configuration.GetConnectionString("Storage")));

            services.AddScoped<SecurityProvider>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IMailAndSmsService, GmxMailAndSmsService>();
            services.AddScoped<IGenericRepository<ConfigurationEntry>, ConfigurationRepository>();
            services.AddScoped<IGenericRepository<LogEntry>, LogRepository>();
            services.AddScoped<IGenericRepository<PodcastEpisode>, PodcastEpisodeRepository>();
            services.AddScoped<IGenericRepositoryService<ConfigurationEntryDto>, ConfigurationRepositoryService>();
            services.AddScoped<IGenericRepositoryService<LogEntryDto>, LogRepositoryService>();
            services.AddScoped<IGenericRepositoryService<PodcastEpisodeDto>, PodcastEpisodeRepositoryService>();

            // services.AddScoped<ITwitterCrawlApiService, TwitterCrawlApiService>();  // TODO: Obsolete: We will not use Twitter API anymore!
            // services.AddScoped<ITwitterTweetApiService, TwitterTweetApiService>();  // TODO: Obsolete: We will not use Twitter API anymore!
            // services.AddScoped<IGenericCrawlHtmlService<TwitterEpisode>, TwitterCrawlHtmlService>();  // TODO: Unfinished & untested.
            services.AddScoped<IGenericCrawlHtmlService<AcastEpisode>, AcastCrawlHtmlService>();
            services.AddScoped<IGenericCrawlHtmlService<ListenNotesEpisode>, ListenNotesCrawlHtmlService>();
            services.AddScoped<IGenericCrawlHtmlService<SpotifyEpisode>, SpotifyCrawlHtmlService>();

            services.AddScoped<IPodcastBusinessLogicService, PodcastBusinessLogicService>();

            services.AddHostedService<ScopedBackgroundService>();
        }

        /// <summary>
        /// Configures the given WebApplication
        /// </summary>
        /// <param name="app">WebApplication</param>
        private static WebApplication ConfigureApplication(WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            return app;
        }

        /// <summary>
        /// TODO: Move code in another class!
        /// 
        /// </summary>
        private static void ConfigureDbTarget()
        {
            var strInstallConnectionString = "server=127.0.0.1;database=sys;uid={username};pwd={password};";  // TODO: Make configurable!
            var strConnectionString = "server=127.0.0.1;database=kerryshalefanpg;uid={username};pwd={password};";  // TODO: Make configurable!

            var config = LogManager.Configuration;
            var dbTarget = (DatabaseTarget)config.FindTargetByName("allDatabase");
            dbTarget.ConnectionString = strConnectionString;

            var installationContext = new InstallationContext();
            var createDbCommand = new DatabaseCommandInfo()
            {
                Text = "CREATE DATABASE IF NOT EXISTS kerryshalefanpg",
                CommandType = CommandType.Text
            };
            dbTarget.InstallConnectionString = strInstallConnectionString;
            dbTarget.InstallDdlCommands.Clear();
            dbTarget.InstallDdlCommands.Add(createDbCommand);

            // Create the database if it does not exist
            try
            {
                dbTarget.Install(installationContext);
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
            }

            var sb = new StringBuilder();
            sb.AppendLine("CREATE TABLE IF NOT EXISTS logentries (");
            sb.AppendLine("Id BIGINT NOT NULL AUTO_INCREMENT,");
            sb.AppendLine("Created TIMESTAMP NULL,");
            sb.AppendLine("CreatedBy NVARCHAR(50) NULL,");
            sb.AppendLine("Modified TIMESTAMP NULL,");
            sb.AppendLine("ModifiedBy NVARCHAR(50) NULL,");
            sb.AppendLine("TimeStamp TIMESTAMP NULL,");
            sb.AppendLine("LogLevel NVARCHAR(25) NULL,");
            sb.AppendLine("Logger NVARCHAR(255) NULL,");
            sb.AppendLine("Message NVARCHAR(1024) NULL,");
            sb.AppendLine("Exception NVARCHAR(1024) NULL,");
            sb.AppendLine("PRIMARY KEY (Id))");
            var createTableCommand = new DatabaseCommandInfo()
            {
                Text = sb.ToString(),
                CommandType = CommandType.Text
            };
            dbTarget.InstallConnectionString = strConnectionString;
            dbTarget.InstallDdlCommands.Clear();
            dbTarget.InstallDdlCommands.Add(createTableCommand);

            // Create the table if it does not exist
            try
            {
                dbTarget.Install(installationContext);
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
            }

            LogManager.ReconfigExistingLoggers();
        }
    }
}