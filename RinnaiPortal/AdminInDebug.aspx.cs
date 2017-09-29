using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace RinnaiPortal
{
    public partial class AdminInDebug : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["DebugEmpName"] == null)
            {
                var empName = Request.QueryString["empId"];
                Session["DebugEmpName"] = empName;
            }
            hdnEmpName.Value = Session["DebugEmpName"].ToString();
        }
    }
}