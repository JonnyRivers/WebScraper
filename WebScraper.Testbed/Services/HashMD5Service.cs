using System;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Logging;

namespace WebScraper.Testbed
{
    internal class HashMD5Service : IHashService
    {
        private readonly ILogger<RequestAction> m_logger;

        public HashMD5Service(ILogger<RequestAction> logger)
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
                    hashStringBuilder.Append(data[i].ToString("x2"));
                }

                string hashString = hashStringBuilder.ToString();
                m_logger.LogInformation($"Generated hash {hashString} for {data.Length} bytes of data.");
                return hashString;
            }
        }
    }
}
