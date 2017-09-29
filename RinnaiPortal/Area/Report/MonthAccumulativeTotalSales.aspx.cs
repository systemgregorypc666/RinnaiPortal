using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Report
{
    public partial class MonthAccumulativeTotalSales : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Authentication.HasResource(User.Identity.Name, "MonthAccumulativeTotalSales"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            PageTitle.Value = "各所當月銷貨累計台數表";
            if (!IsPostBack)
            {
                MonthAccumulativeTotalSalesFrame.Attributes.Add("Src", @"http://rpt.rinnai.com.tw/ReportServer/Pages/ReportViewer.aspx?%2fPortal%2f%E5%90%84%E6%89%80%E7%95%B6%E6%9C%88%E9%8A%B7%E8%B2%A8%E7%B4%AF%E8%A8%88%E5%8F%B0%E6%95%B8%E8%A1%A8&rs:Command=Render");
            }
           
        }
    }
}