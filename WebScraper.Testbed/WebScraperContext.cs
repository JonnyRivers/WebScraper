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
        public DbSet<PageResult> PageResults { get; set; }
        public DbSet<Content> Content { get; set; }
    }

    public enum Status
    {
        Pending,
        InProgress,
        Done
    }

    public class PageRequest
    {
        public int PageRequestId { get; set; }
        public string Url { get; set; }
        public Status Status { get; set; }
        DateTime RequestedAt { get; set; }
        DateTime? StartedAt { get; set; }
        DateTime? CompletedAt { get; set; }
    }

    public class PageResult
    {
        public int PageResultId { get; set; }
        public string Url { get; set; }
        DateTime RequestedAt { get; set; }
        DateTime StartedAt { get; set; }
        DateTime CompletedAt { get; set; }
        string ContentHash { get; set; }
    }

    public class Content
    {
        [Key]
        public string Hash { get; set; }
        public byte[] Data { get; set; }
    }
}
