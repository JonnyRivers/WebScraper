namespace WebScraper.Testbed.Tests.Services.Application
{
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using WebScraper.Testbed.Data;
    using WebScraper.Testbed.Services.Application;

    [TestClass]
    public class MakeRequestServiceTests
    {
        [TestMethod]
        public void TestMakeRequest()
        {
            // Arrange
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<MakeRequestService> logger = loggerFactory.CreateLogger<MakeRequestService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                WebScraperContext dbContext = sqliteMemoryWrapper.DbContext;

                string url = "http://www.google.com";
                var service = new MakeRequestService(logger, dbContext);

                // Act
                service.MakeRequestAsync(url).Wait();

                // Assert
                Assert.AreEqual(0, dbContext.Content.CountAsync().Result);
                Assert.AreEqual(0, dbContext.PageLinks.CountAsync().Result);
                Assert.AreEqual(1, dbContext.Pages.CountAsync().Result);

                Page page = dbContext.Pages.FirstAsync().Result;
                Assert.AreEqual(url, page.Url);
                Assert.AreEqual(Status.Pending, page.Status);
            }
        }
    }
}
