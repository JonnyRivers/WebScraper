namespace WebScraper.Testbed.Actions
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using WebScraper.Testbed.Data;

    internal class ResetAction : IDisposable
    {
        private readonly WebScraperContext m_dbContext;// TODO - make a data service
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
            m_dbContext.PageLinks.RemoveRange(m_dbContext.PageLinks);
            m_dbContext.Content.RemoveRange(m_dbContext.Content);
            m_dbContext.Pages.RemoveRange(m_dbContext.Pages);

            await m_dbContext.SaveChangesAsync();

            return 0;
        }
    }
}
