using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace WebScraper.Testbed
{
    class WebScraperContextFactory : IDbContextFactory<WebScraperContext>
    {
        // This exists so that commands like 'dotnet ef migrations Initial -c WebScraperContext' work

        public const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=WebScraper;Trusted_Connection=True;";

        // To create the db:
        // 'dotnet ef migrations add Initial -c WebScraper.Testbed.WebScraperContext'
        // 'dotnet ef database update Initial'
        // 'dotnet'
        public WebScraperContext Create(DbContextFactoryOptions options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebScraperContext>();
            optionsBuilder.UseSqlServer(ConnectionString);

            return new WebScraperContext(optionsBuilder.Options);
        }
    }
}
