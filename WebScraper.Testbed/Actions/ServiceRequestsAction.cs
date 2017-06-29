namespace WebScraper.Testbed.Actions
{
    using System;
    using System.Threading.Tasks;
    using System.Net.Http;
    
    using Microsoft.Extensions.Logging;

    using WebScraper.Testbed.Data;
    using WebScraper.Testbed.Services.Core;
    using WebScraper.Testbed.Services.Application;

    internal class ServiceRequestsAction : IDisposable
    {
        private readonly ILogger<ServiceRequestsAction> m_logger;
        private readonly IDataService m_dataService;
        private readonly IHashService m_hashService;
        private readonly IHttpClientService m_httpClientService;

        public ServiceRequestsAction(
            ILogger<ServiceRequestsAction> logger, 
            IDataService dataService, 
            IHashService hashService,
            IHttpClientService httpClientService)
        {
            m_logger = logger;
            m_dataService = dataService;
            m_hashService = hashService;
            m_httpClientService = httpClientService;
        }

        public void Dispose()
        {
            m_dataService.Dispose();
        }

        public async Task<int> RunAsync()
        {
            while (true)
            {
                Page nextPage = await m_dataService.FirstOrDefaultPendingPageAsync();

                if (nextPage != null)
                {
                    await m_dataService.UpdatePageDownloadingAsync(nextPage.PageId);

                    try
                    {
                        m_logger.LogInformation($"Processing request for {nextPage.Url}");

                        // get the content
                        HttpClient client = m_httpClientService.Create();
                        HttpResponseMessage response = client.GetAsync(nextPage.Url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            byte[] contentData = await response.Content.ReadAsByteArrayAsync();
                            var contentRecord = new Content
                            {
                                Hash = m_hashService.GenerateHash(contentData),
                                Data = contentData
                            };

                            await m_dataService.EnsureContentAsync(contentRecord);

                            await m_dataService.UpdatePageDownloadedAsync(nextPage.PageId, contentRecord.Hash);
                        }
                        else
                        {
                            await m_dataService.UpdatePageDownloadFailedAsync(nextPage.PageId);
                        }
                    }
                    catch (Exception ex)
                    {
                        m_logger.LogError(ex.ToString());

                        await m_dataService.UpdatePageDownloadFailedAsync(nextPage.PageId);
                    }
                }
                else
                {
                    m_logger.LogInformation("No pending requests.  Sleeping.");
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }
    }
}
