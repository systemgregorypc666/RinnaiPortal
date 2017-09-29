using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ValidationAPI;
using ValidationAPI.Attributes;

namespace RinnaiPortal.ViewModel.Sign
{
    public class GroupViewModel
    {
        [Valiator(ValidateRuleOption.Required, "群組代碼為必填欄位!")]
        public String GroupType { get; set; }

        [Valiator(ValidateRuleOption.Required, "群組名稱為必填欄位!")]
        public String GroupName { get; set; }

        [Valiator(ValidateRuleOption.Required, "存取資源為必填欄位!")]
        public String Resource { get; set; }

        [Valiator(ValidateRuleOption.Required, "存取資源為必選!")]
        [Valiator(ValidateRuleOption.Number, "存取資源值需為數字!")]
        [Valiator(ValidateRuleOption.GreaterAndEqualZero, "存取資源值需為大於等於零的整數!")]
        public int ResourceSelectedIndex { get; set; }

        public string TimeStamp { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}