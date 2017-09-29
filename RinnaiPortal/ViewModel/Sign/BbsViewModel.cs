using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ValidationAPI;
using ValidationAPI.Attributes;

namespace RinnaiPortal.ViewModel
{
    public class BbsViewModel
    {
        [Valiator(ValidateRuleOption.Required, "主題為必填欄位!")]
        public string txt_Title { get; set; }
        public int bbs_id { get; set; }
        
        [Valiator(ValidateRuleOption.Required, "公告內容為必填欄位!")]
        public string txt_Content { get; set; }
                     
        
        public DateTime? DefaultStartDateTime { get; set; }

        //[Valiator(ValidateRuleOption.Required, "起始日期為必填欄位!")]

        
        public DateTime? DefaultEndDateTime { get; set; }

        //[Valiator(ValidateRuleOption.Required, "結束日期為必填欄位!")]
        public string txt_Http { get; set; }
        public string PhotoName { get; set; }
        public string UpName { get; set; }
        public string TimeStamp { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifyDate { get; set; }

        public Dictionary<string, string> DepartmentDic { get; set; }
        public Dictionary<string, string> EmployeeDic { get; set; }
    }
}