namespace WebScraper.Testbed.Services.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using WebScraper.Testbed.Data;

    // TODO - this is a catch-all but could be broken up
    // TODO - this breaks the unit of work pattern in EF and prevents multiple changes in a given transaction
    //        maybe it should just thinly wrap the EF model
    //        maybe we should just use EF directly - will that affect testability?
    internal interface IDataService : IDisposable
    {
        // Create
        Task AddPageAsync(Page page);
        Task AddPageLinkAsync(PageLink pageLink);
        Task EnsureContentAsync(Content content);// candidate for separate service?

        // Read
        IEnumerable<Page> GetAllPages();
        Task<Page> FirstOrDefaultPageByUrlAsync(string url);

        Task<Page> FirstOrDefaultPendingPageAsync();
        Task<Page> FirstOrDefaultDownloadedPageAsync();

        Task<byte[]> GetContentDataAsync(string hash);


        // Update - this all seems like state machine logic - is this a sensible location
        Task UpdatePagePendingAsync(int pageId);
        Task UpdatePageDownloadingAsync(int pageId);
        Task UpdatePageDownloadedAsync(int pageId, string contentHash);
        Task UpdatePageDownloadFailedAsync(int pageId);
        Task UpdatePageParsingAsync(int pageId);
        Task UpdatePageParsedAsync(int pageId);
        Task UpdatePageParseFailedAsync(int pageId);

        // Delete
        Task ClearAllAsync();
    }
}
