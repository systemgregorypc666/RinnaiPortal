using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Extensions
{
    public static class ObjectConvert
    {
        /// <summary>
        /// defined datetime format list
        /// </summary>
        private static string[] _dateTimeFormatList
        {
            get
            {
                return new string[]
				{
					"yyyy/M/d tt hh:mm:ss",
					"yyyy/MM/dd tt hh:mm:ss",
					"yyyy/MM/dd HH:mm:ss",
					"yyyy/M/d HH:mm:ss",
					"yyyy/M/d",
					"yyyy/MM/dd",
					"yyyyMMddHHmmss",
					"yyyy-M-d tt hh:mm:ss",
					"yyyy-MM-dd tt hh:mm:ss",
					"yyyy-MM-dd HH:mm:ss",
					"yyyy-M-d HH:mm:ss",
					"yyyy-M-d",
					"yyyy-MM-dd",
				};
            }
        }

        public static string ToDateTimeFormateString(this object obj, string format = "yyyy-MM-dd HH:mm:ss")
        {
            DateTime validDate;
            if (obj == null ||
                !DateTime.TryParseExact(obj.ToString(), _dateTimeFormatList, new CultureInfo("zh-TW"), DateTimeStyles.AllowWhiteSpaces, out validDate))
            { return (string)null; }

            return validDate.ToString(format);
        }

        public static string ToDateTimeFormateStringWithOutSec(this object obj, string format = "yyyy-MM-dd HH:mm")
        {
            DateTime validDate;
            if (obj == null ||
                !DateTime.TryParseExact(obj.ToString(), _dateTimeFormatList, new CultureInfo("zh-TW"), DateTimeStyles.AllowWhiteSpaces, out validDate))
            { return (string)null; }
            var test = validDate.ToString(format);
            return validDate.ToString(format);
        }
    }
}