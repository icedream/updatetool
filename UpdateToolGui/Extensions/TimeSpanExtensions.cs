using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateToolGui.Extensions
{
    static class TimeSpanExtensions
    {
        private static string GetSeconds(TimeSpan timeSpan)
        {
            return timeSpan.Seconds + " sec";
        }

        private static string GetMinutes(TimeSpan timeSpan)
        {
            if (timeSpan.Minutes == 0) return string.Empty;
            return timeSpan.Minutes + " min";
        }

        private static string GetHours(TimeSpan timeSpan)
        {
            if (timeSpan.Hours == 0) return string.Empty;
            return timeSpan.Hours + " h";
        }

        private static string GetDays(TimeSpan timeSpan)
        {
            if (timeSpan.Days == 0) return string.Empty;
            return timeSpan.Days + " d";
        }

        public static string ToPrettyFormat(this TimeSpan timeSpan)
        {
            var dayParts = new[] { GetDays(timeSpan), GetHours(timeSpan), GetMinutes(timeSpan), GetSeconds(timeSpan) }
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            var numberOfParts = dayParts.Length;

            string result;
            if (numberOfParts < 2)
                result = dayParts.FirstOrDefault() ?? string.Empty;
            else
                result = string.Join(", ", dayParts, 0, numberOfParts - 1) + " and " + dayParts[numberOfParts - 1];

            if (result.Length > 0)
                return char.ToUpper(result[0]) + result.Substring(1);
            else
                return result;
        }
    }
}
