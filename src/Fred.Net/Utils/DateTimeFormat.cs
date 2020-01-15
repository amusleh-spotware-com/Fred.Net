using System;

namespace Fred.Net.Utils
{
    public static class DateTimeFormat
    {
        public static string FormatDate(DateTime date) => date.ToString("yyyy-MM-dd");

        public static string FormatTime(DateTime time) => time.ToString("yyyyMMddHHmm");
    }
}