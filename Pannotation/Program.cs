using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Pannotation.DAL.Migrations;
using Pannotation.DAL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NLog.Web;
using System.Net;
using NLog.Targets;
using NLog;
using NLog.Layouts;

namespace Pannotation
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }

        public static void Main(string[] args)
        {
            var appBasePath = System.IO.Directory.GetCurrentDirectory();
            NLog.GlobalDiagnosticsContext.Set("appbasepath", appBasePath);

            // NLog: setup the logger first to catch all errors
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                foreach (FileTarget target in LogManager.Configuration.AllTargets)
                {
                    target.FileName = appBasePath + "/" + ((SimpleLayout)target.FileName).OriginalText;
                }

                LogManager.ReconfigExistingLoggers();

                logger.Debug("init main");


                var host = BuildWebHost(args);
                var builder = new ConfigurationBuilder()
                .SetBasePath(appBasePath)
                .AddJsonFile("appsettings.json");

                Configuration = builder.Build();
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var context = services.GetRequiredService<DataContext>();
                        DbInitializer.Initialize(context, Configuration, services);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "An error occurred while seeding the database.");
                    }
                }

                host.Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseDefaultServiceProvider(options => options.ValidateScopes = false)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                })
                .UseNLog()  // NLog: setup NLog for Dependency injection
                .Build();
    }
}
