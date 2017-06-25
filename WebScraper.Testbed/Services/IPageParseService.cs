namespace WebScraper.Testbed.Services
{
    using System.IO;

    using WebScraper.Testbed.Content;

    public interface IPageParseService
    {
        // TODO - should this be async?
        WebPageContent ParseWebPage(Stream stream);
    }
}
