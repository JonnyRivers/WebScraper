namespace WebScraper.Testbed
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.EntityFrameworkCore;

    using Actions;
    using Data;
    using Services;

    class Program
    {
        static int Main(string[] args)
        {
            var inMemoryConfig = new Dictionary<string, string>
            {
                {"Action", "reset"}
            };

            var switchMappings = new Dictionary<string, string>
            {
                {"--action", "Action"},
                {"-a", "Action"},
                {"--url", "Url"},
                {"-u", "Url"}
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

            ILogger logger = serviceProvider.GetService<ILogger>();

            if (appConfiguration.Action == "reset")
            {
                ResetAction action = serviceProvider.GetService<ResetAction>();
                return action.RunAsync().Result;
            }
            else if (appConfiguration.Action == "make-request")
            {
                IMakeRequestService service = serviceProvider.GetService<IMakeRequestService>();
                service.MakeRequestAsync(appConfiguration.Url).Wait();
            }
            else if (appConfiguration.Action == "service-content")
            {
                ServiceContentAction action = serviceProvider.GetService<ServiceContentAction>();
                return action.RunAsync().Result;
            }
            else if (appConfiguration.Action == "service-requests")
            {
                ServiceRequestsAction action = serviceProvider.GetService<ServiceRequestsAction>();
                return action.RunAsync().Result;
            }
            else
            {
                logger.LogError($"Unrecognized action '{appConfiguration.Action}'");
                return 1;
            }

            return 0;
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            ILoggerFactory loggerFactory = new LoggerFactory().AddConsole().AddDebug();
            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddLogging();

            serviceCollection.AddDbContext<WebScraperContext>(
                options => options.UseSqlServer(WebScraperContextFactory.ConnectionString));

            serviceCollection.AddTransient<IDataService, EFDataService>();
            serviceCollection.AddTransient<IHashService, MD5HashService>();
            serviceCollection.AddTransient<IHttpClientService, HttpClientService>();
            serviceCollection.AddTransient<IPageParseService, PageParseService>();

            serviceCollection.AddTransient<ResetAction>();
            serviceCollection.AddTransient<ServiceContentAction>();
            serviceCollection.AddTransient<ServiceRequestsAction>();
        }
    }
}
