using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebScraper.Testbed
{
    public class WebScraperContext : DbContext
    {
        public WebScraperContext(DbContextOptions<WebScraperContext> options) : base(options) { }

        public DbSet<PageRequest> PageRequests { get; set; }
        public DbSet<Content> Content { get; set; }
    }

    public enum Status
    {
        Pending,
        InProgress,
        Done,
        Failed
    }

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

    public class Content
    {
        [Key]
        public string Hash { get; set; }
        public byte[] Data { get; set; }
    }
}
