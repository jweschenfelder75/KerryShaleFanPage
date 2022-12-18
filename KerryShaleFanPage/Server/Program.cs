using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using System;

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
                ConfigureServices(builder.Services);

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
        /// Creates the Host Buiilder
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
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();
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