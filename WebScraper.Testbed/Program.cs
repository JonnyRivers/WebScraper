﻿using System;
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
        public string Url { get; set; }
    }

    class Program
    {
        static int Main(string[] args)
        {
            var inMemoryConfig = new Dictionary<string, string>
            {
                {"Task", "reset"}
            };

            var switchMappings = new Dictionary<string, string>
            {
                {"--task", "Task"},
                {"-t", "Task"},
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

            if (appConfiguration.Task == "reset")
            {
                ResetAction action = serviceProvider.GetService<ResetAction>();
                return action.RunAsync().Result;
            }
            else if (appConfiguration.Task == "request")
            {
                RequestAction action = serviceProvider.GetService<RequestAction>();
                return action.RunAsync(appConfiguration.Url).Result;
            }
            else if (appConfiguration.Task == "service-requests")
            {
                ServiceRequestsAction action = serviceProvider.GetService<ServiceRequestsAction>();
                return action.RunAsync().Result;
            }

            return 1;
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            ILoggerFactory loggerFactory = new LoggerFactory().AddConsole().AddDebug();
            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddLogging();

            serviceCollection.AddDbContext<WebScraperContext>(
                options => options.UseSqlServer(WebScraperContextFactory.ConnectionString));

            serviceCollection.AddTransient<IHashService, HashMD5Service>();

            serviceCollection.AddTransient<ResetAction>();
            serviceCollection.AddTransient<RequestAction>();
            serviceCollection.AddTransient<ServiceRequestsAction>();
        }
    }
}
