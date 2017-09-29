using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Training.InfoMaintain
{
	public partial class StudentsTrainingMaintain : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "StudentsTrainingMaintain"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			PageTitle.Value = @"學員訓練維護";
			if (!IsPostBack)
			{
				var domainSettings = ConfigUtils.ParsePageSetting("Domain");
				var domainName = domainSettings["Training"];
				StudentsTrainingMaintainFrame1.Attributes.Add("Src", String.Format("http://{0}/webform22.aspx", domainName));
			}
		}
	}
}