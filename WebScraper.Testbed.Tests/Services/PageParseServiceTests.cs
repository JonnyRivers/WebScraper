namespace WebScraper.Testbed.Tests.Services
{
    using System.IO;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PageParseServiceTests
    {
        [TestMethod]
        public void TestParseGuardian()
        {
            using (FileStream guardianContentStream = File.OpenRead("Guardian-2017-06-18.html"))
            {
                var pageParseService = new Services.PageParseService();
                Content.WebPageContent webPageContent = pageParseService.ParseWebPage(guardianContentStream);

                // Check parsed content
                Assert.IsNotNull(webPageContent);
                Assert.AreEqual(238, webPageContent.Links.Count());
                Assert.AreEqual(@"https://assets.guim.co.uk/", webPageContent.Links.First().Value);
                Assert.AreEqual(@"https://subscribe.theguardian.com/us?INTCMP=NGW_FOOTER_US_GU_SUBSCRIBE", webPageContent.Links.Last().Value);
            }
        }
    }
}
