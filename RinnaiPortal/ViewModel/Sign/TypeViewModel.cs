using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ValidationAPI;
using ValidationAPI.Attributes;

namespace RinnaiPortal.ViewModel
{
    public class TypeViewModel
    {
        public int FormID { get; set; }

        [Valiator(ValidateRuleOption.Required, "表單類型為必填欄位!")]
        public string FormType { get; set; }

        [Valiator(ValidateRuleOption.Required, "簽核層級代碼為必填欄位!")]
        public string SignID_FK { get; set; }

        [Valiator(ValidateRuleOption.Required, "簽核層級代碼為必選!")]
        [Valiator(ValidateRuleOption.Number, "簽核層級代碼值需為數字!")]
        [Valiator(ValidateRuleOption.GreaterThanZero, "簽核層級代碼值需為大於零的整數!")]
        public int SignID_FKSelectedIndex { get; set; }

        [Valiator(ValidateRuleOption.Required, "歸檔部門名稱為必填!")]
        public string FilingDepartmentID_FK { get; set; }

        [Valiator(ValidateRuleOption.Required, "歸檔部門名稱為必選!")]
        [Valiator(ValidateRuleOption.Number, "歸檔部門值需為數字!")]
        [Valiator(ValidateRuleOption.GreaterThanZero, "歸檔部門值需為大於零的整數!")]
        public int FilingDepartmentID_FKSelectedIndex { get; set; }

        public string TimeStamp { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifyDate { get; set; }

        public Dictionary<string, string> SignIDDic { get; set; }
        public Dictionary<string, string> DepartmentDic { get; set; }
    }
}