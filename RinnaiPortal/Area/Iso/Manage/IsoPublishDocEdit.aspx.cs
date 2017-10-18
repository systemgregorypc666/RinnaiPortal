using RinnaiPortal.Repository;
using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Iso.Manage
{
    public partial class IsoPublishDocEdit : System.Web.UI.Page
    {
        private IsoRepository Repository = new IsoRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "IsoPublishDocEdit"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            if (!IsPostBack)
            {
                //var docID = Convert.ToInt16(Request["docId"]);
                //Detalis = Repository.GetIsoAppDocumentByIDForManage(docID);

                //string statusDesc = string.Empty;
                ////判斷號碼是否已取得
                //switch (Detalis.ApplicationStatus)
                //{
                //    case "W":
                //        statusDesc = "待審核";
                //        break;

                //    case "Y":
                //        statusDesc = "已核准";
                //        break;

                //    case "N":
                //        statusDesc = "拒絕";
                //        break;

                //    default:
                //        break;
                //}
  
            }

        }
    }
}