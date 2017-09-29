using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Report
{
    public partial class DeliveryStaffPerformanceDetailsDashOne : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "InventorySummary"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            PageTitle.Value = "配送人員業績明細表 > -1";
            if (!IsPostBack)
            {
                DeliveryStaffPerformanceDetailsDashOneFrame.Attributes.Add("Src", @"http://rpt.rinnai.com.tw/ReportServer/Pages/ReportViewer.aspx?%2fPortal%2f%E9%85%8D%E9%80%81%E4%BA%BA%E5%93%A1%E6%A5%AD%E7%B8%BE%E6%98%8E%E7%B4%B0%E8%A1%A8-1&rs:Command=Render");
            }
        }
    }
}