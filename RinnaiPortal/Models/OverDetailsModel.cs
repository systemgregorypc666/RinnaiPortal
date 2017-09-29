using System;
using System.Collections.Generic;

namespace RinnaiPortal.Models
{
    public class OverDetailsModel
    {
        public OverDetailsDataModel[] Data { get; set; }

        public class OverDetailsDataModel
        {
            public int SN { get; set; }
            public string Type { get; set; }
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string DepartmentID { get; set; }
            public string DepartmentName { get; set; }
            public DateTime StartDateTime { get; set; }
            public DateTime EndDateTime { get; set; }
            public string SupportDeptID { get; set; }
            public string SupportDeptName { get; set; }
            public string PayTypeKey { get; set; }
            public string MealOrderKey { get; set; }
            public string NationalType { get; set; }
            public string Note { get; set; }
            public string RealStartDateTime { get; set; }
            public string RealEndDateTime { get; set; }
            public string RealEndDateTime1 { get; set; }
            public float TotalHours { get; set; }
            public float totalH { get; set; }
        }
    }
}