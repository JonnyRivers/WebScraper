using Microsoft.EntityFrameworkCore;

namespace WebScraper.Testbed.Data
{
    public class WebScraperContext : DbContext
    {
        public WebScraperContext(DbContextOptions<WebScraperContext> options) : base(options) { }

        public DbSet<Content> Content { get; set; }
        public DbSet<PageLink> PageLinks { get; set; }
        public DbSet<PageRequest> PageRequests { get; set; }
    }
}
