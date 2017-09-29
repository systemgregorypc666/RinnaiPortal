using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.ViewModel.Sign
{
    public class SignListParms : ListParms
    {
        public string SignDocID { get; set; }
        public string FinalStatus { get; set; }
        public string LOGON_USER{ get; set; }
        public MemberViewModel Member { get; set; }
        public string CLID { get; set; }
        public string TABLE_ID { get; set; }
        public string EmployeeID_FK { get; set; }
        public string payYYYYMM { get; set; }
        public string PageName { get; set; }

        public SignListParms()
        {
            SignDocID = String.Empty;
            CLID = String.Empty;
            TABLE_ID = String.Empty;
        }

    }
}