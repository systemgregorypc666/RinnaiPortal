using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Report
{
    public partial class ProductionSalesStockNew : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "ProductionSalesStockNew"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            PageTitle.Value = "產銷存報表 > (新)";
            if (!IsPostBack)
            {
                ProductionSalesStockNewFrame.Attributes.Add("Src", @"http://rpt.rinnai.com.tw/ReportServer/Pages/ReportViewer.aspx?%2fPortal%2f%e7%94%a2%e9%8a%b7%e5%ad%98%e5%a0%b1%e8%a1%a8(%e6%96%b0)&rs:Command=Render");
            }
        }
    }
}