namespace WebScraper.Testbed.Tests.TestServices
{
    using System.Net.Http;
    using WebScraper.Testbed.Services.Core;

    public class MockHttpClientService : IHttpClientService
    {
        private HttpClientHandler m_mockHandler;

        public MockHttpClientService(HttpClientHandler mockHandler)
        {
            m_mockHandler = mockHandler;
        }

        public HttpClient Create()
        {
            return new HttpClient(m_mockHandler);
        }
    }
}
