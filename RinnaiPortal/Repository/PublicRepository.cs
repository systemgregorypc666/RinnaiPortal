using RinnaiPortal.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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


        /// <summary>
        /// 儲存錯誤訊息Log檔(不覆寫)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        public static void SaveMesagesToTextFile(string path, string fileName, string content)
        {
            Exception error = null;
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (!File.Exists(path + fileName))
                {
                    using (StreamWriter sw = File.CreateText(path + fileName))
                    {
                        sw.WriteLine(DateTime.Now);
                        sw.WriteLine(content);
                        sw.WriteLine("-------------------------------------");
                        sw.WriteLine("");
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path + fileName))
                    {
                        sw.WriteLine(DateTime.Now);
                        sw.WriteLine(content);
                        sw.WriteLine("-------------------------------------");
                        sw.WriteLine("");
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
        }
        /// <summary>
        /// 返回例外的詳細資訊
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ExceptionDetalisMessages(Exception ex)
        {
            var e1 = ex.Source;
            var e2 = ex.StackTrace;
            var e3 = ex.Message;
            var e4 = ex.InnerException;
            return string.Format("exception:(method:{0},description:{1},innerException:{2}, messages:{3})", e1, e2, e3, e4);
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