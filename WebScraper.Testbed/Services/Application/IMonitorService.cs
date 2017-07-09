namespace WebScraper.Testbed.Services.Application
{
    using System;
    using System.Threading.Tasks;

    interface IMonitorService : IDisposable
    {
        void Report();
    }
}
