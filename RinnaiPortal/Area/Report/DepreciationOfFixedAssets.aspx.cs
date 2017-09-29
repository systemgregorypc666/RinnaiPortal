using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Report
{
    public partial class DepreciationOfFixedAssets : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "DepreciationOfFixedAssets"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            PageTitle.Value = "固定資產折舊";
            if (!IsPostBack)
            {
                DepreciationOfFixedAssetsFrame.Attributes.Add("Src", @"http://rpt.rinnai.com.tw/ReportServer/Pages/ReportViewer.aspx?%2fPortal%2f%E5%9B%BA%E5%AE%9A%E8%B3%87%E7%94%A2%E6%8A%98%E8%88%8A&rs:Command=Render");
            }

        }
    }
}