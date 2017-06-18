using System.ComponentModel.DataAnnotations;

namespace WebScraper.Testbed.Data
{
    public class PageLink
    {
        [Key]
        public int PageLinkId { get; set; }
        public int SourcePageId { get; set; }
        public int TargetPageId { get; set; }
    }
}
