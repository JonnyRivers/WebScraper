namespace WebScraper.Testbed.Services.Core
{
    public interface IHashService
    {
        // TODO - should this be async?
        string GenerateHash(byte[] data);
    }
}
