namespace WebScraper.Testbed.Services
{
    internal interface IHashService
    {
        // TODO - should this be async?
        string GenerateHash(byte[] data);
    }
}
