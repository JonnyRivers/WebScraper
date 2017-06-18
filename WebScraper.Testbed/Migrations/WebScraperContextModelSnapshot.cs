using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using WebScraper.Testbed;

namespace WebScraper.Testbed.Migrations
{
    [DbContext(typeof(WebScraperContext))]
    partial class WebScraperContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WebScraper.Testbed.Content", b =>
                {
                    b.Property<string>("Hash")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Data");

                    b.HasKey("Hash");

                    b.ToTable("Content");
                });

            modelBuilder.Entity("WebScraper.Testbed.PageRequest", b =>
                {
                    b.Property<int>("PageRequestId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CompletedAt");

                    b.Property<string>("ContentHash");

                    b.Property<DateTime>("RequestedAt");

                    b.Property<DateTime?>("StartedAt");

                    b.Property<int>("Status");

                    b.Property<string>("Url");

                    b.HasKey("PageRequestId");

                    b.ToTable("PageRequests");
                });
        }
    }
}
