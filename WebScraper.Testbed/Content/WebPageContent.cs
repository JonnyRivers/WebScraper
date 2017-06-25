namespace WebScraper.Testbed.Content
{
    using System.Collections.Generic;

    public class WebPageContent
    {
        public WebPageContent(IEnumerable<WebPageLink> links)
        {
            Links = links;
        }

        public IEnumerable<WebPageLink> Links { get; }
    }
}
