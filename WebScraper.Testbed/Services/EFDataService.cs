namespace WebScraper.Testbed.Services
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using WebScraper.Testbed.Data;

    internal class EFDataService : IDataService
    {
        private readonly WebScraperContext m_dbContext;

        public EFDataService(WebScraperContext dbContext)
        {
            m_dbContext = dbContext;
        }

        public void Dispose()
        {
            m_dbContext.Dispose();
        }

        public async Task AddPageAsync(Page page)
        {
            await m_dbContext.Pages.AddAsync(page);

            await m_dbContext.SaveChangesAsync();
        }

        public async Task EnsureContentAsync(Content newRecord)
        {
            Content existingRecord = await m_dbContext.Content.FirstOrDefaultAsync(x => x.Hash == newRecord.Hash);
            if (existingRecord == null)
            {
                await m_dbContext.Content.AddAsync(newRecord);

                await m_dbContext.SaveChangesAsync();
            }
        }

        public async Task<Page> FirstOrDefaultPendingPageAsync()
        {
            return await m_dbContext.Pages
                .FirstOrDefaultAsync(x => x.Status == Status.Pending);
        }

        public async Task UpdatePageDownloadingAsync(int pageId)
        {
            Page dbPage = await m_dbContext.Pages.SingleAsync(p => p.PageId == pageId);
            dbPage.StartedAt = DateTime.UtcNow;
            dbPage.Status = Status.Downloading;

            await m_dbContext.SaveChangesAsync();
        }

        public async Task UpdatePageDownloadedAsync(int pageId, string contentHash)
        {
            Page dbPage = await m_dbContext.Pages.SingleAsync(p => p.PageId == pageId);
            dbPage.CompletedAt = DateTime.UtcNow;
            dbPage.ContentHash = contentHash;
            dbPage.Status = Status.Downloaded;

            await m_dbContext.SaveChangesAsync();
        }

        public async Task UpdatePageDownloadFailedAsync(int pageId)
        {
            Page dbPage = await m_dbContext.Pages.SingleAsync(p => p.PageId == pageId);
            dbPage.CompletedAt = DateTime.UtcNow;
            dbPage.Status = Status.DownloadFailed;

            await m_dbContext.SaveChangesAsync();
        }

        public async Task ClearAllAsync()
        {
            m_dbContext.PageLinks.RemoveRange(m_dbContext.PageLinks);
            m_dbContext.Content.RemoveRange(m_dbContext.Content);
            m_dbContext.Pages.RemoveRange(m_dbContext.Pages);

            await m_dbContext.SaveChangesAsync();
        }
    }
}
