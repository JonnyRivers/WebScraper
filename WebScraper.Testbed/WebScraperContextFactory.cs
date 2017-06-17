using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace WebScraper.Testbed
{
    class WebScraperContextFactory : IDbContextFactory<WebScraperContext>
    {
        // This exists so that commands like 'dotnet ef migrations Initial -c WebScraperContext' work

        // To create the db:
        // 'dotnet ef migrations add Initial -c WebScraper.Testbed.WebScraperContext'
        // 'dotnet ef database update Initial'
        // 'dotnet ef migrations remove'
        public WebScraperContext Create(DbContextFactoryOptions options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WebScraperContext>();
            string connectionString = @"Server=(localdb)\mssqllocaldb;Database=WebScraper;Trusted_Connection=True;";
            optionsBuilder.UseSqlServer(connectionString);

            return new WebScraperContext(optionsBuilder.Options);
        }
    }
}
