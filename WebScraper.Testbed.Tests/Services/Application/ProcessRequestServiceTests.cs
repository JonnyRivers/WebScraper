namespace WebScraper.Testbed.Tests.Services.Application
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using WebScraper.Testbed.Data;
    using WebScraper.Testbed.Services.Application;
    using WebScraper.Testbed.Services.Core;

    using Infrastructure;
    using TestServices;

    [TestClass]
    public class ProcessRequestServiceTests
    {
        [TestMethod]
        public void TestProcessRequest()
        {
            // Arrange
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<ProcessRequestService> processRequestServiceLogger = loggerFactory.CreateLogger<ProcessRequestService>();

            string url = "http://www.google.com/";
            string responseText = "mock response";
            var response = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(responseText)
            };
            var mockedResponsesByRequestUri = new Dictionary<string, HttpResponseMessage>()
            {
                { url, response }
            };
            HttpClientHandler httpClientHandler = new MockHttpClientHandler(mockedResponsesByRequestUri);
            IHttpClientService httpClientService = new MockHttpClientService(httpClientHandler);

            string dummyHash = "0123456789abcdef";
            IHashService hashService = new FakeHashService(dummyHash);

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                WebScraperContext dbContext = sqliteMemoryWrapper.DbContext;

                var processRequestService = new ProcessRequestService(processRequestServiceLogger, dbContext, httpClientService, hashService);

                var page = new Page
                {
                    Url = url,
                    StartedAt = DateTime.UtcNow,
                    Status = Status.Pending
                };
                dbContext.Pages.Add(page);

                dbContext.SaveChanges();

                // Act
                processRequestService.ProcessRequestAsync().Wait();

                // Assert
                List<Page> pages = dbContext.Pages.ToListAsync().Result;

                Assert.AreEqual(1, pages.Count);
                Assert.AreEqual(Status.Downloaded, pages[0].Status);
                Assert.AreEqual(dummyHash, pages[0].ContentHash);

                List<Content> contentRecords = dbContext.Content.ToListAsync().Result;

                Assert.AreEqual(1, contentRecords.Count);
                Assert.AreEqual(dummyHash, contentRecords[0].Hash);
                Assert.AreEqual(responseText, System.Text.Encoding.UTF8.GetString(contentRecords[0].Data));
            }
        }
    }
}
