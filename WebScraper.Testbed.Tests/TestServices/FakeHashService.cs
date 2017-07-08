namespace WebScraper.Testbed.Tests.TestServices
{
    using WebScraper.Testbed.Services.Core;

    /// <summary>
    /// A fake implementation of IHashService that always returns the same hash
    /// Suitable only for simple situations
    /// </summary>
    public class FakeHashService : IHashService
    {
        private string m_hash;

        public FakeHashService(string hash)
        {
            m_hash = hash;
        }

        public string GenerateHash(byte[] data)
        {
            return m_hash;
        }
    }
}
