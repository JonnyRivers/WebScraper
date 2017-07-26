namespace WebScraper.Testbed.Services.Application
{
    using System;

    interface IMonitorService : IDisposable
    {
        void Report();
    }
}
