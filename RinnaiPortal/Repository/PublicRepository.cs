using RinnaiPortal.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Repository
{
    public static class PublicRepository
    {
        public static WorkflowTypeEnum CurrentWorkflowMode = WorkflowTypeEnum.RELEASE;
        /// <summary>
        /// To the full taiwan date.
        /// </summary>
        /// <param name="datetime">The datetime.</param>
        /// <returns></returns>
        public static string ToFullTaiwanDate(this DateTime datetime)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();

            return string.Format("{0}年{1}月{2}日",
                taiwanCalendar.GetYear(datetime),
                datetime.Month,
                datetime.Day);
        }

        //public static DateTime? ToDateTimeNullable(this string dateTime)
        //{
        //    DateTime validDate = new DateTime();
        //    if (dateTime.TryParseDateTimeExact(ref validDate, "yyyy-MM-dd"))
        //    {
        //        return validDate;
        //    }
        //    else
        //    {
        //        return (DateTime?)null;
        //    }
        //}

    }
}