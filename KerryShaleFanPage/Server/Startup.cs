using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using NLog.Targets;
using KerryShaleFanPage.Context.Contexts;
using KerryShaleFanPage.Context.Entities;
using KerryShaleFanPage.Context.Repositories;
using KerryShaleFanPage.Server.Hub;
using KerryShaleFanPage.Server.Interfaces.BusinessLogic;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices;
using KerryShaleFanPage.Server.Interfaces.HtmlAndApiServices.ToDo;
using KerryShaleFanPage.Server.Interfaces.MailAndSmsServices;
using KerryShaleFanPage.Server.Interfaces.Maintenance;
using KerryShaleFanPage.Server.Interfaces.Repositories;
using KerryShaleFanPage.Server.Interfaces.Security;
using KerryShaleFanPage.Server.Services;
using KerryShaleFanPage.Server.Services.BusinessLogic;
using KerryShaleFanPage.Server.Services.HtmlAndApiServices;
using KerryShaleFanPage.Server.Services.HtmlAndApiServices.ToDo;
using KerryShaleFanPage.Server.Services.MailAndSmsServices;
using KerryShaleFanPage.Server.Services.Maintenance;
using KerryShaleFanPage.Server.Services.Repositories;
using KerryShaleFanPage.Server.Services.Security;
using KerryShaleFanPage.Shared.Configuration;
using KerryShaleFanPage.Shared.Objects;
using KerryShaleFanPage.Shared.Objects.Acast;
using KerryShaleFanPage.Shared.Objects.ListenNotes;
using KerryShaleFanPage.Shared.Objects.Spotify;
using KerryShaleFanPage.Shared.Objects.ToDo.Twitter;
using KerryShaleFanPage.Shared.Repositories;
using KerryShaleFanPage.Shared.Security;

namespace KerryShaleFanPage.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddNLog();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddSignalR();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<GeneralSettings>(Configuration.GetSection("GeneralSettings"));
            services.Configure<NewsSettings>(Configuration.GetSection("NewsSettings"));
            services.Configure<GallerySettings>(Configuration.GetSection("GallerySettings"));

            services.AddScoped<SecurityProvider>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<ISecuredConfigurationService, SecuredConfigurationService>();
            services.AddScoped<IMaintenanceNotificationService, MaintenanceNotificationService>();
            services.AddScoped<IGmailMailAndSmsService, GmailMailAndSmsService>();
            services.AddScoped<IGmxMailAndSmsService, GmxMailAndSmsService>();

            services.AddDbContext<LogDbContext>((services, options) =>
            {
                string connectionString = GetValidConnectionString(services);
                options.UseMySQL(connectionString);
            });
            services.AddDbContext<PodcastEpisodeDbContext>((services, options) =>
            {
                string connectionString = GetValidConnectionString(services);
                options.UseMySQL(connectionString);
            });

            services.AddScoped<IGenericService<NewsItemDto>, NewsService>();
            services.AddScoped<IGenericService<GalleryItemDto>, GalleryService>();
            services.AddScoped<IGenericRepository<LogEntry>, LogRepository>();
            //services.AddScoped<IGenericRepository<PodcastEpisode>, PodcastEpisodeRepository>();
            services.AddScoped<IGenericRepositoryService<LogEntryDto>, LogRepositoryService>();
            //services.AddScoped<IGenericRepositoryService<PodcastEpisodeDto>, PodcastEpisodeRepositoryService>();

            // services.AddScoped<ITwitterCrawlApiService, TwitterCrawlApiService>();  // TODO: Obsolete: We will not use Twitter API anymore!
            // services.AddScoped<ITwitterTweetApiService, TwitterTweetApiService>();  // TODO: Obsolete: We will not use Twitter API anymore!
            // services.AddScoped<IGenericCrawlHtmlService<TwitterEpisode>, TwitterCrawlHtmlService>();  // TODO: Unfinished & untested.
            //services.AddScoped<IGenericCrawlHtmlService<AcastEpisode>, AcastCrawlHtmlService>();
            //services.AddScoped<IGenericCrawlHtmlService<ListenNotesEpisode>, ListenNotesCrawlHtmlService>();
            //services.AddScoped<IGenericCrawlHtmlService<SpotifyEpisode>, SpotifyCrawlHtmlService>();

            services.AddScoped<IGeneralBusinessLogicService, GeneralBusinessLogicService>();
            //services.AddScoped<IPodcastBusinessLogicService, PodcastBusinessLogicService>();

            services.AddHostedService<ScopedGeneralBackgroundService>();
            //services.AddHostedService<ScopedPodcastBackgroundService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            ConfigureDbTarget(app);

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            // Begin I18N configuration
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en"),
                new CultureInfo("de"),
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            app.UseRequestLocalization(options);
            // End I18N configuration

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<SignalRHub>("/signalrhub");
                endpoints.MapFallbackToFile("index.html");
            });
        }

        /// <summary>
        /// TODO: Move code in another class? Or let it here, it looked already worse in the past. ;-)
        /// 
        /// </summary>
        private static void ConfigureDbTarget(IApplicationBuilder app)
        {
            try
            {
                var strConnectionString = GetValidConnectionString(app.ApplicationServices);

                var dbTarget = LogManager.Configuration.FindTargetByName<DatabaseTarget>("allDatabase");

                if (dbTarget != null)
                {
                    dbTarget.ConnectionString = strConnectionString;

                    LogManager.ReconfigExistingLoggers();
                }
            }
            catch (Exception ex)
            {
                var exception = ex;
            }
        }

        private static string GetValidConnectionString(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var configurationService = scope.ServiceProvider.GetRequiredService<ISecuredConfigurationService>();
            var settings = configurationService.GetDecryptedConfigurationForSettingsFromEncryptedFile();

            return $"server=127.0.0.1;database=kerryshalefanpg;uid={settings.DbUsername};pwd={settings.DbPassword};";
        }
    }
}