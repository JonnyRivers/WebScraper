namespace WebScraper.Testbed.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using WebScraper.Testbed.Data;
    using WebScraper.Testbed.Services.Core;
    using WebScraper.Testbed.Services.Application;
    using WebScraper.Testbed.Content;

    internal class ServiceContentAction : IDisposable
    {
        private readonly ILogger<ServiceRequestsAction> m_logger;
        private readonly IDataService m_dataService;
        private readonly IPageParseService m_pageParseService;

        public ServiceContentAction(ILogger<ServiceRequestsAction> logger, IDataService dataService, IPageParseService pageParseService)
        {
            m_logger = logger;
            m_dataService = dataService;
            m_pageParseService = pageParseService;
        }

        public void Dispose()
        {
            m_dataService.Dispose();
        }

        public async Task<int> RunAsync()
        {
            while (true)
            {
                Page nextPage = await m_dataService.FirstOrDefaultDownloadedPageAsync();

                if (nextPage != null)
                {
                    await m_dataService.UpdatePageParsingAsync(nextPage.PageId);

                    try
                    {
                        m_logger.LogInformation($"Parsing content from {nextPage.Url}");

                        byte[] contentData = await m_dataService.GetContentDataAsync(nextPage.ContentHash);

                        using (var contentStream = new MemoryStream(contentData))
                        {
                            WebPageContent webPageContent = m_pageParseService.ParseWebPage(contentStream);

                            Dictionary<string, Page> pagesByUrl = m_dataService.GetAllPages().ToDictionary(p => p.Url, p => p);

                            foreach (WebPageLink link in webPageContent.Links)
                            {
                                // TODO - account for casing in URL
                                if (pagesByUrl.ContainsKey(link.Value))
                                {
                                    var pageLink = new PageLink
                                    {
                                        SourcePageId = nextPage.PageId,
                                        TargetPageId = pagesByUrl[link.Value].PageId
                                    };
                                    await m_dataService.AddPageLinkAsync(pageLink);
                                }
                                else
                                {
                                    var page = new Page
                                    {
                                        Url = link.Value,
                                        Status = Status.Pending,
                                        RequestedAt = DateTime.UtcNow,
                                        ContentHash = String.Empty
                                    };
                                    await m_dataService.AddPageAsync(page);

                                    var pageLink = new PageLink
                                    {
                                        SourcePageId = nextPage.PageId,
                                        TargetPageId = page.PageId
                                    };
                                    await m_dataService.AddPageLinkAsync(pageLink);
                                }
                            }
                        }

                        await m_dataService.UpdatePageParsedAsync(nextPage.PageId);
                    }
                    catch (Exception ex)
                    {
                        m_logger.LogError(ex.ToString());

                        await m_dataService.UpdatePageParseFailedAsync(nextPage.PageId);
                    }
                }
                else
                {
                    m_logger.LogInformation("No pending content.  Sleeping.");
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }
    }
}
