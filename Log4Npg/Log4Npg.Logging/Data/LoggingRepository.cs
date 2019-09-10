using Log4Npg.Models;

namespace Log4Npg.Logging.Data
{
    public class LoggingRepository : ILoggingRepository
    {
        private readonly LoggingDatabaseContext _dbContext;

        public LoggingRepository(LoggingDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddLogEntry(LogEntry logEntry)
        {
            _dbContext.LogEntries.Add(logEntry);
            _dbContext.SaveChanges();
        }
    }
}