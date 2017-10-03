using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.ViewModel.Sign.Forms
{
    public class OvertimeViewModel : RinnaiForms
    {
        public OvertimeViewModel()
        {
            SN = 0;
            Checkbox = false;
            EmployeeID_FK = String.Empty;
            EmployeeName = String.Empty;
            StartDateTime = null;
            EndDateTime = null;
            SupportDeptID_FK = String.Empty;
            SupportDeptName = String.Empty;
            Note = String.Empty;
            RealEndDateTime = String.Empty;
            PayTypeKey = String.Empty;
            PayTypeValue = String.Empty;
            MealOrderKey = String.Empty;
            MealOrderValue = String.Empty;
            NationType = String.Empty;
            FormSeries = "Overtime";
            AutoInsert = false;
        }
        public string ApplyDeptID { get; set; }
        public string ApplyDeptName { get; set; }
        public DateTime ApplyDateTime { get; set; }

        public bool Checkbox { get; set; }
        public string EmployeeID_FK { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentID_FK { get; set; }
        public string DepartmentName { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string RealEndDateTime { get; set; }
        public string SupportDeptID_FK { get; set; }
        public string SupportDeptName { get; set; }
        public string PayTypeKey { get; set; }
        public string PayTypeValue { get; set; }
        public string MealOrderKey { get; set; }
        public string MealOrderValue { get; set; }
        public string NationType { get; set; }
        //public string HolidayType { get; set; }
        public bool IsHoliday { get; set; }
        public double TotalHours { get; set; }

        public string WorkType { get; set; }
        public string CostDepartmentID { get; set; }
        public string BeginTime { get; set; }
        public string EndTime { get; set; }

    }
}