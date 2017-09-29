using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ValidationAPI;
using ValidationAPI.Attributes;

namespace RinnaiPortal.ViewModel
{
    public class ProcedureViewModel
    {
        [Valiator(ValidateRuleOption.Required, "簽核代碼為必填欄位!")]
        public string SignID { get; set; }
        [Valiator(ValidateRuleOption.Required, "簽核層數為必填欄位!")]
        [Valiator(ValidateRuleOption.Number, "簽核層數值需為數字!")]
        [Valiator(ValidateRuleOption.GreaterThanZero, "簽核層數值需為大於零的整數!")]
        public int SignLevel { get; set; }
        [Valiator(ValidateRuleOption.Required, "最高簽核層級為必填欄位!")]
        [Valiator(ValidateRuleOption.Number, "最高簽核層級值需為數字!")]
        [Valiator(ValidateRuleOption.GreaterAndEqualZero, "最高簽核層級值需為大於等於零的整數!")]
        public int MaxLevel { get; set; }
        [Valiator(ValidateRuleOption.Required, "停用為必填欄位!")]
        [Valiator(ValidateRuleOption.Boolean, "停用值需為布林值!")]
        [Valiator(ValidateRuleOption.BooleanBindWith, "DisabledDate", "停用/帳號停用日期有誤!")]
        public bool Disabled { get; set; }
        [Valiator(ValidateRuleOption.DateFormatNullable, "轉換帳號停用日期格式失敗!")]
        public DateTime? DisabledDate { get; set; }
        public string TimeStamp { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifyDate { get; set; }
    }

}