using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ValidationAPI;
using ValidationAPI.Attributes;

namespace RinnaiPortal.ViewModel.Sign.Forms
{
    /// <summary>
    /// 頁面的對應模型
    /// </summary>
    public class TrainViewModel : RinnaiForms
    {
        public TrainViewModel()
        {
            FormSeries = "Train";
        }
        public DateTime ApplyDateTime { get; set; }
        public string DepartmentID_FK { get; set; }
        public string DepartmentName { get; set; }
        public string CLID { get; set; }
        public string SID { get; set; }
        public string CLNAME { get; set; }
        public DateTime? Start_Date { get; set; }
        public string HOURS { get; set; }
        public string SNAME { get; set; }

    }
}