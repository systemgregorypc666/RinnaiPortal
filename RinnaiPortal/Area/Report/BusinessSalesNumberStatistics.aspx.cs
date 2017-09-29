using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Report
{
    public partial class BusinessSalesNumberStatistics : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "BusinessSalesNumberStatistics"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            PageTitle.Value = "營業所銷貨台數統計表";
            if (!IsPostBack)
            {
                BusinessSalesNumberStatisticsFrame.Attributes.Add("Src", @"http://rpt.rinnai.com.tw/ReportServer/Pages/ReportViewer.aspx?%2fPortal%2f%e7%87%9f%e6%a5%ad%e6%89%80%e5%9f%ba%e6%ba%96%e5%9c%a8%e5%ba%ab%e8%a3%9c%e5%85%85%e7%ae%a1%e7%90%86%e8%a1%a8&rs:Command=Render");
            }
        }
    }
}