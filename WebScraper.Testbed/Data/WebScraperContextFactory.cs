using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace WebScraper.Testbed.Data
{
    class WebScraperContextFactory : IDbContextFactory<WebScraperContext>
    {
        // This exists so that commands like 'dotnet ef migrations Initial' work

        public const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=WebScraper;Trusted_Connection=True;";

        // To create the db:
        // 'dotnet ef migrations add Initial'
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
