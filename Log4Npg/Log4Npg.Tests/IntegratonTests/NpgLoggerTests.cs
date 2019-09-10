using NUnit.Framework;
using Log4Npg.Logging.Data;
using Microsoft.EntityFrameworkCore;
using Log4Npg.Models;
using Newtonsoft.Json;
using Log4Npg.Logging;

namespace Log4Npg.Tests.IntegrationTests
{
    public class NpgLoggerTests
    {
        private ILoggingRepository _loggingRepository;
        private INpgLogger _npgLogger;
        private TestDatabaseContext _testDbContext;
        private const string TestConnectionString = "User ID=root;Password=password;server=localhost;Port=5432;Database=Log4NpgTestDatabase";


        [OneTimeSetUp]
        public void InitTestFixture()
        {           
            var optionsBuilder = new DbContextOptionsBuilder<LoggingDatabaseContext>();
            optionsBuilder.UseNpgsql(TestConnectionString);
            _testDbContext = new TestDatabaseContext(optionsBuilder.Options);
            _loggingRepository = new LoggingRepository(_testDbContext);
            _npgLogger = new NpgLogger(_loggingRepository, LogLevel.All);           
        }

        [TearDown]
        public void Cleanup()
        {
            _testDbContext.DeleteTestData(new [] { "\"LogEntries\"" });
        }

        [TestCase("LogDebug", LogLevel.Debug, 1)]
        [TestCase("LogInfo", LogLevel.Info, 2)]
        [TestCase("LogWarning", LogLevel.Warn, 3)]
        [TestCase("LogError", LogLevel.Error, 4)]
        [TestCase("LogFatal", LogLevel.Fatal, 5)]
        public void AddLogEntryWorks_AtEachLogLevel(string methodName, LogLevel level, int logId)
        {
            var type = _npgLogger.GetType();
            var logMethod = type.GetMethod(methodName);
            var logMessage = new TestLogMessage
            {
                Id = 1,
                Name = "TestLogMessage",
                Source = "Log4NpgIntegrationTests"
            };

            logMethod.Invoke(_npgLogger, new object[] {logMessage});

            var addedLogEntry = _testDbContext.FindLogEntryById(logId);
            var addedLogMessage = JsonConvert.DeserializeObject<TestLogMessage>(addedLogEntry.Message);

            Assert.AreEqual(level, addedLogEntry.Level);
            Assert.AreEqual(logMessage.Id, addedLogMessage.Id);
            Assert.AreEqual(logMessage.Name, addedLogMessage.Name);
            Assert.AreEqual(logMessage.Source, addedLogMessage.Source);
        }

        public class TestLogMessage
        {
            public int Id {get; set;}
            public string Name {get; set;}
            public string Source {get; set;}
        }
        
    }
}