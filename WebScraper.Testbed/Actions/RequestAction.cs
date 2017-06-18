namespace WebScraper.Testbed.Actions
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using WebScraper.Testbed.Data;
    using WebScraper.Testbed.Services;

    internal class RequestAction : IDisposable
    {
        private readonly ILogger<RequestAction> m_logger;
        private readonly IDataService m_dataService;

        public RequestAction(ILogger<RequestAction> logger, IDataService dataService)
        {
            m_logger = logger;
            m_dataService = dataService;
        }

        public void Dispose()
        {
            m_dataService.Dispose();
        }

        public async Task<int> RunAsync(string url)
        {
            var page = new Page
            {
                Url = url,
                Status = Status.Pending,
                RequestedAt = DateTime.UtcNow,
                ContentHash = String.Empty
            };
            await m_dataService.AddPageAsync(page);

            return 0;
        }
    }
}
