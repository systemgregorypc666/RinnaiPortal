using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ValidationAPI;
using ValidationAPI.Attributes;

namespace RinnaiPortal.ViewModel
{
    public class AgentViewModel
    {
        public int SN { get; set; }
        [Valiator(ValidateRuleOption.Required, "員工姓名為必選!")]
        [Valiator(ValidateRuleOption.Number, "員工姓名值需為數字!")]
        [Valiator(ValidateRuleOption.GreaterThanZero, "員工姓名值需為大於零的整數!")]
        public int EmployeeID_FKSelectedIndex { get; set; }
        [Valiator(ValidateRuleOption.Required, "員工編號為必填欄位!")]
        public string EmployeeID_FK { get; set; }
        [Valiator(ValidateRuleOption.Required, "起始日期為必填欄位!")]
        [Valiator(ValidateRuleOption.DateFormat, "轉換起始日期格式失敗!")]
        [Valiator(ValidateRuleOption.DateLessThan, "EndDate", "結束日期不得小於起始日期!")]
        public DateTime BeginDate { get; set; }
        [Valiator(ValidateRuleOption.Required, "結束日期為必填欄位!")]
        [Valiator(ValidateRuleOption.DateFormat, "轉換結束日期格式失敗!")]
        public DateTime EndDate { get; set; }
        public string TimeStamp { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifyDate { get; set; }

        public Dictionary<string, string> EmployeeDic { get; set; }
    }
}