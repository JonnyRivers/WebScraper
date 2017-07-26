namespace WebScraper.Testbed.Services.Application
{
    using System;

    interface IGraphMLService : IDisposable
    {
        void GenerateGraph(string path);
    }
}
