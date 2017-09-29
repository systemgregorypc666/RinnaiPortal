using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Extensions
{
    public static class StringConvert
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


        /// <summary>
        /// Try Parse DateTime
        /// </summary>
        /// <param name="value">this(value of String)</param>
        /// <param name="format">Defined format </param>
        /// <param name="validDate">After parse has been success.validDate(type of date time )</param>
        /// <returns>boolean</returns>
        public static bool TryParseDateTimeExact(this string value, ref DateTime validDate, string format = "yyyy-MM-dd HH:mm:ss")
        {
            var result = DateTime.TryParseExact(value, _dateTimeFormatList, new CultureInfo("zh-TW"), DateTimeStyles.AllowWhiteSpaces, out validDate);
            return result;
        }

        public static DateTime ToDateTime(this string dateTime)
        {
            DateTime validDate = new DateTime();
            if (TryParseDateTimeExact(dateTime, ref validDate, "yyyy-MM-dd HH:mm:ss"))
            {
                return validDate;
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime? ToDateTimeNullable(this string dateTime)
        {
            DateTime validDate = new DateTime();
            if (dateTime.TryParseDateTimeExact(ref validDate, "yyyy-MM-dd"))
            {
                return validDate;
            }
            else
            {
                return (DateTime?)null;
            }
        }

        public static string ToDateFormateString(this string dateTime, string format = "yyyy-MM-dd HH:mm:ss")
        {
            DateTime validDate = new DateTime();
            if (dateTime.TryParseDateTimeExact(ref validDate, "yyyy-MM-dd"))
            {
                return validDate.ToString(format);
            }
            else
            {
                return (string)null;
            }
        }

        public static int ToInt32(this string parseString)
        {
            int result;
            if (Int32.TryParse(parseString, out result))
            {
                return result;
            }
            return -1;
        }

        public static bool ToBoolean(this string parseString)
        {
            Boolean result;
            var foo = Boolean.TryParse(parseString, out result);
            return result;
        }

        public static string ToJsonRaw(this string str)
        {
            var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return jsonSerializer.Serialize(str);
        }

        public static string ToAlertFormat(this string message)
        {
            var msg = String.Format("<script type='text/javascript'> alert({0}); </script>", ToJsonRaw(message));
            return msg;
        }

        public static string ToAlertAndRedirect(this string message, string url)
        {
            return String.Format("<script type='text/javascript'> alert({0}); window.location.href='{1}';</script>", ToJsonRaw(String.Concat(message, ",網頁即將跳轉!")), url);
        }

        public static string ToAlertAndCloseAndRedirect(this string message, string url)
        {
            return String.Format("<script type='text/javascript'> alert({0}); window.close(); window.parent.location.href='{1}';</script>", ToJsonRaw(String.Concat(message, ",網頁即將關閉後跳轉!")), url);
        }

        public static string ToConfirmFormat(this string message, string trueAction, string falseAction = ";")
        {
            return String.Format(
            @"<script type='text/javascript'> 
				if(confirm(message))
					{0}
				else
					{1}
			   </script>"
            , trueAction, falseAction);
        }
    }
}