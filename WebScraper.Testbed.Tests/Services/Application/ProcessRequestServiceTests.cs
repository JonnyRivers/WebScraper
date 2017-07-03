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

    [TestClass]
    public class ProcessRequestServiceTests
    {
        [TestMethod]
        public void TestProcessRequest()
        {
            // Arrange
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<MD5HashService> hashLogger = loggerFactory.CreateLogger<MD5HashService>();
            ILogger<ProcessRequestService> processRequestServiceLogger = loggerFactory.CreateLogger<ProcessRequestService>();

            IHttpClientService httpClientService = new HttpClientService();// TODO - fake this
            IHashService hashService = new MD5HashService(hashLogger);// TODO - fake this

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                WebScraperContext dbContext = sqliteMemoryWrapper.DbContext;

                var processRequestService = new ProcessRequestService(processRequestServiceLogger, dbContext, httpClientService, hashService);

                var page = new Page
                {
                    Url = "http://www.google.com",
                    StartedAt = DateTime.UtcNow,
                    Status = Status.Pending
                };
                dbContext.Pages.Add(page);

                dbContext.SaveChanges();

                // Act
                processRequestService.ProcessRequestAsync().Wait();

                // Assert
                // TODO - ad more tests once we have faked this
                Assert.AreEqual(dbContext.Pages.FirstAsync().Result.ContentHash, dbContext.Content.FirstAsync().Result.Hash);
                Assert.AreEqual(1, dbContext.Content.CountAsync().Result);
            }
        }
    }
}
