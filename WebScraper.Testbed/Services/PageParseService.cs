namespace WebScraper.Testbed.Services
{
    using System;

    using WebScraper.Testbed.Content;

    public class PageParseService : IPageParseService
    {
        public WebPageContent ParseWebPage(byte[] contentData)
        {
            if (contentData == null)
            {
                throw new ArgumentNullException(nameof(contentData));
            }

            string contentText = System.Text.Encoding.UTF8.GetString(contentData);

            // parse the links

            throw new NotImplementedException();
        }
    }
}
