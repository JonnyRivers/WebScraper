namespace WebScraper.Testbed.Services
{
    using System;
    using System.Threading.Tasks;

    using WebScraper.Testbed.Data;

    // TODO - this is a catch-all but could be broken up
    internal interface IDataService : IDisposable
    {
        // Create
        Task AddPageAsync(Page page);
        Task EnsureContentAsync(Content content);

        // Read
        Task<Page> FirstOrDefaultPendingPageAsync();

        // Update
        Task UpdatePageDownloadingAsync(int pageId);
        Task UpdatePageDownloadedAsync(int pageId, string contentHash);
        Task UpdatePageDownloadFailedAsync(int pageId);

        // Delete
        Task ClearAllAsync();
    }
}
