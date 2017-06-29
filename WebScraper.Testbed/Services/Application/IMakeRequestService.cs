namespace WebScraper.Testbed.Services.Application
{
    using System;
    using System.Threading.Tasks;

    internal interface IMakeRequestService : IDisposable
    {
        Task MakeRequestAsync(string url);
    }
}
