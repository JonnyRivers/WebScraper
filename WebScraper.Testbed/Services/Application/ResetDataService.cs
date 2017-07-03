namespace WebScraper.Testbed.Services.Application
{
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using WebScraper.Testbed.Data;

    public class ResetDataService : IResetDataService
    {
        private readonly ILogger<ResetDataService> m_logger;
        private readonly WebScraperContext m_dbContext;

        public ResetDataService(ILogger<ResetDataService> logger, WebScraperContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Dispose()
        {
            m_dbContext.Dispose();
        }

        public async Task ResetDataAsync()
        {
            m_logger.LogInformation($"Resetting all data");

            m_dbContext.PageLinks.RemoveRange(m_dbContext.PageLinks);
            m_dbContext.Content.RemoveRange(m_dbContext.Content);
            m_dbContext.Pages.RemoveRange(m_dbContext.Pages);

            await m_dbContext.SaveChangesAsync();

            m_logger.LogInformation($"Done resetting all data");
        }
    }
}
