using System;
using System.Collections.Generic;
using System.Text;

namespace WebScraper.Testbed
{
    internal interface IHashService
    {
        string GenerateHash(byte[] data);
    }
}
