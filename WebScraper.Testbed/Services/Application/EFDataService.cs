namespace WebScraper.Testbed.Services.Application
{
    using System;
    using System.Collections.Generic;
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

        public async Task AddPageLinkAsync(PageLink pageLink)
        {
            await m_dbContext.PageLinks.AddAsync(pageLink);

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

        public IEnumerable<Page> GetAllPages()
        {
            return m_dbContext.Pages;
        }

        public async Task<Page> FirstOrDefaultPageByUrlAsync(string url)
        {
            return await m_dbContext.Pages.FirstOrDefaultAsync(p => p.Url == url);
        }

        public async Task<byte[]> GetContentDataAsync(string contentHash)
        {
            Content existingRecord = await m_dbContext.Content.FirstOrDefaultAsync(x => x.Hash == contentHash);
            if (existingRecord == null)
            {
                throw new InvalidOperationException($"Hash {contentHash} not found");
            }

            return existingRecord.Data;
        }

        public async Task<Page> FirstOrDefaultPendingPageAsync()
        {
            return await m_dbContext.Pages
                .FirstOrDefaultAsync(x => x.Status == Status.Pending);
        }

        public async Task<Page> FirstOrDefaultDownloadedPageAsync()
        {
            return await m_dbContext.Pages
                .FirstOrDefaultAsync(x => x.Status == Status.Downloaded);
        }

        public async Task UpdatePagePendingAsync(int pageId)
        {
            Page dbPage = await m_dbContext.Pages.SingleAsync(p => p.PageId == pageId);
            dbPage.ContentHash = string.Empty;
            dbPage.StartedAt = null;
            dbPage.CompletedAt = null;
            dbPage.Status = Status.Pending;

            await m_dbContext.SaveChangesAsync();
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

        public async Task UpdatePageParsingAsync(int pageId)
        {
            Page dbPage = await m_dbContext.Pages.SingleAsync(p => p.PageId == pageId);
            dbPage.Status = Status.Parsing;

            await m_dbContext.SaveChangesAsync();
        }

        public async Task UpdatePageParsedAsync(int pageId)
        {
            Page dbPage = await m_dbContext.Pages.SingleAsync(p => p.PageId == pageId);
            dbPage.CompletedAt = DateTime.UtcNow;
            dbPage.Status = Status.Parsed;

            await m_dbContext.SaveChangesAsync();
        }

        public async Task UpdatePageParseFailedAsync(int pageId)
        {
            Page dbPage = await m_dbContext.Pages.SingleAsync(p => p.PageId == pageId);
            dbPage.CompletedAt = DateTime.UtcNow;
            dbPage.Status = Status.ParseFailed;

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
