namespace WebScraper.Testbed.Services.Application
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using WebScraper.Testbed.Data;

    public class MakeRequestService : IMakeRequestService
    {
        private readonly ILogger<MakeRequestService> m_logger;
        private readonly WebScraperContext m_dbContext;

        public MakeRequestService(ILogger<MakeRequestService> logger, WebScraperContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Dispose()
        {
            m_dbContext.Dispose();
        }

        public async Task MakeRequestAsync(string url)
        {
            m_logger.LogInformation($"Received request for '{url}'");

            Page page = await m_dbContext.Pages.FirstOrDefaultAsync(p => p.Url == url);

            if (page == null)
            {
                m_logger.LogInformation($"No page with url '{url}' has ever been made.  Adding to database.");

                page = new Page
                {
                    Url = url,
                    Status = Status.Pending,
                    RequestedAt = DateTime.UtcNow,
                    ContentHash = String.Empty
                };

                await m_dbContext.Pages.AddAsync(page);
                await m_dbContext.SaveChangesAsync();
            }
            else
            {
                m_logger.LogInformation($"Setting state on page with url '{url}' to Pending.");

                page.ContentHash = string.Empty;
                page.StartedAt = null;
                page.CompletedAt = null;
                page.Status = Status.Pending;

                await m_dbContext.SaveChangesAsync();
            }

            m_logger.LogInformation($"Done logging request for '{url}'");
        }
    }
}
