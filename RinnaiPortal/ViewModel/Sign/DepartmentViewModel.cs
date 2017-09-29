using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ValidationAPI;
using ValidationAPI.Attributes;

namespace RinnaiPortal.ViewModel
{
    public class DepartmentViewModel
    {
        [Valiator(ValidateRuleOption.Required, "部門代碼為必填欄位!")]
        public string DepartmentID { get; set; }

        [Valiator(ValidateRuleOption.Required, "部門名稱為必填欄位!")]
        public string DepartmentName { get; set; }

        [Valiator(ValidateRuleOption.Required, "主管員工編號為必填欄位!")]
        public string ChiefID_FK { get; set; }

        [Valiator(ValidateRuleOption.Required, "主管姓名為必選!")]
        [Valiator(ValidateRuleOption.Number, "主管姓名值需為數字!")]
        [Valiator(ValidateRuleOption.GreaterThanZero, "主管姓名值需為大於零的整數!")]
        public int ChiefID_FKSelectedIndex { get; set; }

        [Valiator(ValidateRuleOption.Required, "上層部門代碼為必填欄位!")]
        public string UpperDepartmentID { get; set; }

        [Valiator(ValidateRuleOption.Required, "上層部門名稱為必選!")]
        [Valiator(ValidateRuleOption.Number, "上層部門名稱值需為數字!")]
        [Valiator(ValidateRuleOption.GreaterThanZero, "上層部門名稱值需為大於零的整數!")]
        public int UpperDepartmentIDSelectedIndex { get; set; }

        [Valiator(ValidateRuleOption.Required, "層級為必填欄位!")]
        [Valiator(ValidateRuleOption.Number, "層級值需為數字!")]
        [Valiator(ValidateRuleOption.GreaterAndEqualZero, "層級值需為大於等於零的整數!")]
        public int DepartmentLevel { get; set; }

        [Valiator(ValidateRuleOption.Required, "歸檔員工編號為必填欄位!")]
        public string FilingEmployeeID_FK { get; set; }

        [Valiator(ValidateRuleOption.Required, "歸檔員工姓名為必選!")]
        [Valiator(ValidateRuleOption.Number, "歸檔員工姓名值需為數字!")]
        [Valiator(ValidateRuleOption.GreaterThanZero, "歸檔員工姓名值需為大於零的整數!")]
        public int FilingEmployeeID_FKSelectedIndex { get; set; }

        [Valiator(ValidateRuleOption.Required, "停用為必填欄位!")]
        [Valiator(ValidateRuleOption.Boolean, "停用值需為布林值!")]
        [Valiator(ValidateRuleOption.BooleanBindWith, "DisabledDate", "停用/停用日期有誤!")]
        public bool Disabled { get; set; }

        [Valiator(ValidateRuleOption.DateFormatNullable, "轉換停用日期格式失敗!")]
        public DateTime? DisabledDate { get; set; }

        public string TimeStamp { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifyDate { get; set; }

        public Dictionary<string, string> DepartmentDic { get; set; }
        public Dictionary<string, string> EmployeeDic { get; set; }
    }
}