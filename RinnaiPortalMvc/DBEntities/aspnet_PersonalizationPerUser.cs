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
    
    public partial class aspnet_PersonalizationPerUser
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> PathId { get; set; }
        public Nullable<System.Guid> UserId { get; set; }
        public byte[] PageSettings { get; set; }
        public System.DateTime LastUpdatedDate { get; set; }
    
        public virtual aspnet_Paths aspnet_Paths { get; set; }
        public virtual aspnet_Users aspnet_Users { get; set; }
    }
}
