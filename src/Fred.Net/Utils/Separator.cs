using System;
using System.Collections.Generic;

namespace Fred.Net.Utils
{
    public static class Separator
    {
        public static string GetStringSeparatedBySemicolon(List<string> tags)
        {
            string result = string.Empty;

            tags.ForEach(tag => result += tag + ";");

            result = result.Remove(result.Length - 1);

            return result;
        }

        public static string GetDatesSeparatedByComma(List<DateTime> dates)
        {
            string result = string.Empty;

            dates.ForEach(date => result += DateTimeFormat.FormatDate(date) + ",");

            result = result.Remove(result.Length - 1);

            return result;
        }
    }
}