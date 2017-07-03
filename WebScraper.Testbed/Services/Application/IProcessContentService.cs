namespace WebScraper.Testbed.Services.Application
{
    using System;
    using System.Threading.Tasks;

    internal interface IProcessContentService : IDisposable
    {
        Task<bool> ProcessContentAsync();
    }
}
