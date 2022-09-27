using System;

namespace PowerTrackEnterprise.Core.Handlers
{
    public class ErrorHandler
    {
        public static void Log(Exception exception)
        {
            LogHandler.LogError($"[{exception}] {exception.Message}");

            if (exception.InnerException != null)
                LogHandler.LogError($"[{exception.InnerException}] {exception.InnerException.Message}");
        }
    }
}
