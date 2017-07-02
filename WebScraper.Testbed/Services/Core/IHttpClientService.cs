namespace WebScraper.Testbed.Services.Core
{
    using System.Net.Http;

    // TODO - we could further abstract away the reliance on HttpClient
    public interface IHttpClientService
    {
        HttpClient Create();
    }
}
