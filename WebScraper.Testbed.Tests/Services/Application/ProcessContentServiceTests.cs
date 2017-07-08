namespace WebScraper.Testbed.Tests.Services.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using WebScraper.Testbed.Data;
    using WebScraper.Testbed.Services.Application;
    using WebScraper.Testbed.Services.Core;

    [TestClass]
    public class ProcessContentServiceTests
    {
        [TestMethod]
        public void TestProcessContent()
        {
            // Arrange
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<PageParseService> pageParseServiceLogger = loggerFactory.CreateLogger<PageParseService>();
            ILogger<ProcessContentService> processContentServiceLogger = loggerFactory.CreateLogger<ProcessContentService>();

            IPageParseService pageParseService = new PageParseService(pageParseServiceLogger);

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                WebScraperContext dbContext = sqliteMemoryWrapper.DbContext;

                var processContentService = new ProcessContentService(processContentServiceLogger, dbContext, pageParseService);

                var page1 = new Page
                {
                    Url = "http://www.google.com",
                    StartedAt = DateTime.UtcNow,
                    Status = Status.Downloaded,
                    DownloadedAt = DateTime.UtcNow,
                    ContentHash = "1234567812345678"
                };
                dbContext.Pages.Add(page1);

                string secondPageUrl = "http://www.jonnyrivers.com";
                string dataText =
                    "<!DOCTYPE html>" +
                    $"<link href=\"{secondPageUrl}\" />";
                var page1Content = new Content
                {
                    Hash = page1.ContentHash,
                    Data = Encoding.ASCII.GetBytes(dataText)
                };
                dbContext.Content.Add(page1Content);

                dbContext.SaveChanges();

                // Act
                bool serviceProcessResult = processContentService.ProcessContentAsync().Result;

                // Assert
                Assert.IsTrue(serviceProcessResult);

                List<Content> contentRecords = dbContext.Content.ToListAsync().Result;
                List<Page> pages = dbContext.Pages.ToListAsync().Result;
                List<PageLink> pageLinks = dbContext.PageLinks.ToListAsync().Result;

                Assert.AreEqual(1, contentRecords.Count);
                Assert.AreEqual(2, pages.Count);
                Assert.AreEqual(1, pageLinks.Count);

                Assert.AreEqual(Status.Parsed, pages[0].Status);
                Assert.AreEqual(page1.Url, pages[0].Url);
                Assert.AreEqual(page1.ContentHash, pages[0].ContentHash);
                Assert.AreEqual(Status.Pending, pages[1].Status);
                Assert.AreEqual(secondPageUrl, pages[1].Url);

                Assert.AreEqual(pages[0].PageId, pageLinks[0].SourcePageId);
                Assert.AreEqual(pages[1].PageId, pageLinks[0].TargetPageId);
            }
        }

        // Here we are testing a bi-directional link in the presence of a regular link
        // We also test that pages with no links are handled correctly
        [TestMethod]
        public void TestMultiDirectionalPageLinks()
        {
            // Arrange
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<PageParseService> pageParseServiceLogger = loggerFactory.CreateLogger<PageParseService>();
            ILogger<ProcessContentService> processContentServiceLogger = loggerFactory.CreateLogger<ProcessContentService>();

            IPageParseService pageParseService = new PageParseService(pageParseServiceLogger);

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                WebScraperContext dbContext = sqliteMemoryWrapper.DbContext;

                var processContentService = new ProcessContentService(processContentServiceLogger, dbContext, pageParseService);

                // page1<--->page2
                //    \
                //     \
                //      page3

                var page1 = new Page
                {
                    Url = "http://www.jonnyrivers.com",
                    StartedAt = DateTime.UtcNow,
                    Status = Status.Downloaded,
                    DownloadedAt = DateTime.UtcNow,
                    ContentHash = "aaaaaaaaaaaaaaa1"
                };
                dbContext.Pages.Add(page1);

                var page2 = new Page
                {
                    Url = "http://www.jonnyrivers.com/about",
                    StartedAt = DateTime.UtcNow,
                    Status = Status.Downloaded,
                    DownloadedAt = DateTime.UtcNow,
                    ContentHash = "aaaaaaaaaaaaaaa2"
                };
                dbContext.Pages.Add(page2);

                var page3 = new Page
                {
                    Url = "http://www.jonnyrivers.com/contact",
                    StartedAt = DateTime.UtcNow,
                    Status = Status.Downloaded,
                    DownloadedAt = DateTime.UtcNow,
                    ContentHash = "aaaaaaaaaaaaaaa3"
                };
                dbContext.Pages.Add(page3);

                string page1ContentText =
                    "<!DOCTYPE html>" +
                    $"<link href=\"{page2.Url}\" />" +
                    $"<link href=\"{page3.Url}\" />";
                var page1Content = new Content
                {
                    Hash = page1.ContentHash,
                    Data = Encoding.ASCII.GetBytes(page1ContentText)
                };
                dbContext.Content.Add(page1Content);

                string page2ContentText =
                    "<!DOCTYPE html>" +
                    $"<link href=\"{page1.Url}\" />";
                var page2Content = new Content
                {
                    Hash = page2.ContentHash,
                    Data = Encoding.ASCII.GetBytes(page2ContentText)
                };
                dbContext.Content.Add(page2Content);

                string page3ContentText =
                    "<!DOCTYPE html>";// no link
                var page3Content = new Content
                {
                    Hash = page3.ContentHash,
                    Data = Encoding.ASCII.GetBytes(page3ContentText)
                };
                dbContext.Content.Add(page3Content);

                dbContext.SaveChanges();

                // Act
                bool[] serviceCallResults = new bool[3];
                for (int i = 0; i < 3; ++i)
                {
                    serviceCallResults[i] = processContentService.ProcessContentAsync().Result;
                }

                // Assert

                Assert.IsTrue(serviceCallResults.All(x => x));

                // page1<--->page2
                //    \
                //     \
                //      page3

                List<Content> contentRecords = dbContext.Content.ToListAsync().Result;
                List<Page> pages = dbContext.Pages.ToListAsync().Result;
                List<PageLink> pageLinks = dbContext.PageLinks.ToListAsync().Result;

                Assert.AreEqual(3, contentRecords.Count);
                Assert.AreEqual(3, pages.Count);
                Assert.AreEqual(3, pageLinks.Count);

                Assert.AreEqual(Status.Parsed, pages[0].Status);
                Assert.AreEqual(page1.Url, pages[0].Url);
                Assert.AreEqual(page1.ContentHash, pages[0].ContentHash);
                Assert.AreEqual(Status.Parsed, pages[1].Status);
                Assert.AreEqual(page2.Url, pages[1].Url);
                Assert.AreEqual(page2.ContentHash, pages[1].ContentHash);
                Assert.AreEqual(Status.Parsed, pages[2].Status);
                Assert.AreEqual(page3.Url, pages[2].Url);
                Assert.AreEqual(page3.ContentHash, pages[2].ContentHash);

                Assert.AreEqual(pages[0].PageId, pageLinks[0].SourcePageId);
                Assert.AreEqual(pages[1].PageId, pageLinks[0].TargetPageId);
                Assert.AreEqual(pages[0].PageId, pageLinks[1].SourcePageId);
                Assert.AreEqual(pages[2].PageId, pageLinks[1].TargetPageId);
                Assert.AreEqual(pages[1].PageId, pageLinks[2].SourcePageId);
                Assert.AreEqual(pages[0].PageId, pageLinks[2].TargetPageId);
            }
        }

        [TestMethod]
        [Ignore]
        public void TestProcessContentPerformance()
        {
            // Arrange
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<PageParseService> pageParseServiceLogger = loggerFactory.CreateLogger<PageParseService>();
            ILogger<ProcessContentService> processContentServiceLogger = loggerFactory.CreateLogger<ProcessContentService>();

            IPageParseService pageParseService = new PageParseService(pageParseServiceLogger);

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                WebScraperContext dbContext = sqliteMemoryWrapper.DbContext;

                var processContentService = new ProcessContentService(processContentServiceLogger, dbContext, pageParseService);

                int numPages = 100;
                int numLinks = 100;
                for (int pageIndex = 0; pageIndex < numPages; ++pageIndex)
                {
                    var page = new Page
                    {
                        Url = $"http://www.jonnyrivers.com/{pageIndex}",
                        StartedAt = DateTime.UtcNow,
                        Status = Status.Downloaded,
                        DownloadedAt = DateTime.UtcNow,
                        ContentHash = pageIndex.ToString()
                    };

                    dbContext.Pages.Add(page);

                    StringBuilder contentBuilder = new StringBuilder();
                    contentBuilder.AppendLine("<!DOCTYPE html>");
                    
                    for (int linkIndex = 0; linkIndex < numLinks; ++linkIndex)
                    {
                        contentBuilder.AppendLine($"<link href=\"http://www.jonnyrivers.com/{pageIndex}/{linkIndex}\" />");
                    }
                    var pageContent = new Content
                    {
                        Hash = page.ContentHash,
                        Data = Encoding.ASCII.GetBytes(contentBuilder.ToString())
                    };
                    dbContext.Content.Add(pageContent);
                }

                dbContext.SaveChanges();

                // Act
                bool[] serviceCallResults = new bool[numPages];
                for (int pageIndex = 0; pageIndex < numPages; ++pageIndex)
                {
                    serviceCallResults[pageIndex] = processContentService.ProcessContentAsync().Result;
                }

                // Assert
                Assert.IsTrue(serviceCallResults.All(x => x));
                Assert.AreEqual(numPages + numPages * numLinks, dbContext.Pages.CountAsync().Result);
                Assert.AreEqual(numPages * numLinks, dbContext.PageLinks.CountAsync().Result);
                Assert.AreEqual(numPages, dbContext.Content.CountAsync().Result);
            }
        }
    }
}
