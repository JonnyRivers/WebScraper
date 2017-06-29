namespace WebScraper.Testbed.Services.Core
{
    using System.Net.Http;

    public class HttpClientService : IHttpClientService
    {
        public HttpClient Create()
        {
            return new HttpClient();
        }
    }
}
