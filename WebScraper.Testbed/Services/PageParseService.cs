﻿namespace WebScraper.Testbed.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using WebScraper.Testbed.Content;

    public class PageParseService : IPageParseService
    {
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
