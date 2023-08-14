using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimelinerNet
{
    internal static class SpanMode
    {
        public static Mode NearSpanMode(this TimeSpan span)
        {
            if (span <= TimeSpan.FromSeconds(1)) return Mode.Seconds;
            else if (span <= TimeSpan.FromSeconds(1)) return Mode.Seconds;
            else if (span <= TimeSpan.FromMinutes(1)) return Mode.Minute;
            else if (span <= TimeSpan.FromHours(1)) return Mode.Hour;
            else if (span <= TimeSpan.FromDays(1)) return Mode.Day;
            else if (span <= TimeSpan.FromDays(30)) return Mode.Month;
            else return Mode.Year;
        }
        public static TimeSpan ModeToSpan(this Mode mode, DateTime when)
        {
            switch (mode)
            {
                case Mode.Seconds:
                    return TimeSpan.FromSeconds(1);
                case Mode.Minute:
                    return TimeSpan.FromMinutes(1);
                case Mode.Hour:
                    return TimeSpan.FromHours(1);
                case Mode.Day:
                    return TimeSpan.FromDays(1);
                case Mode.Month:
                    return TimeSpan.FromDays(DateTime.DaysInMonth(when.Year, when.Month));
                case Mode.Year:
                    return TimeSpan.FromDays(DateTime.IsLeapYear(when.Year) ? 366 : 365);
                default:
                    return default;
            }
        }

        public static DateTime GetMajorLeftEdge(this DateTime dt, Mode mode)
        {
            switch (mode)
            {
                case Mode.Seconds:
                    return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
                case Mode.Minute:
                    return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
                case Mode.Hour:
                    return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                case Mode.Day:
                    return new DateTime(dt.Year, dt.Month, dt.Day);
                case Mode.Month:
                    return new DateTime(dt.Year, dt.Month, 1);
                case Mode.Year:
                    return new DateTime(dt.Year, 1, 1);
                default:
                    return default;
            }
        }
        public static string ToFullString(this DateTime dt, Mode mode)
        {
            switch (mode)
            {
                case Mode.Seconds:
                    return dt.ToString("MM.dd HH:mm:ss");
                case Mode.Minute:
                    return dt.ToString("MM.dd HH:mm:00");
                case Mode.Hour:
                    return dt.ToString("yyyy.MM.dd HH:00");
                case Mode.Day:
                    return dt.ToString("yyyy.MM.dd");
                case Mode.Month:
                    return dt.ToString("yyyy.MMMM");
                case Mode.Year:
                    return dt.ToString("yyyy");
                default:
                    return default;
            }
        }

        public static string ToLastString(this DateTime dt, Mode mode)
        {
            switch (mode)
            {
                case Mode.Seconds:
                    return dt.ToString("ss");
                case Mode.Minute:
                    return dt.ToString("mm");
                case Mode.Hour:
                    return dt.ToString("HH");
                case Mode.Day:
                    return dt.ToString("dd");
                case Mode.Month:
                    return dt.ToString("MM");
                case Mode.Year:
                    return dt.ToString("yy");
                default:
                    return default;
            }
        }
        public enum Mode
        {
            Minimum,
            Seconds,
            Minute,
            Hour,
            Day,
            Month,
            Year,
            Maximum
        }
    }
}
