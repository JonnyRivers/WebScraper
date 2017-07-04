namespace WebScraper.Testbed.Services.Core
{
    using System.Security.Cryptography;
    using System.Text;

    using Microsoft.Extensions.Logging;

    public class MD5HashService : IHashService
    {
        private readonly ILogger m_logger;

        public MD5HashService(ILogger<MD5HashService> logger)
        {
            m_logger = logger;
        }

        public string GenerateHash(byte[] data)
        {
            m_logger.LogInformation($"Generating hash for {data.Length} bytes of data.");

            using (MD5 md5Hash = MD5.Create())
            {
                byte[] hashBytes = md5Hash.ComputeHash(data);

                var hashStringBuilder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashStringBuilder.Append(hashBytes[i].ToString("x2"));
                }

                string hashString = hashStringBuilder.ToString();
                m_logger.LogInformation($"Generated hash {hashString} for {data.Length} bytes of data.");
                return hashString;
            }
        }
    }
}
