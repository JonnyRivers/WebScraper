using System;
using System.ComponentModel.DataAnnotations;

namespace WebScraper.Testbed.Data
{
    public class PageRequest
    {
        [Key]
        public int PageRequestId { get; set; }
        public string Url { get; set; }
        public Status Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string ContentHash { get; set; }
    }
}
