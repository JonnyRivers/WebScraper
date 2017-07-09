namespace WebScraper.Testbed.Tests.Services.Core
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using WebScraper.Testbed.Services.Core;

    [TestClass]
    public class PageParseServiceTests
    {
        [TestMethod]
        public void TestParsePage()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<PageParseService> logger = loggerFactory.CreateLogger<PageParseService>();

            string link1 = "https://assets.guim.co.uk/";
            string link2 = "https://i.guim.co.uk/";
            string testPageContent =
                "< !DOCTYPE html >" +
                "< html id = \"js-context\" class=\"js-off is-not-modern id--signed-out\" data-page-path=\"/us\">" + 
                "<head>" + 
                "<meta charset = \"utf-8\" />" + 
                "<link />" + // invalid link
               $"<link rel = \"\" href = \"{link1}\" />" +
               $"<link href=\"{link2}\"/>";
            byte[] testPageContentBytes = Encoding.UTF8.GetBytes(testPageContent);

            using (MemoryStream testPageStream = new MemoryStream(testPageContentBytes))
            {
                var pageParseService = new PageParseService(logger);
                Content.WebPageContent webPageContent = pageParseService.ParseWebPage(testPageStream);

                // Check parsed content
                Assert.IsNotNull(webPageContent);
                Assert.IsNotNull(webPageContent.Links);

                var links = new List<Content.WebPageLink>(webPageContent.Links);
                Assert.AreEqual(2, links.Count);
                Assert.AreEqual(link1, links[0].Value);
                Assert.AreEqual(link2, links[1].Value);
            }
        }

        [TestMethod]
        public void TestParseImage()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<PageParseService> logger = loggerFactory.CreateLogger<PageParseService>();

            using (FileStream guardianContentStream = File.OpenRead("TestImage.png"))
            {
                var pageParseService = new PageParseService(logger);
                Content.WebPageContent webPageContent = pageParseService.ParseWebPage(guardianContentStream);

                // Check parsed content
                Assert.IsNotNull(webPageContent);
                Assert.IsNotNull(webPageContent.Links);
                Assert.AreEqual(0, webPageContent.Links.Count());
            }
        }
    }
}
