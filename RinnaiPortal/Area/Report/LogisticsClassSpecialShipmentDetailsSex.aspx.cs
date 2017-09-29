using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Report
{
    public partial class LogisticsClassSpecialShipmentDetailsSex : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "LogisticsClassSpecialShipmentDetailsSex"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            PageTitle.Value = "物流課專用出貨明細表 > 幸智";
            if (!IsPostBack)
            {
                LogisticsClassSpecialShipmentDetailsSexFrame.Attributes.Add("Src", @"http://rpt.rinnai.com.tw/ReportServer/Pages/ReportViewer.aspx?%2fPortal%2f%e7%89%a9%e6%b5%81%e8%aa%b2%e5%b0%88%e7%94%a8%e5%87%ba%e8%b2%a8%e6%98%8e%e7%b4%b0%e8%a1%a8(%e5%b9%b8%e6%99%ba)&rs:Command=Render");
            }
        }
    }
}