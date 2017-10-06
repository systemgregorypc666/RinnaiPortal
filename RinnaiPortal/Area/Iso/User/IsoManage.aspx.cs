using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Iso.User
{
    public partial class IsoManage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "IsoManage"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

        }
    }
}