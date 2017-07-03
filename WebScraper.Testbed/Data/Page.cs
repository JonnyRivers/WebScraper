using System;
using System.ComponentModel.DataAnnotations;

namespace WebScraper.Testbed.Data
{
    public class Page
    {
        [Key]
        public int PageId { get; set; }
        public string Url { get; set; }
        public Status Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? DownloadedAt { get; set; }
        public DateTime? ParsedAt { get; set; }
        public string ContentHash { get; set; }
    }
}
