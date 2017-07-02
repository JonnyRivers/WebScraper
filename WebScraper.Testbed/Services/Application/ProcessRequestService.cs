using System;
using System.Collections.Generic;
using System.Text;

namespace WebScraper.Testbed.Services.Application
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using WebScraper.Testbed.Data;

    public class ProcessRequestService : IProcessRequestService
    {
        private readonly ILogger<ProcessRequestService> m_logger;
        private readonly WebScraperContext m_dbContext;

        public ProcessRequestService(ILogger<ProcessRequestService> logger, WebScraperContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Dispose()
        {
            m_dbContext.Dispose();
        }

        public async Task ProcessRequestAsync()
        {
            m_logger.LogInformation("Finding next available pending page.");
            Page nextPage = await m_dbContext.Pages
                .FirstOrDefaultAsync(x => x.Status == Status.Pending);

            if (nextPage == null)
            {
                m_logger.LogInformation("No pages are available to process.");

                return;
            }

            nextPage.StartedAt = DateTime.UtcNow;
            nextPage.Status = Status.Downloading;

            await m_dbContext.SaveChangesAsync();
        }
    }
}
