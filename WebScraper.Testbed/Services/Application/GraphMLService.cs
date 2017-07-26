namespace WebScraper.Testbed.Services.Application
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using Microsoft.Extensions.Logging;
    using WebScraper.Testbed.Data;

    public class GraphMLService : IGraphMLService
    {
        private readonly ILogger m_logger;
        private readonly WebScraperContext m_dbContext;

        public GraphMLService(
            ILogger<GraphMLService> logger,
            WebScraperContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Dispose()
        {
            m_dbContext.Dispose();
        }

        public void GenerateGraph(string path)
        {
            List<Page> parsedPages = m_dbContext.Pages.Where(p => p.Status == Status.Parsed).ToList();
            HashSet<int> parsedPageIds = new HashSet<int>(parsedPages.Select(p => p.PageId));
            List<PageLink> parsedPageLinks = m_dbContext.PageLinks
                .Where(l => parsedPageIds.Contains(l.SourcePageId) && parsedPageIds.Contains(l.TargetPageId))
                .ToList();

            List<XElement> nodeElements = parsedPages.Select(p =>
                new XElement("node",
                    new XAttribute("id", p.PageId)
                )
            ).ToList();
            List<XElement> edgeElements = parsedPageLinks.Select(l =>
                new XElement("edge",
                    new XAttribute("id", l.PageLinkId),
                    new XAttribute("source", l.SourcePageId),
                    new XAttribute("target", l.TargetPageId)
                )
            ).ToList();

            var document = new XDocument(
                new XElement("graphml",
                    new XElement("graph",
                        new XAttribute("id", "G"),
                        new XAttribute("edgedefault", "directed"),
                        nodeElements,
                        edgeElements
                    )
                )
            );

            using (System.IO.Stream stream = System.IO.File.Create(path))
            {
                document.Save(stream);
            }
        }
    }
}
