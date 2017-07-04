namespace WebScraper.Testbed.Services.Application
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    using Microsoft.EntityFrameworkCore;

    using WebScraper.Testbed.Data;
    using WebScraper.Testbed.Services.Core;

    public class ProcessRequestService : IProcessRequestService
    {
        private readonly ILogger m_logger;
        private readonly WebScraperContext m_dbContext;
        private readonly IHttpClientService m_httpClientService;
        private readonly IHashService m_hashService;

        public ProcessRequestService(
            ILogger<ProcessRequestService> logger, 
            WebScraperContext dbContext, 
            IHttpClientService httpClientService,
            IHashService hashService)
        {
            m_logger = logger;
            m_dbContext = dbContext;
            m_httpClientService = httpClientService;
            m_hashService = hashService;
        }

        public void Dispose()
        {
            m_dbContext.Dispose();
        }

        // TODO - this function is too long
        public async Task<bool> ProcessRequestAsync()
        {
            m_logger.LogInformation("Finding next available pending page.");
            Page nextPage = await m_dbContext.Pages
                .FirstOrDefaultAsync(x => x.Status == Status.Pending);

            if (nextPage == null)
            {
                m_logger.LogInformation("No pages are available to process.");

                return false;
            }

            nextPage.StartedAt = DateTime.UtcNow;
            nextPage.Status = Status.Downloading;

            await m_dbContext.SaveChangesAsync();

            m_logger.LogInformation($"Updated status for {nextPage.Url} to Downloading");

            try
            {
                m_logger.LogInformation($"Processing request for {nextPage.Url}");

                // get the content
                HttpClient client = m_httpClientService.Create();
                HttpResponseMessage response = client.GetAsync(nextPage.Url).Result;

                if (response.IsSuccessStatusCode)
                {
                    m_logger.LogInformation($"Successfully downloaded {nextPage.Url}");

                    byte[] contentData = await response.Content.ReadAsByteArrayAsync();
                    var contentRecord = new Content
                    {
                        Hash = m_hashService.GenerateHash(contentData),
                        Data = contentData
                    };

                    Content existingRecord = await m_dbContext.Content.FirstOrDefaultAsync(x => x.Hash == contentRecord.Hash);
                    if (existingRecord == null)
                    {
                        m_logger.LogInformation($"Created content record for {nextPage.Url} with hash {contentRecord.Hash}");
                        await m_dbContext.Content.AddAsync(contentRecord);

                        await m_dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        m_logger.LogInformation($"Content record for {nextPage.Url} with hash {contentRecord.Hash} already exists");
                    }

                    nextPage.DownloadedAt = DateTime.UtcNow;
                    nextPage.ContentHash = contentRecord.Hash;
                    nextPage.Status = Status.Downloaded;

                    await m_dbContext.SaveChangesAsync();

                    m_logger.LogInformation($"Updated status for {nextPage.Url} to Downloaded");
                }
                else
                {
                    await UpdatePageFailedAsync(nextPage);
                }
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex.ToString());

                await UpdatePageFailedAsync(nextPage);
            }

            return true;
        }

        private async Task UpdatePageFailedAsync(Page page)
        {
            page.DownloadedAt = DateTime.UtcNow;
            page.Status = Status.DownloadFailed;

            await m_dbContext.SaveChangesAsync();

            m_logger.LogInformation($"Updated status for {page.Url} to DownloadFailed");
        }
    }
}
