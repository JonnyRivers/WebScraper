namespace WebScraper.Testbed.Services.Core
{
    using System.Net.Http;

    // TODO - we could further abstract away the reliance on HttpClient
    internal interface IHttpClientService
    {
        HttpClient Create();
    }
}
