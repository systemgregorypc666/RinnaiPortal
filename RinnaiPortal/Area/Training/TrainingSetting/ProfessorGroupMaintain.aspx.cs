using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Training.BasicSetting
{
	public partial class ProfessorGroupMaintain : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "ProfessorGroupMaintain"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			PageTitle.Value = @"講師基本資料查詢\修改";
			if (!IsPostBack)
			{
				var domainSettings = ConfigUtils.ParsePageSetting("Domain");
				var domainName = domainSettings["Training"];
				ProfessorGroupMaintainFrame1.Attributes.Add("Src", String.Format("http://{0}/Teacher_Group_Edit.aspx", domainName));
			}
		}
	}
}