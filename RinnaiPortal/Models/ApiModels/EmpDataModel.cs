using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Models.ApiModels
{
    /// <summary>
    /// 提供給api使用的員工資料模型，僅提供部分資料
    /// </summary>
    public class EmpDataModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeADAccount { get; set; }
        public string DepartmentID { get; set; }
        public string NationalType { get; set; }
    }
}