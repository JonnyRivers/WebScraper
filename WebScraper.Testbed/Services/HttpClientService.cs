namespace WebScraper.Testbed.Services
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
