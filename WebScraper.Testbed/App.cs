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

    internal class App
    {
        private AppConfiguration m_appConfiguration;
        private ILogger<App> m_logger;
        private IServiceProvider m_serviceProvider;

        internal App(string[] args)
        {
            m_appConfiguration = BuildAppConfiguration(args);
            m_serviceProvider = BuildServiceProvider();
            m_logger = m_serviceProvider.GetService<ILogger<App>>();
        }

        internal int Run()
        {
            if (m_appConfiguration.Action == "reset")
            {
                ResetAction action = m_serviceProvider.GetService<ResetAction>();
                return action.RunAsync().Result;
            }
            else if (m_appConfiguration.Action == "make-request")
            {
                IMakeRequestService service = m_serviceProvider.GetService<IMakeRequestService>();
                service.MakeRequestAsync(m_appConfiguration.Url).Wait();
            }
            else if (m_appConfiguration.Action == "service-content")
            {
                ServiceContentAction action = m_serviceProvider.GetService<ServiceContentAction>();
                return action.RunAsync().Result;
            }
            else if (m_appConfiguration.Action == "service-requests")
            {
                ServiceRequestsAction action = m_serviceProvider.GetService<ServiceRequestsAction>();
                return action.RunAsync().Result;
            }
            else
            {
                m_logger.LogError($"Unrecognized action '{m_appConfiguration.Action}'");
                return 1;
            }

            return 0;
        }

        private AppConfiguration BuildAppConfiguration(string[] args)
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

            return appConfiguration;
        }

        private IServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            // .NET Services
            ILoggerFactory loggerFactory = new LoggerFactory().AddConsole().AddDebug();
            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddLogging();
            serviceCollection.AddDbContext<WebScraperContext>(
                options => options.UseSqlServer(WebScraperContextFactory.ConnectionString));

            // Core services
            serviceCollection.AddTransient<IDataService, EFDataService>();
            serviceCollection.AddTransient<IHashService, MD5HashService>();
            serviceCollection.AddTransient<IHttpClientService, HttpClientService>();
            serviceCollection.AddTransient<IPageParseService, PageParseService>();

            // Application services
            serviceCollection.AddTransient<IMakeRequestService, MakeRequestService>();

            serviceCollection.AddTransient<ResetAction>();
            serviceCollection.AddTransient<ServiceContentAction>();
            serviceCollection.AddTransient<ServiceRequestsAction>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
