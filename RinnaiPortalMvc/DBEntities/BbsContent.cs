//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace RinnaiPortalMvc.DBEntities
{
    using System;
    using System.Collections.Generic;
    
    public partial class BbsContent
    {
        public int bbs_id { get; set; }
        public string bbs_title { get; set; }
        public string bbs_content { get; set; }
        public Nullable<System.DateTime> startdatetime { get; set; }
        public Nullable<System.DateTime> enddatetime { get; set; }
        public string bbs_http { get; set; }
        public string bbs_photo { get; set; }
        public string bbs_file { get; set; }
        public string Creator { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string Modifier { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
    }
}
