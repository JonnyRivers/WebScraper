using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebScraper.Testbed
{
    internal class RequestAction : IDisposable
    {
        private readonly WebScraperContext m_dbContext;
        private readonly ILogger<RequestAction> m_logger;

        public RequestAction(WebScraperContext dbContext, ILogger<RequestAction> logger)
        {
            m_dbContext = dbContext;
            m_logger = logger;
        }

        public void Dispose()
        {
            m_dbContext.Dispose();
        }

        public async Task<int> RunAsync(string url)
        {
            var pageRequest = new PageRequest
            {
                Url = url,
                Status = Status.Pending,
                RequestedAt = DateTime.UtcNow,
                ContentHash = String.Empty
            };
            await m_dbContext.PageRequests.AddAsync(pageRequest);

            await m_dbContext.SaveChangesAsync();

            return 0;
        }
    }
}
