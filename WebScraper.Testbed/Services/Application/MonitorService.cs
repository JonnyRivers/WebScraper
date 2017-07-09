namespace WebScraper.Testbed.Services.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;
    using WebScraper.Testbed.Data;

    public class MonitorService : IMonitorService
    {
        private readonly ILogger m_logger;
        private readonly WebScraperContext m_dbContext;

        public MonitorService(
            ILogger<ProcessContentService> logger,
            WebScraperContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Dispose()
        {
            m_dbContext.Dispose();
        }

        public void Report()
        {
            var pageCountByStatus = new Dictionary<Status, int>();
            foreach(Status status in Enum.GetValues(typeof(Status)))
            {
                int pageCount = m_dbContext.Pages.Count(p => p.Status == status);
                pageCountByStatus.Add(status, pageCount);
            }

            int numPages = pageCountByStatus.Values.Sum();
            int numPageLinks = m_dbContext.PageLinks.Count();
            int numContentRecords = m_dbContext.Content.Count();

            Console.Clear();
            Console.WriteLine("Pages by status");
            Console.WriteLine();
            foreach (Status status in Enum.GetValues(typeof(Status)))
            {
                Console.WriteLine($"{status}: {pageCountByStatus[status]}");
            }

            Console.WriteLine();
            Console.WriteLine($"There are {numPages} pages");
            Console.WriteLine($"There are {numPageLinks} page links");
            Console.WriteLine($"There are {numContentRecords} content records");
        }
    }
}

