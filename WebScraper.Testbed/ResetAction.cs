using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebScraper.Testbed
{
    internal class ResetAction : IDisposable
    {
        private readonly WebScraperContext m_dbContext;
        private readonly ILogger<ResetAction> m_logger;

        public ResetAction(WebScraperContext dbContext, ILogger<ResetAction> logger)
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
            m_dbContext.PageRequests.RemoveRange(m_dbContext.PageRequests);
            m_dbContext.Content.RemoveRange(m_dbContext.Content);

            await m_dbContext.SaveChangesAsync();

            return 0;
        }
    }
}
