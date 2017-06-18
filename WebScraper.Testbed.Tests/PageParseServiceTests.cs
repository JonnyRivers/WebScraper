

namespace WebScraper.Testbed.Tests
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PageParseServiceTests
    {
        [TestMethod]
        public void TestParseGuardian()
        {
            byte[] guardianContentData = System.IO.File.ReadAllBytes("Guardian-2017-06-18.html");

            var pageParseService = new Services.PageParseService();
            Content.WebPageContent webPageContent = pageParseService.ParseWebPage(guardianContentData);

            // Check parsed content
            Assert.IsNotNull(webPageContent);
            Assert.AreEqual(50, webPageContent.Links.Count());
        }
    }
}
