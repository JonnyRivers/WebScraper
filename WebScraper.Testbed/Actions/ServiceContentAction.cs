using System;
using System.Collections.Generic;
using System.Text;

namespace WebScraper.Testbed.Actions
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using WebScraper.Testbed.Data;
    using WebScraper.Testbed.Services;

    internal class ServiceContentAction : IDisposable
    {
        private readonly WebScraperContext m_dbContext;// TODO - make a data service
        private readonly ILogger<ServiceRequestsAction> m_logger;

        public ServiceContentAction(WebScraperContext dbContext, ILogger<ServiceRequestsAction> logger, IHashService hashService)
        {
            m_dbContext = dbContext;
            m_logger = logger;
        }

        public void Dispose()
        {
            m_dbContext.Dispose();
        }

        public async Task<int> RunAsync()
        {
            int result = await Task.Run<int>(() => { return 0; });

            return result;
        }
    }
}
