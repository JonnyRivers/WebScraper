using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;


namespace WebScraper.Testbed
{
    class AppConfiguration
    {
        public string Task { get; set; }
    }

    class Program
    {
        static int Main(string[] args)
        {
            var inMemoryConfig = new Dictionary<string, string>
            {
                {"Task", "Initialize"}
            };

            var switchMappings = new Dictionary<string, string>
            {
                {"--task", "Task"},
                {"-t", "Task"}
            };

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .AddInMemoryCollection(inMemoryConfig)
                .AddCommandLine(args, switchMappings);
            IConfiguration configuration = configurationBuilder.Build();
            AppConfiguration appConfiguration = configuration.Get<AppConfiguration>();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            App app = serviceProvider.GetService<App>();
            return app.RunAsync().Result;
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            ILoggerFactory loggerFactory = new LoggerFactory().AddConsole().AddDebug();
            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddLogging();

            string connectionString = @"Server=(localdb)\mssqllocaldb;Database=WebScraper;Trusted_Connection=True;";
            serviceCollection.AddDbContext<WebScraperContext>(options => options.UseSqlServer(connectionString));

            serviceCollection.AddTransient<App>();
        }
    }
}
