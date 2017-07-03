namespace WebScraper.Testbed.Tests.Services.Application
{
    using System;

    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using WebScraper.Testbed.Data;
    using WebScraper.Testbed.Services.Application;
    using WebScraper.Testbed.Services.Core;
    using System.Text;

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

            IPageParseService pageParseService = new PageParseService(pageParseServiceLogger);// TODO - fake this

            // TODO - simplify the sqliteMemoryConnection setup to a single IDisposable object
            var sqliteMemoryConnection = new SqliteConnection("DataSource=:memory:");
            sqliteMemoryConnection.Open();

            try
            {
                var dbContextOptionsBuilder = new DbContextOptionsBuilder<WebScraperContext>()
                    .UseSqlite(sqliteMemoryConnection);
                using (var dbContext = new WebScraperContext(dbContextOptionsBuilder.Options))
                {
                    dbContext.Database.EnsureCreated();
                    var processContentService = new ProcessContentService(processContentServiceLogger, dbContext, pageParseService);

                    var page = new Page
                    {
                        Url = "http://www.google.com",
                        StartedAt = DateTime.UtcNow,
                        Status = Status.Downloaded,
                        CompletedAt = DateTime.UtcNow,
                        ContentHash = "1234567812345678"
                    };
                    dbContext.Pages.Add(page);

                    string dataText =
                        "<!DOCTYPE html>" +
                        "<link rel = \"dns - prefetch\" href=\"https://assets.guim.co.uk/\"/>";
                    var content = new Content
                    {
                        Hash = "1234567812345678",
                        Data = Encoding.ASCII.GetBytes(dataText)
                    };
                    dbContext.Content.Add(content);

                    dbContext.SaveChanges();

                    // Act
                    processContentService.ProcessContentAsync().Wait();

                    // Assert
                    // TODO - add more asserts once we have faked dependencies
                    Assert.AreEqual(1, dbContext.Content.CountAsync().Result);
                    Assert.AreEqual(2, dbContext.Pages.CountAsync().Result);
                    Assert.AreEqual(1, dbContext.PageLinks.CountAsync().Result);

                    Assert.AreEqual(Status.Parsed, dbContext.Pages.FirstAsync().Result.Status);
                }
            }
            finally
            {
                sqliteMemoryConnection.Close();
            }
        }
    }
}
