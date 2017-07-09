namespace WebScraper.Testbed.Tests.Services.Application
{
    using System;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using WebScraper.Testbed.Data;
    using WebScraper.Testbed.Services.Application;

    using Infrastructure;

    [TestClass]
    public class ResetDataServiceTests
    {
        [TestMethod]
        public void TestResetData()
        {
            // Arrange
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<ResetDataService> logger = loggerFactory.CreateLogger<ResetDataService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                WebScraperContext dbContext = sqliteMemoryWrapper.DbContext;

                // Arrange
                var page = new Page
                {
                    Url = "http://www.google.com",
                    StartedAt = DateTime.UtcNow,
                    Status = Status.Pending
                };
                dbContext.Pages.Add(page);

                PageLink pageLink = new PageLink
                {
                    SourcePageId = 1,
                    TargetPageId = 2
                };
                dbContext.PageLinks.Add(pageLink);

                Content content = new Content
                {
                    Hash = "deadbeef12345678",
                    Data = new byte[] { 1, 2, 3, 4, 5 }
                };
                dbContext.Content.Add(content);

                dbContext.SaveChanges();

                var service = new ResetDataService(logger, dbContext);

                // Act
                service.ResetDataAsync().Wait();

                // Assert
                Assert.AreEqual(0, dbContext.Content.CountAsync().Result);
                Assert.AreEqual(0, dbContext.PageLinks.CountAsync().Result);
                Assert.AreEqual(0, dbContext.Pages.CountAsync().Result);
            }
        }
    }
}
