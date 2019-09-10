using Microsoft.EntityFrameworkCore;
using System.Linq;
using Log4Npg.Models;

namespace Log4Npg.Logging.Data
{
    public class TestDatabaseContext : LoggingDatabaseContext
    {
        public TestDatabaseContext(DbContextOptions<LoggingDatabaseContext> options) : base (options)
        {

        }

        public void DeleteTestData(string[] tableNames)
        {
            foreach (var table in tableNames)
            {
                var command = $"TRUNCATE TABLE {table}";
                Database.ExecuteSqlCommand(command);
            }
        }

        public LogEntry FindLogEntryById(int logEntryId)
        {
            return LogEntries.FirstOrDefault(x => x.LogEntryId == logEntryId);
        }
    }
}