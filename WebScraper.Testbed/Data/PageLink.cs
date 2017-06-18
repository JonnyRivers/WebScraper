using System.ComponentModel.DataAnnotations;

namespace WebScraper.Testbed.Data
{
    public class PageLink
    {
        [Key]
        public int PageLinkId { get; set; }
        public int SourcePageRequestId { get; set; }
        public int TargetPageRequestId { get; set; }
    }
}
