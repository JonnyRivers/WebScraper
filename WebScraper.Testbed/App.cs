namespace WebScraper.Testbed
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.EntityFrameworkCore;

    using Data;
    using Services.Core;
    using Services.Application;

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
                IResetDataService service = m_serviceProvider.GetService<IResetDataService>();
                service.ResetDataAsync().Wait();
            }
            else if (m_appConfiguration.Action == "make-request")
            {
                IMakeRequestService service = m_serviceProvider.GetService<IMakeRequestService>();
                service.MakeRequestAsync(m_appConfiguration.Url).Wait();
            }
            else if (m_appConfiguration.Action == "service-content")
            {
                while (true)
                {
                    IProcessContentService service = m_serviceProvider.GetService<IProcessContentService>();
                    bool requestWasProcessed = service.ProcessContentAsync().Result;

                    // TODO - fix code duplication here
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.Q)
                        {
                            return 0;
                        }
                    }

                    if (!requestWasProcessed)
                    {
                        m_logger.LogInformation("No pending content.  Sleeping.");
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }
            else if (m_appConfiguration.Action == "service-requests")
            {
                while (true)
                {
                    IProcessRequestService service = m_serviceProvider.GetService<IProcessRequestService>();
                    bool requestWasProcessed = service.ProcessRequestAsync().Result;

                    if(Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        if(keyInfo.Key == ConsoleKey.Q)
                        {
                            return 0;
                        }
                    }

                    if (!requestWasProcessed)
                    {
                        m_logger.LogInformation("No pending requests.  Sleeping.");
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }
            else if (m_appConfiguration.Action == "monitor")
            {
                while (true)
                {
                    IMonitorService service = m_serviceProvider.GetService<IMonitorService>();
                    service.Report();

                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        if (keyInfo.Key == ConsoleKey.Q)
                        {
                            return 0;
                        }
                    }

                    System.Threading.Thread.Sleep(5000);
                }
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
            if(m_appConfiguration.Action != "report")// TODO - fix this terrible hack
            {
                serviceCollection.AddSingleton(loggerFactory);
                serviceCollection.AddLogging();
            }
            
            serviceCollection.AddDbContext<WebScraperContext>(
                options => options.UseSqlServer(WebScraperContextFactory.ConnectionString),
                ServiceLifetime.Transient);

            // Core services
            serviceCollection.AddTransient<IHashService, MD5HashService>();
            serviceCollection.AddTransient<IHttpClientService, HttpClientService>();
            serviceCollection.AddTransient<IPageParseService, PageParseService>();

            // Application services
            serviceCollection.AddTransient<IMakeRequestService, MakeRequestService>();
            serviceCollection.AddTransient<IProcessContentService, ProcessContentService>();
            serviceCollection.AddTransient<IProcessRequestService,ProcessRequestService>();
            serviceCollection.AddTransient<IResetDataService, ResetDataService>();
            serviceCollection.AddTransient<IMonitorService, MonitorService>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
