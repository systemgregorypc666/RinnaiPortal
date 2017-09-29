using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Extensions
{
    public static class DatetimeFormat
    {
        public static string FormatDatetime(this DateTime datetime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return datetime.ToString(format);
        }

        public static string FormatDatetimeNullable(this DateTime? datetime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            return datetime.HasValue ? datetime.Value.ToString(format) : (string)null;
        }
    }
}