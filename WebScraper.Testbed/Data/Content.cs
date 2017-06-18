using System.ComponentModel.DataAnnotations;

namespace WebScraper.Testbed.Data
{
    public class Content
    {
        [Key]
        public string Hash { get; set; }
        public byte[] Data { get; set; }
    }
}
