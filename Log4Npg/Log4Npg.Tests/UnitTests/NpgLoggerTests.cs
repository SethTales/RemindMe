using NUnit.Framework;
using NSubstitute;
using Log4Npg.Logging.Data;
using Log4Npg.Logging;
using Log4Npg.Models;
using System.Reflection;
using System.Collections.Generic;

namespace Log4Npg.Tests.UnitTests
{
    [TestFixture]
    public class NpgLoggerTests
    {
        private ILoggingRepository _loggingResitory;
        private INpgLogger _defaultLogger;
        private string[] _logMethodNames = new[] {"LogDebug", "LogInfo", "LogWarning", "LogError", "LogFatal"};

        [SetUp]
        public void Setup()
        {
            _loggingResitory = Substitute.For<ILoggingRepository>();
            _defaultLogger = new NpgLogger(_loggingResitory, LogLevel.All);
        }

        [TestCase("LogDebug", LogLevel.Debug)]
        [TestCase("LogInfo", LogLevel.Info)]
        [TestCase("LogWarning", LogLevel.Warn)]
        [TestCase("LogError", LogLevel.Error)]
        [TestCase("LogFatal", LogLevel.Fatal)]
        public void EachLoggingMethod_CallsLoggingRepository_WithCorrectLogLevelEnum(string methodName, LogLevel level)
        {
            var type = _defaultLogger.GetType();
            var logMethod = type.GetMethod(methodName);
            var logMessage = "this is a test message";

            logMethod.Invoke(_defaultLogger, new object[] {logMessage});

            _loggingResitory.Received(1).AddLogEntry(Arg.Is<LogEntry>(x =>
                x.Level == level));
        }

        [TestCase(LogLevel.All, 5)]
        [TestCase(LogLevel.Debug, 5)]
        [TestCase(LogLevel.Info, 4)]
        [TestCase(LogLevel.Warn, 3)]
        [TestCase(LogLevel.Error, 2)]
        [TestCase(LogLevel.Fatal, 1)]
        public void NpgLogger_LogsAtCorrectLevel_BasedOnLogLevel_SetInConstructor(LogLevel baseLogLevel, int expectedAddLogEntryInvocations)
        {
            var restrictedLogger = new NpgLogger(_loggingResitory, baseLogLevel);
            var type = _defaultLogger.GetType();
            var logMethods = new List<MethodInfo>();
            foreach (var methodName in _logMethodNames)
            {
                logMethods.Add(type.GetMethod(methodName));
            }
            var logMessage = "this is a test message";

            foreach (var method in logMethods)
            {
                method.Invoke(restrictedLogger, new object[] {logMessage});
            }

            _loggingResitory.Received(expectedAddLogEntryInvocations).AddLogEntry(Arg.Any<LogEntry>());
        }

    }
}