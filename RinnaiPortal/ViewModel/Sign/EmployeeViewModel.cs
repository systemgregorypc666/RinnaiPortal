using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using ValidationAPI;
using ValidationAPI.Attributes;

namespace RinnaiPortal.ViewModel
{
    public class EmployeeViewModel
    {
        [Valiator(ValidateRuleOption.Required, "員工編號為必填欄位!")]
        public string EmployeeID { get; set; }

        [Valiator(ValidateRuleOption.Required, "員工姓名為必填欄位!")]
        public string EmployeeName { get; set; }

        [Valiator(ValidateRuleOption.Required, "部門代碼為必填欄位!")]
        public string DepartmentID_FK { get; set; }
        public string CostDepartmentID { get; set; }

        [Valiator(ValidateRuleOption.Required, "部門名稱為必選!")]
        [Valiator(ValidateRuleOption.Number, "部門名稱值需為數字!")]
        [Valiator(ValidateRuleOption.GreaterThanZero, "部門名稱值需為大於零的整數!")]
        public int DepartmentID_FKSelectedIndex { get; set; }

        public string AgentID { get; set; }
        public string AgentName { get; set; }

        [Valiator(ValidateRuleOption.Required, "停用為必填欄位!")]
        [Valiator(ValidateRuleOption.Boolean, "停用值需為布林值!")]
        [Valiator(ValidateRuleOption.BooleanBindWith, "DisabledDate", "停用/帳號停用日期有誤!")]
        public bool Disabled { get; set; }

        [Valiator(ValidateRuleOption.DateFormatNullable, "轉換帳號停用日期格式失敗!")]
        public DateTime? DisabledDate { get; set; }

        //[Valiator(ValidateRuleOption.Required, "AD帳號為必填欄位!")]
        public string ADAccount { get; set; }

        [Valiator(ValidateRuleOption.Required, "權限類別為必填欄位!")]
        public string AccessType { get; set; }

        [Valiator(ValidateRuleOption.Required, "權限類別為必選!")]
        [Valiator(ValidateRuleOption.Number, "權限類別值需為數字!")]
        [Valiator(ValidateRuleOption.GreaterAndEqualZero, "權限類別值需為大於等於零的整數!")]
        public int AccessTypeSelectedIndex { get; set; }

        [Valiator(ValidateRuleOption.Required, "國籍為必填欄位!")]
        public string NationalType { get; set; }

        [Valiator(ValidateRuleOption.Required, "性別為必選!")]
        public string SexType { get; set; }

        [Valiator(ValidateRuleOption.Required, "國籍為必選!")]
        [Valiator(ValidateRuleOption.Number, "國籍值需為數字!")]
        [Valiator(ValidateRuleOption.GreaterThanZero, "國籍值需為大於零的整數!")]
        public int NationalTypeSelectedIndex { get; set; }

        public int SexTypeSelectedIndex { get; set; }

        public string TimeStamp { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifyDate { get; set; }

        public Dictionary<string, string> DepartmentDic { get; set; }
        public Dictionary<string, string> AgentNameDic { get; set; }
    }


}