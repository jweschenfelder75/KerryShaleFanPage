using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Web;
using NLog.Targets;
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
using KerryShaleFanPage.Shared.Contexts;
using KerryShaleFanPage.Shared.Extensions;
using KerryShaleFanPage.Shared.Objects;
using KerryShaleFanPage.Shared.Objects.Acast;
using KerryShaleFanPage.Shared.Objects.ListenNotes;
using KerryShaleFanPage.Shared.Objects.Spotify;
using KerryShaleFanPage.Shared.Objects.ToDo.Twitter;
using KerryShaleFanPage.Shared.Repositories;
using KerryShaleFanPage.Shared.Security;

namespace KerryShaleFanPage
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
            LogManager.ThrowConfigExceptions = true;

            var strConnectionString = "server=127.0.0.1;database=kerryshalefanpg;uid={username};pwd={password};";  // TODO: Make configurable!
            GetDBLogger(strConnectionString);
            LogManager.ReconfigExistingLoggers();
            LogManager.Setup().ReloadConfiguration();

            var logger = NLogBuilder.ConfigureNLog(LogManager.Configuration).GetCurrentClassLogger();
            try
            {
                logger.Info("Start app");
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                ConfigureServices(builder.Services, builder.Configuration);

                var app = builder.Build();

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
        /// Creates the Host Builder
        /// </summary>
        /// <param name="args">Optional arguments from Main method</param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    // TODO: LoggingLevel is not working via DI yet. It is only working in this class so far. A
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddNLog();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning);
                    logging.AddFilter("Microsoft.AspNetCore", Microsoft.Extensions.Logging.LogLevel.Error);
                    logging.AddFilter("Microsoft.AspNetCore.*", Microsoft.Extensions.Logging.LogLevel.Error);
                 })
                .UseNLog(new NLogAspNetCoreOptions()
                {
                    // TODO: LoggingLevel is not working via DI yet. It is only working in this class so far. A
                    LoggingConfigurationSectionName = _NLOG_SECTION_NAME,
                    RemoveLoggerFactoryFilter = true
                });

        /// <summary>
        /// Configures the given ServiceCollection
        /// </summary>
        /// <param name="services">ServiceCollection</param>
        /// <param name="configuration"></param>
        private static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddLogging(logging => 
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddNLog();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning);
                logging.AddFilter("Microsoft.AspNetCore", Microsoft.Extensions.Logging.LogLevel.Error);
                logging.AddFilter("Microsoft.AspNetCore.*", Microsoft.Extensions.Logging.LogLevel.Error);
            });

            services.AddSingleton<SecurityProvider>();
            services.AddSingleton<ISecurityService, SecurityService>();
            services.Add(new ServiceDescriptor(typeof(PodcastEpisodeDbContext), new PodcastEpisodeDbContext(configuration.GetConnectionString("Storage"))));
            services.AddSingleton<IMailAndSmsService, GmxMailAndSmsService>();
            services.AddSingleton<IGenericRepository<PodcastEpisode>, PodcastEpisodeRepository>();
            services.AddSingleton<IGenericRepositoryService<PodcastEpisodeDto>, PodcastEpisodeRepositoryService>();

            // services.AddSingleton<ITwitterCrawlApiService, TwitterCrawlApiService>();  // TODO: Obsolete: We will not use Twitter API anymore!
            // services.AddSingleton<ITwitterTweetApiService, TwitterTweetApiService>();  // TODO: Obsolete: We will not use Twitter API anymore!
            // services.AddSingleton<IGenericCrawlHtmlService<TwitterEpisode>, TwitterCrawlHtmlService>();  // TODO: Unfinished & untested.
            services.AddSingleton<IGenericCrawlHtmlService<AcastEpisode>, AcastCrawlHtmlService>();
            services.AddSingleton<IGenericCrawlHtmlService<ListenNotesEpisode>, ListenNotesCrawlHtmlService>();
            services.AddSingleton<IGenericCrawlHtmlService<SpotifyEpisode>, SpotifyCrawlHtmlService>();
            services.AddSingleton<IPodcastBusinessLogicService, PodcastBusinessLogicService>();

            services.AddHostedService<TimedHostedService>();
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
        /// See: https://stackoverflow.com/questions/20101809/creating-a-database-programatically-in-nlog-to-enable-using-databasetarget
        /// </summary>
        private static void GetDBLogger(string strConnectionString)
        {
            var sb = new StringBuilder();
            var installationContext = new InstallationContext();

            var builder = new MySqlConnectionStringBuilder()
            {
                ConnectionString = strConnectionString
            };
            var strDatabase = builder.Database;

            DatabaseTarget targetDB = new DatabaseTarget()
            {
                Name = "allDatabase",
                //KeepConnection = false,
                DBProvider = "MySql.Data.MySqlClient.MySqlConnection, MySql.Data",
                ConnectionString = strConnectionString,
                CommandText = "INSERT INTO Logs (Created,CreatedBy,Modified,ModifiedBy,TimeStamp,LogLevel,Logger,Message,Exception) VALUES (@Created,@CreatedBy,@Modified,@ModifiedBy,@TimeStamp,@LogLevel,@Logger,@Message,@Exception);"
            };

            DatabaseParameterInfo paramDB;
            paramDB = new DatabaseParameterInfo()
            {
                Name = "@Created",
                Layout = "${date}"
            };
            targetDB.Parameters.Add(paramDB);

            paramDB = new DatabaseParameterInfo()
            {
                Name = "@CreatedBy",
                Layout = _NLOG_SECTION_NAME
            };
            targetDB.Parameters.Add(paramDB);

            paramDB = new DatabaseParameterInfo()
            {
                Name = "@Modified",
                Layout = "${date}"
            };
            targetDB.Parameters.Add(paramDB);

            paramDB = new DatabaseParameterInfo()
            {
                Name = "@ModifiedBy",
                Layout = _NLOG_SECTION_NAME
            };
            targetDB.Parameters.Add(paramDB);

            paramDB = new DatabaseParameterInfo()
            {
                Name = "@TimeStamp",
                Layout = "${date}"
            };
            targetDB.Parameters.Add(paramDB);

            paramDB = new DatabaseParameterInfo()
            {
                Name = "@LogLevel",
                Layout = "${level}"
            };
            targetDB.Parameters.Add(paramDB);

            paramDB = new DatabaseParameterInfo()
            {
                Name = "@Logger",
                Layout = "${logger:truncate=255}"
            };
            targetDB.Parameters.Add(paramDB);

            paramDB = new DatabaseParameterInfo()
            {
                Name = "@Message",
                Layout = "${message:truncate=1024}"
            };
            targetDB.Parameters.Add(paramDB);

            paramDB = new DatabaseParameterInfo()
            {
                Name = "@Exception",
                Layout = "${exception:tostring:truncate=1024}"
            };
            targetDB.Parameters.Add(paramDB);

            var config = LogManager.Configuration;
            config ??= new LoggingConfiguration();

            config.AddTarget(targetDB.Name, targetDB);

            var rule1 = new LoggingRule("Microsoft.AspNetCore.*", NLog.LogLevel.Warn, targetDB);
            config.LoggingRules.Add(rule1);
            var rule2 = new LoggingRule("Microsoft.AspNetCore", NLog.LogLevel.Warn, targetDB);
            config.LoggingRules.Add(rule2);
            var rule3 = new LoggingRule("*", NLog.LogLevel.Info, targetDB);
            config.LoggingRules.Add(rule3);
            LogManager.Configuration = config;

            var builder2 = new MySqlConnectionStringBuilder()
            {
                ConnectionString = strConnectionString,
                Database = "sys"
            };

            targetDB.InstallConnectionString = builder2.ConnectionString;

            sb.AppendLine($"CREATE DATABASE IF NOT EXISTS {strDatabase}");

            var createDBCommand = new DatabaseCommandInfo()
            {
                Text = sb.ToString(),
                CommandType = CommandType.Text
            };
            targetDB.InstallDdlCommands.Add(createDBCommand);

            try
            {
                targetDB.Install(installationContext);
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
            }

            targetDB.InstallDdlCommands.Clear();
            sb.Clear();
            sb.AppendLine("CREATE TABLE IF NOT EXISTS Logs (");
            sb.AppendLine("Id VARCHAR(40) NOT NULL DEFAULT (uuid()),");
            sb.AppendLine("Created TIMESTAMP NULL,");
            sb.AppendLine("CreatedBy NVARCHAR(50) NULL,");
            sb.AppendLine("Modified TIMESTAMP NULL,");
            sb.AppendLine("ModifiedBy NVARCHAR(50) NULL,");
            sb.AppendLine("TimeStamp TIMESTAMP NULL,");
            sb.AppendLine("LogLevel NVARCHAR(25) NULL,");
            sb.AppendLine("Logger NVARCHAR(255) NULL,");
            sb.AppendLine("Message NVARCHAR(1024) NULL,");
            sb.AppendLine("Exception NVARCHAR(1024) NULL, ");
            sb.AppendLine("PRIMARY KEY (Id)) ");

            var createTableCommand = new DatabaseCommandInfo()
            {
                Text = sb.ToString(),
                CommandType = CommandType.Text
            };
            targetDB.InstallDdlCommands.Add(createTableCommand);

            targetDB.InstallConnectionString = strConnectionString;

            try
            {
                targetDB.Install(installationContext);
            }
            catch (Exception ex)
            {
                var exception = ex;  // TODO: Log exception!
            }
            SimpleConfigurator.ConfigureForTargetLogging(targetDB);
        }
    }
}