namespace WebScraper.Testbed.Services.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using WebScraper.Testbed.Content;

    public class PageParseService : IPageParseService
    {
        private readonly ILogger m_logger;

        public PageParseService(ILogger<PageParseService> logger)
        {
            m_logger = logger;
        }

        public WebPageContent ParseWebPage(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(stream);

            var linkNodes = new List<HtmlAgilityPack.HtmlNode>();
            linkNodes.AddRange(doc.DocumentNode.Descendants("link"));
            linkNodes.AddRange(doc.DocumentNode.Descendants("a"));

            IEnumerable<string> hrefValues = linkNodes
                .Select(e => e.GetAttributeValue("href", null))
                .Where(v => !string.IsNullOrEmpty(v) && v.StartsWith("http", StringComparison.OrdinalIgnoreCase));
            var hrefValueSet = new HashSet<string>(hrefValues);

            IEnumerable<WebPageLink> webPageLinks = hrefValueSet.Select(h => new WebPageLink(h));

            return new WebPageContent(webPageLinks);
        }
    }
}
