using CNBlackListSoamChecker.DbManager;
using System.Collections.Generic;

namespace CNBlackListSoamChecker
{
    internal static class Temp
    {
        private static DatabaseManager databaseManager = null;
        internal static List<SpamMessage> spamMessageList = null;
        internal static Dictionary<long, GroupCfg> groupConfig = new Dictionary<long, GroupCfg>() { };
        internal static Dictionary<int, BanUser> bannedUsers = new Dictionary<int, BanUser>() { };
        public static long MainChannelID = -1001040155464; // -1001040155464
        public static long ReasonChannelID = -1001014865159; // -1001014865159
        public static string MainChannelName = "CNBlackList"; // CNBlackList
        public static string ReasonChannelName = "BanReason"; // BanReason

        internal static DatabaseManager GetDatabaseManager()
        {
            if (databaseManager == null)
            {
                databaseManager = new DatabaseManager();
            }
            return databaseManager;
        }
    }
}
