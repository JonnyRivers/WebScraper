﻿namespace WebScraper.Testbed.Actions
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using WebScraper.Testbed.Data;

    internal class RequestAction : IDisposable
    {
        private readonly WebScraperContext m_dbContext;// TODO - make a data service
        private readonly ILogger<RequestAction> m_logger;

        public RequestAction(WebScraperContext dbContext, ILogger<RequestAction> logger)
        {
            m_dbContext = dbContext;
            m_logger = logger;
        }

        public void Dispose()
        {
            m_dbContext.Dispose();
        }

        public async Task<int> RunAsync(string url)
        {
            var pageRequest = new Page
            {
                Url = url,
                Status = Status.Pending,
                RequestedAt = DateTime.UtcNow,
                ContentHash = String.Empty
            };
            await m_dbContext.Pages.AddAsync(pageRequest);

            await m_dbContext.SaveChangesAsync();

            return 0;
        }
    }
}
