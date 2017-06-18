using System;
using System.Collections.Generic;
using System.Text;

namespace WebScraper.Testbed.Content
{
    public class WebPageContent
    {
        public WebPageContent(IEnumerable<WebPageLink> links)
        {
            Links = links;
        }

        public IEnumerable<WebPageLink> Links { get; }
    }
}
