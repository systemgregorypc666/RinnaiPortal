using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Report
{
    public partial class ReturnDailyReportWithDiscount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "ReturnDailyReportWithDiscount"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            PageTitle.Value = "退貨日報表(含折讓)";
            if (!IsPostBack)
            {
                ReturnDailyReportWithDiscountFrame.Attributes.Add("Src", @"http://rpt.rinnai.com.tw/ReportServer/Pages/ReportViewer.aspx?%2fPortal%2f%e9%80%80%e8%b2%a8%e6%97%a5%e5%a0%b1%e8%a1%a8(%e5%90%ab%e6%8a%98%e8%ae%93)&rs:Command=Render");
            }
        }
    }
}