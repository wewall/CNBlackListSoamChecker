using System;

namespace CNBlackListSoamChecker
{
    public class GetTime
    {
        public static long GetUnixTime()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static string DecodeUnixTime(long time, int offset)
        {
            DateTime dtime = new DateTime(1970, 1, 1).AddSeconds(time + offset * 3600);
            return dtime.ToString("yyyy-MM-dd HH:mm:ss (UTC+" + offset + ")");
        }

        public static string GetExpiresTime(long time)
        {
            if (time == 0)
            {
                return "9999-09-09 09:09:09 (UTC+9)";
            }
            DateTime dtime = new DateTime(1970, 1, 1).AddSeconds(time + 28800);
            return dtime.ToString("yyyy-MM-dd HH:mm:ss (UTC+8)");
        }

        public static bool GetIsExpired(long time)
        {
            if (time == 0)
            {
                return false;
            }
            if (GetUnixTime() >= time - 30)
            {
                return true;
            }
            return false;
        }

        /*public static int GetTimeOffset()
        {
        *
        }*/
    }
}
