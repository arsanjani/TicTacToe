using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using TicTacToe.Server.app_Start;

namespace TicTacToe.Server.app_Start
{
    public class Program
    {
        public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(ConfigureLogging)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var machineName = Environment.MachineName;
                    var env = context.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{machineName}.json", optional: true, reloadOnChange: true);
                })
                .UseStartup<Startup>();
        }

        static void ConfigureLogging(WebHostBuilderContext context, ILoggingBuilder logging)
        {
            // You can customise logging here
            if (context.HostingEnvironment.IsDevelopment())
            {
                // Configure logging for development
                logging.SetMinimumLevel(LogLevel.Information);
            }
        }
    }
} 