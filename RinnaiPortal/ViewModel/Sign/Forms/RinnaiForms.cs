using System;

namespace RinnaiPortal.ViewModel.Sign.Forms
{
    /// <summary>
    /// 前端頁面的基底模型
    /// </summary>
    public class RinnaiForms
    {
        public RinnaiForms()
        {
        }

        public int SN { get; set; }
        public int FormID_FK { get; set; }
        public string SignDocID_FK { get; set; }
        public string ApplyID_FK { get; set; }
        public string ApplyName { get; set; }
        public string RuleID_FK { get; set; }
        public string FormSeries { get; set; }
        public bool AutoInsert { get; set; }
        public string Note { get; set; }
        public string Creator { get; set; }
        public DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}