using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using article.API.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebHost.Customization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace article.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();

            var host = BuildWebHost(configuration, args);


            host.MigrateDbContext<ArticleContext>((context, services) =>
            {
                var env = services.GetService<IHostingEnvironment>();
                var settings = services.GetService<IOptions<ArticleSettings>>();
                var logger = services.GetService<ILogger<ArticleContextSeed>>();

                new ArticleContextSeed()
                    .SeedAsync(context, env, settings, logger)
                    .Wait();
            });
               //.MigrateDbContext<IntegrationEventLogContext>((_, __) => { });


            host.Run();
        }

        private static IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
              Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
                  .CaptureStartupErrors(false)
                  .UseStartup<Startup>()
                  //.UseApplicationInsights()
                  .UseContentRoot(Directory.GetCurrentDirectory())
                  .UseWebRoot("Pics")
                  .UseConfiguration(configuration)
                  //.UseSerilog()
                  .Build();


        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();
           

            return builder.Build();
        }
    }
}
