namespace WebScraper.Testbed.Tests.Services.Core
{
    using System.Text;

    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using WebScraper.Testbed.Services.Core;

    [TestClass]
    public class MD5HashServiceTests
    {
        [TestMethod]
        public void TestGenerateHash()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<MD5HashService> logger = loggerFactory.CreateLogger<MD5HashService>();

            var service = new MD5HashService(logger);

            string testString = "Jonny";
            byte[] testData = Encoding.ASCII.GetBytes(testString);

            string hash1 = service.GenerateHash(testData);
            string hash2 = service.GenerateHash(testData);

            string expectedHash = "820b4ad02742e6630b554a48de7d2d9f";// from http://www.md5.cz/

            Assert.AreEqual(expectedHash, hash1);
            Assert.AreEqual(expectedHash, hash2);
        }
    }
}
