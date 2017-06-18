using System;
using System.Collections.Generic;
using System.Text;

namespace WebScraper.Testbed.Actions
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using WebScraper.Testbed.Data;
    using WebScraper.Testbed.Services;

    internal class ServiceContentAction : IDisposable
    {
        private readonly ILogger<ServiceRequestsAction> m_logger;
        private readonly IDataService m_dataService;

        public ServiceContentAction(ILogger<ServiceRequestsAction> logger, IDataService dataService)
        {
            m_logger = logger;
            m_dataService = dataService;
        }

        public void Dispose()
        {
            m_dataService.Dispose();
        }

        public async Task<int> RunAsync()
        {
            int result = await Task.Run<int>(() => { return 0; });

            return result;
        }
    }
}
