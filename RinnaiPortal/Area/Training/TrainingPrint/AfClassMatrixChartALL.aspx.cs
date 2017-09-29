using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Training.TrainingPrint
{
	public partial class AfClassMatrixChartALL : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "AfClassMatrixChartALL"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			PageTitle.Value = @"課後滿意度調查統計分析彙整表";
			if (!IsPostBack)
			{
				var domainSettings = ConfigUtils.ParsePageSetting("Domain");
				var domainName = domainSettings["Training"];
				AfClassMatrixChartALLFrame1.Attributes.Add("Src", String.Format("http://{0}/AfClassMatrixChartALL.aspx", domainName));
			}
		}
	}
}