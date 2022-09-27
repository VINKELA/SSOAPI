using Utility.Logging;

namespace PowerTrackEnterprise.Core.Handlers
{
    public class LogHandler
    {
        public static void LogInfo(string messageInfo)
        {
            Log("INFO", messageInfo);
        }

        public static void LogWarn(string messageInfo)
        {
            Log("WARN", messageInfo);
        }

        public static void LogError(string messageInfo)
        {
            Log("ERROR", messageInfo);
        }

        private static void Log(string messageType, string messageInfo)
        {
            ActivityLogger.Log(messageType, messageInfo);
        }
    }
}
