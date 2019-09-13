using RemindMe.Data;
using Microsoft.EntityFrameworkCore;

namespace RemindMe.Tests.IntegrationTests
{
    public class TestDatabaseContext : RemindMeDatabaseContext
    {
        private readonly DbContextOptions<RemindMeDatabaseContext> _contextOptions;

        public TestDatabaseContext(DbContextOptions<RemindMeDatabaseContext> contextOptions) : base(contextOptions)
        {
            _contextOptions = contextOptions;
        }

        public void DeleteTestData(string[] tableNames)
        {
            foreach (var table in tableNames)
            {
                var command = $"TRUNCATE TABLE {table}";
                Database.ExecuteSqlCommand(command);
            }
        }
    }
}