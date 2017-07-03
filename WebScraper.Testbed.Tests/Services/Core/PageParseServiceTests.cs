namespace WebScraper.Testbed.Tests.Services.Core
{
    using System.IO;
    using System.Linq;

    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using WebScraper.Testbed.Services.Core;

    [TestClass]
    public class PageParseServiceTests
    {
        [TestMethod]
        public void TestParseGuardian()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<PageParseService> logger = loggerFactory.CreateLogger<PageParseService>();

            using (FileStream guardianContentStream = File.OpenRead("Guardian-2017-06-18.html"))
            {
                var pageParseService = new PageParseService(logger);
                Content.WebPageContent webPageContent = pageParseService.ParseWebPage(guardianContentStream);

                // Check parsed content
                Assert.IsNotNull(webPageContent);
                Assert.IsNotNull(webPageContent.Links);
                Assert.AreEqual(238, webPageContent.Links.Count());
                Assert.AreEqual(@"https://assets.guim.co.uk/", webPageContent.Links.First().Value);
                Assert.AreEqual(@"https://subscribe.theguardian.com/us?INTCMP=NGW_FOOTER_US_GU_SUBSCRIBE", webPageContent.Links.Last().Value);
            }
        }
    }
}
