using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Report
{
    public partial class SalesmanAndCustomerSalesAmount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "SalesmanAndCustomerSalesAmount"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            PageTitle.Value = "業務員及客戶銷售金額表";
            if (!IsPostBack)
            {
                SalesmanAndCustomerSalesAmountFrame.Attributes.Add("Src", @"http://rpt.rinnai.com.tw/ReportServer/Pages/ReportViewer.aspx?%2fPortal%2f%e6%a5%ad%e5%8b%99%e5%93%a1%e5%8f%8a%e5%ae%a2%e6%88%b6%e9%8a%b7%e5%94%ae%e9%87%91%e9%a1%8d%e8%a1%a8&rs:Command=Render");
            }
        }
    }
}