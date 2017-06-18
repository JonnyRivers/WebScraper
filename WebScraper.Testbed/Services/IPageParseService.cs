namespace WebScraper.Testbed.Services
{
    using WebScraper.Testbed.Content;

    public interface IPageParseService
    {
        WebPageContent ParseWebPage(byte[] contentData);
    }
}
