namespace WebScraper.Testbed.Actions
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using WebScraper.Testbed.Services;

    internal class ResetAction : IDisposable
    {
        private readonly ILogger<RequestAction> m_logger;
        private readonly IDataService m_dataService;

        public ResetAction(ILogger<RequestAction> logger, IDataService dataService)
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
            await m_dataService.ClearAllAsync();

            return 0;
        }
    }
}
