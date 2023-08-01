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
            else if (span <= TimeSpan.FromSeconds(15)) return Mode.Seconds;
            else if (span <= TimeSpan.FromMinutes(1)) return Mode.SecondsQuater;
            else if (span <= TimeSpan.FromMinutes(15)) return Mode.Minute;
            else if (span <= TimeSpan.FromHours(1)) return Mode.MinuteQuater;
            else if (span <= TimeSpan.FromHours(6)) return Mode.Hour;
            else if (span <= TimeSpan.FromDays(1)) return Mode.HourQuater;
            else if (span <= TimeSpan.FromDays(7)) return Mode.Day;
            else if (span <= TimeSpan.FromDays(30)) return Mode.Week;
            else if (span <= TimeSpan.FromDays(90)) return Mode.Month;
            else if (span <= TimeSpan.FromDays(360)) return Mode.Season;
            else return Mode.Year;
        }
        public static TimeSpan ModeToSpan(this Mode mode)
        {
            switch (mode)
            {
                case Mode.Seconds:
                    return TimeSpan.FromSeconds(1);
                case Mode.SecondsQuater:
                    return TimeSpan.FromSeconds(15);
                case Mode.Minute:
                    return TimeSpan.FromMinutes(1);
                case Mode.MinuteQuater:
                    return TimeSpan.FromMinutes(15);
                case Mode.Hour:
                    return TimeSpan.FromHours(1);
                case Mode.HourQuater:
                    return TimeSpan.FromHours(6);
                case Mode.Day:
                    return TimeSpan.FromDays(1);
                case Mode.Week:
                    return TimeSpan.FromDays(7);
                case Mode.Month:
                    return TimeSpan.FromDays(30);
                case Mode.Season:
                    return TimeSpan.FromDays(90);
                case Mode.Year:
                    return TimeSpan.FromDays(360);
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
                case Mode.SecondsQuater:
                    return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, (int)Math.Floor(dt.Second / 15.0));
                case Mode.Minute:
                    return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
                case Mode.MinuteQuater:
                    return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, (int)Math.Floor(dt.Minute / 15.0), 0);
                case Mode.Hour:
                    return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
                case Mode.HourQuater:
                    return new DateTime(dt.Year, dt.Month, dt.Day, (int)Math.Floor(dt.Hour / 6.0), 0, 0);
                case Mode.Day:
                    return new DateTime(dt.Year, dt.Month, dt.Day);
                case Mode.Week:
                    return new DateTime(dt.Year, dt.Month, (int)Math.Floor(dt.Day / 7.0) + 1);
                case Mode.Month:
                    return new DateTime(dt.Year, dt.Month, 1);
                case Mode.Season:
                    return new DateTime(dt.Year, (int)Math.Floor(dt.Month / 3.0) + 1, 1);
                case Mode.Year:
                    return new DateTime(dt.Year, 1, 1);
                default:
                    return default;
            }
        }
        public enum Mode
        {
            Minimum,
            Seconds,
            SecondsQuater,
            Minute,
            MinuteQuater,
            Hour,
            HourQuater,
            Day,
            Week,
            Month,
            Season,
            Year,
            Maximum
        }
    }
}
