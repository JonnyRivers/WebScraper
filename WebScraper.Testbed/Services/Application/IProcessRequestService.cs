namespace WebScraper.Testbed.Services.Application
{
    using System;
    using System.Threading.Tasks;

    internal interface IProcessRequestService : IDisposable
    {
        Task<bool> ProcessRequestAsync();
    }
}
