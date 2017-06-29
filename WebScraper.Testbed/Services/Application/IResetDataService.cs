namespace WebScraper.Testbed.Services.Application
{
    using System;
    using System.Threading.Tasks;

    internal interface IResetDataService : IDisposable
    {
        Task ResetDataAsync();
    }
}
