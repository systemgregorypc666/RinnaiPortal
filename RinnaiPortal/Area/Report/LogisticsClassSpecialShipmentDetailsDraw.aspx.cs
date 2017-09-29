using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Report
{
    public partial class LogisticsClassSpecialShipmentDetailsDraw : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "LogisticsClassSpecialShipmentDetailsDraw"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            PageTitle.Value = "物流課專用出貨明細表 > 卓課";
            if (!IsPostBack)
            {
                LogisticsClassSpecialShipmentDetailsDrawFrame.Attributes.Add("Src", @"http://rpt.rinnai.com.tw/ReportServer/Pages/ReportViewer.aspx?%2fPortal%2f%E7%89%A9%E6%B5%81%E8%AA%B2%E5%B0%88%E7%94%A8%E5%87%BA%E8%B2%A8%E6%98%8E%E7%B4%B0%E8%A1%A8(%E5%8D%93%E8%AA%B2)&rs:Command=Render");
            }
        }
    }
}