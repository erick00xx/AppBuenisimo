using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppBuenisimo.Utils
{
    public static class TimeProvider
    {
        private static readonly TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

        public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

        public static DateTime Today => Now.Date;
    }
}