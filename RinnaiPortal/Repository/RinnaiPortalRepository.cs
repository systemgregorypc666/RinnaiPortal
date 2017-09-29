using DBTools;
using RinnaiPortal.Repository.Sign;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Repository
{
    public class RinnaiPortalRepository 
    {
        private DB _dc { get; set; }
        public RinnaiPortalRepository(DB dc)
        {
            _dc = dc;
        }

        public DataTable GetMenuData(string resource) 
        {
            string strSQL = String.Format("select nodeid,parentid,nodetitle,nodeuri from SystemArchitecture where Active = 'true' and ResourceAlias in ({0})", resource);
            var dt = _dc.QueryForDataTable(strSQL, null);
            return dt;
        }
    }
}