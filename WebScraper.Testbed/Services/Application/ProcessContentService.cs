namespace WebScraper.Testbed.Services.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using WebScraper.Testbed.Content;
    using WebScraper.Testbed.Data;
    using WebScraper.Testbed.Services.Core;

    public class ProcessContentService : IProcessContentService
    {
        private readonly ILogger m_logger;
        private readonly WebScraperContext m_dbContext;
        private readonly IPageParseService m_pageParseService;

        public ProcessContentService(
            ILogger<ProcessContentService> logger,
            WebScraperContext dbContext,
            IPageParseService pageParseService)
        {
            m_logger = logger;
            m_dbContext = dbContext;
            m_pageParseService = pageParseService;
        }

        public void Dispose()
        {
            m_dbContext.Dispose();
        }

        public async Task<bool> ProcessContentAsync()
        {
            m_logger.LogInformation("Finding next available downloaded page.");

            Page nextPage = await m_dbContext.Pages
                .FirstOrDefaultAsync(x => x.Status == Status.Downloaded);
            
            if (nextPage == null)
            {
                m_logger.LogInformation("No pages are available to process.");

                return false;
            }

            nextPage.StartedAt = DateTime.UtcNow;
            nextPage.Status = Status.Parsing;

            await m_dbContext.SaveChangesAsync();

            try
            {
                m_logger.LogInformation($"Parsing content from '{nextPage.Url}' with hash '{nextPage.ContentHash}'");

                Content contentRecord = await m_dbContext.Content.FirstOrDefaultAsync(x => x.Hash == nextPage.ContentHash);
                if (contentRecord == null)
                {
                    throw new InvalidOperationException($"Hash {nextPage.ContentHash} not found");
                }

                using (var contentStream = new MemoryStream(contentRecord.Data))
                {
                    WebPageContent webPageContent = m_pageParseService.ParseWebPage(contentStream);

                    // For performance reasons we do two passes
                    // One transactions per pass

                    // Pass 1 - create pages
                    var linkedPagesByUrl = new Dictionary<string, Page>();
                    bool pagesAdded = false;
                    foreach (WebPageLink link in webPageContent.Links)
                    {
                        Page linkedPage = await m_dbContext.Pages.FirstOrDefaultAsync(p => p.Url == link.Value);

                        if(linkedPage == null)
                        {
                            linkedPage = new Page
                            {
                                Url = link.Value,
                                Status = Status.Pending,
                                RequestedAt = DateTime.UtcNow,
                                ContentHash = String.Empty
                            };
                            await m_dbContext.Pages.AddAsync(linkedPage);

                            pagesAdded = true;
                        }

                        linkedPagesByUrl.Add(link.Value, linkedPage);
                    }

                    if(pagesAdded)
                    {
                        // Have the db generate page IDs for new pages
                        await m_dbContext.SaveChangesAsync();
                    }

                    // Pass 2 - create links to new pages
                    foreach (WebPageLink link in webPageContent.Links)
                    {
                        var pageLink = new PageLink
                        {
                            SourcePageId = nextPage.PageId,
                            TargetPageId = linkedPagesByUrl[link.Value].PageId
                        };
                        await m_dbContext.PageLinks.AddAsync(pageLink);
                    }
                }

                nextPage.ParsedAt = DateTime.UtcNow;
                nextPage.Status = Status.Parsed;

                await m_dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex.ToString());

                nextPage.ParsedAt = DateTime.UtcNow;
                nextPage.Status = Status.ParseFailed;

                await m_dbContext.SaveChangesAsync();
            }

            return true;
        }
    }
}
