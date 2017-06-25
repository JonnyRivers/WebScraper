namespace WebScraper.Testbed.Actions
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    
    using WebScraper.Testbed.Services;

    internal class MakeRequestAction : IDisposable
    {
        private readonly ILogger<MakeRequestAction> m_logger;
        private readonly IMakeRequestService m_makeRequestService;

        public MakeRequestAction(ILogger<MakeRequestAction> logger, IMakeRequestService makeRequestService)
        {
            m_logger = logger;
            m_makeRequestService = makeRequestService;
        }

        public void Dispose()
        {
            m_makeRequestService.Dispose();
        }

        public async Task<int> RunAsync(string url)
        {
            await m_makeRequestService.MakeRequestAsync(url);

            return 0;
        }
    }
}
