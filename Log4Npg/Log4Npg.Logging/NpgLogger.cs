using Newtonsoft.Json;
using System;
using Log4Npg.Models;
using Log4Npg.Logging.Data;

namespace Log4Npg.Logging
{
    public class NpgLogger : INpgLogger
    {
        private readonly ILoggingRepository _loggingRepository;
        private readonly LogLevel _logLevel;
        public NpgLogger(ILoggingRepository loggingRepository, LogLevel logLevel)
        {
            _loggingRepository = loggingRepository;
            _logLevel = logLevel;
        }

        public void LogDebug(object message)
        {
            if (CanLogAtLevel(LogLevel.Debug))
            {
                var logEntry = BuildLogEntry(message, LogLevel.Debug);
                _loggingRepository.AddLogEntry(logEntry);
            }
        }

        public void LogInfo(object message)
        {
            if (CanLogAtLevel(LogLevel.Info))
            {
                var logEntry = BuildLogEntry(message, LogLevel.Info);
                _loggingRepository.AddLogEntry(logEntry);
            }
        }

        public void LogWarning(object message)
        {
            if (CanLogAtLevel(LogLevel.Warn))
            {
                var logEntry = BuildLogEntry(message, LogLevel.Warn);
                _loggingRepository.AddLogEntry(logEntry);
            }
        }

        public void LogError(object message)
        {
            if (CanLogAtLevel(LogLevel.Error))
            {
                var logEntry = BuildLogEntry(message, LogLevel.Error);
                _loggingRepository.AddLogEntry(logEntry);
            }
        }

        public void LogFatal(object message)
        {
            if (CanLogAtLevel(LogLevel.Fatal))
            {
                var logEntry = BuildLogEntry(message, LogLevel.Fatal);
                _loggingRepository.AddLogEntry(logEntry);
            }
        }

        private LogEntry BuildLogEntry(object message, LogLevel level)
        {
            var json = JsonConvert.SerializeObject(message);
            return new LogEntry
            {
                Level = level,
                EventTime = DateTime.UtcNow,
                Message = JsonConvert.SerializeObject(message)
            };
        }

        private bool CanLogAtLevel(LogLevel level)
        {
            return level >= _logLevel;
        }
    }
}
