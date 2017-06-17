using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebScraper.Testbed
{
    internal class App : IDisposable
    {
        private readonly WebScraperContext m_dbContext;
        private readonly ILogger<App> m_logger;

        public App(WebScraperContext dbContext, ILogger<App> logger)
        {
            m_dbContext = dbContext;
            m_logger = logger;
        }

        public void Dispose()
        {
            m_dbContext.Dispose();
        }

        public async Task<int> RunAsync()
        {
            int numPageRequests = await m_dbContext.PageRequests.CountAsync();
            m_logger.LogInformation($"numPageRequests: {numPageRequests}");

            return 0;
        }
    }
}
