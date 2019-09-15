using RemindMe.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using RemindMe.Models;
using System.Threading.Tasks;

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
                var command = $"TRUNCATE TABLE \"{table}\"";
                Database.ExecuteSqlCommand(command);
            }
        }

        public async Task<List<RemindMeUser>> GetTestUsers()
        {
            return await RemindMeUsers.ToListAsync();
        }
    }
}