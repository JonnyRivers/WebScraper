namespace WebScraper.Testbed.Tests.Infrastructure
{
    using System;

    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    using WebScraper.Testbed.Data;

    internal class SqliteMemoryWrapper : IDisposable
    {
        private SqliteConnection m_connection;

        internal WebScraperContext DbContext { get; }

        internal SqliteMemoryWrapper()
        {
            m_connection = new SqliteConnection("DataSource=:memory:");
            m_connection.Open();
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<WebScraperContext>()
                .UseSqlite(m_connection);
            DbContext = new WebScraperContext(dbContextOptionsBuilder.Options);
            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            m_connection.Close();
            DbContext.Dispose();
        }
    }
}
