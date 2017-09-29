using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Report
{
    public partial class SalesmanCheckoutPerformance : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "SalesmanCheckoutPerformance"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            PageTitle.Value = "業務員結帳業績表";
            if (!IsPostBack)
            {
                SalesmanCheckoutPerformanceFrame.Attributes.Add("Src", @"http://rpt.rinnai.com.tw/ReportServer/Pages/ReportViewer.aspx?%2fPortal%2f%e6%a5%ad%e5%8b%99%e5%93%a1%e7%b5%90%e5%b8%b3%e6%a5%ad%e7%b8%be%e8%a1%a8&rs:Command=Render");
            }
        }
    }
}