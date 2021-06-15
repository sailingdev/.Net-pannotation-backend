using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Pannotation.Common.Extensions
{
    public static class DateTimeExtentions
    {
        public static string ToISO(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        public static string ToISO(this DateTime? date)
        {
            return date.HasValue ? ToISO(date.Value) : null;
        }

        public static string FormatDate(this DateTime dateTime, double timeZoneOffset)
        {
            DateTime localDate = dateTime.AddHours(timeZoneOffset);
            DateTime localNow = DateTime.UtcNow.AddHours(timeZoneOffset);
            CultureInfo culture = new CultureInfo("en-US");

            string result = String.Empty;
            //if today -> 3:30​ ​ PM​ ​ or​ ​ 11:19​ ​ AM
            if (localDate.Date == localNow.Date)
                result = localDate.ToString("h:mm tt", culture);
            else if (localDate.Date == localNow.Date.AddDays(-1)) //“Yesterday”
                result = "Yesterday";
            else if (localDate.Date >= localNow.Date.AddDays(-7)) //the​ ​ day​ ​ of​ ​ the​ ​ week​ 
                result = localDate.ToString("dddd", culture);
            else // more than week ago
                result = localDate.ToString("MMM d", culture);

            return result;
        }
    }
}
