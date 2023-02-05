using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using KerryShaleFanPage.Server.Services;
using KerryShaleFanPage.Shared.Objects.ListenNotes;
using KerryShaleFanPage.Shared.Objects.Spotify;
using KerryShaleFanPage.Context.Contexts;
using KerryShaleFanPage.Context.Entities;
using KerryShaleFanPage.Context.Repositories;
using KerryShaleFanPage.Server.Interfaces.BusinessLogic;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices.ToDo;
using KerryShaleFanPage.Server.Interfaces.MailAndSmsServices;
using KerryShaleFanPage.Server.Interfaces.Repositories;
using KerryShaleFanPage.Server.Services.BusinessLogic;
using KerryShaleFanPage.Server.Services.HtmlAndApiServices;
using KerryShaleFanPage.Server.Services.HtmlAndApiServices.ToDo;
using KerryShaleFanPage.Server.Services.MailAndSmsServices;
using KerryShaleFanPage.Server.Services.Repositories;
using KerryShaleFanPage.Shared.Objects;
using KerryShaleFanPage.Shared.Objects.ToDo.Twitter;

namespace KerryShaleFanPage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));

            var logger = NLogBuilder.ConfigureNLog(LogManager.Configuration).GetCurrentClassLogger();
            try
            {
                logger.Debug("Start app");
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
            .UseNLog(new NLogAspNetCoreOptions()
            {
                LoggingConfigurationSectionName = "NLog"
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

            services.Add(new ServiceDescriptor(typeof(PodcastEpisodeDbContext), new PodcastEpisodeDbContext(configuration.GetConnectionString("Storage"))));
            services.AddSingleton<IGenericRepository<PodcastEpisode>, PodcastEpisodeRepository>();
            services.AddSingleton<IGenericRepositoryService<PodcastEpisodeDto>, PodcastEpisodeRepositoryService>();

            services.AddSingleton<IMailAndSmsService, GmxMailAndSmsService>();
            // services.AddSingleton<ITwitterCrawlApiService, TwitterCrawlApiService>();  // TODO: Obsolete: We will not use Twitter API anymore!
            // services.AddSingleton<ITwitterTweetApiService, TwitterTweetApiService>();  // TODO: Obsolete: We will not use Twitter API anymore!
            // services.AddSingleton<IGenericCrawlHtmlService<TwitterEpisode>, TwitterCrawlHtmlService>();  // TODO: Unfinished & untested.
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
    }
}