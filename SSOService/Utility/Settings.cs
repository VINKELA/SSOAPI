using System;
using System.Configuration;

namespace PowerTrackEnterprise.Core.Utility
{
    public class Settings
    {
        public static int LogRollOver => Convert.ToInt32(ConfigurationManager.AppSettings["LogRollOver"] ?? "100000");

        public static bool UseStakify => Convert.ToBoolean(ConfigurationManager.AppSettings["UseStakify"] ?? "false");

        public static bool UseFileSystem => Convert.ToBoolean(ConfigurationManager.AppSettings["UseFileSystem"] ?? "false");

        public static string DateFormat => ConfigurationManager.AppSettings["DateFormat"] ?? "yyyy-MM-dd";

        public static int TempDataPartnerId => Convert.ToInt32(ConfigurationManager.AppSettings["TempDataPartnerId"] ?? "0");
    }
}
