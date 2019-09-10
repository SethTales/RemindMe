
namespace Log4Npg
{
    public interface INpgLogger
    {
        void LogDebug(object message);
        void LogInfo(object message);
        void LogWarning(object message);
        void LogError(object message);
        void LogFatal(object message);
    }
}