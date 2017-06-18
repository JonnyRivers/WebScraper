namespace WebScraper.Testbed.Actions
{
    using System;
    using System.Threading.Tasks;
    using System.Net.Http;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using WebScraper.Testbed.Data;
    using WebScraper.Testbed.Services;

    internal class ServiceRequestsAction : IDisposable
    {
        private readonly WebScraperContext m_dbContext;// TODO - make a data service
        private readonly ILogger<ServiceRequestsAction> m_logger;
        private readonly IHashService m_hashService;

        public ServiceRequestsAction(WebScraperContext dbContext, ILogger<ServiceRequestsAction> logger, IHashService hashService)
        {
            m_dbContext = dbContext;
            m_logger = logger;
            m_hashService = hashService;
        }

        public void Dispose()
        {
            m_dbContext.Dispose();
        }

        public async Task<int> RunAsync()
        {
            while (true)
            {
                Page nextPage = await m_dbContext.Pages
                    .FirstOrDefaultAsync(x => x.Status == Status.Pending);
                if (nextPage != null)
                {
                    // TODO - offload blocks like this to a service?
                    nextPage.StartedAt = DateTime.UtcNow;
                    nextPage.Status = Status.Downloading;
                    await m_dbContext.SaveChangesAsync();

                    try
                    {
                        m_logger.LogInformation($"Processing request for {nextPage.Url}");

                        // get the content
                        var client = new HttpClient();// todo - inject this
                        HttpResponseMessage response = client.GetAsync(nextPage.Url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            byte[] contentData = await response.Content.ReadAsByteArrayAsync();
                            string hash = m_hashService.GenerateHash(contentData);

                            // TODO - offload blocks like this to a service?
                            Content contentRecord = await m_dbContext.Content.SingleOrDefaultAsync(x => x.Hash == hash);
                            if (contentRecord == null)
                            {
                                contentRecord = new Content
                                {
                                    Hash = hash,
                                    Data = contentData
                                };

                                m_dbContext.Content.Add(contentRecord);
                            }

                            // TODO - offload blocks like this to a service?
                            nextPage.CompletedAt = DateTime.UtcNow;
                            nextPage.Status = Status.Downloaded;
                            nextPage.ContentHash = contentRecord.Hash;

                            await m_dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            // TODO - offload blocks like this to a service?
                            nextPage.CompletedAt = DateTime.UtcNow;
                            nextPage.Status = Status.DownloadFailed;
                            nextPage.ContentHash = String.Empty;

                            await m_dbContext.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        m_logger.LogError(ex.ToString());

                        // TODO - offload blocks like this to a service?
                        nextPage.CompletedAt = DateTime.UtcNow;
                        nextPage.Status = Status.DownloadFailed;
                        nextPage.ContentHash = String.Empty;

                        await m_dbContext.SaveChangesAsync();
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
