namespace WebScraper.Testbed.Services
{
    using System;
    using System.Threading.Tasks;

    internal interface IMakeRequestService : IDisposable
    {
        Task MakeRequestAsync(string url);
    }
}
