using System;

namespace EWS
{
    public static class DateTimeExtension
    {
        public static DateTime RoundToSeconds(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0, dt.Kind);
        }
    }
}
