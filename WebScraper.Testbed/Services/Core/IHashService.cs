namespace WebScraper.Testbed.Services.Core
{
    internal interface IHashService
    {
        // TODO - should this be async?
        string GenerateHash(byte[] data);
    }
}
