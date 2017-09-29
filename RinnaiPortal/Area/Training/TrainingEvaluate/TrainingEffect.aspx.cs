using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Training.TrainingEvaluate
{
	public partial class TrainingEffect : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "TrainingEffect"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			PageTitle.Value = @"訓練成效調查維護 ";
			if (!IsPostBack)
			{
				var profileData = Authentication.LoginList.ContainsKey(User.Identity.Name) ? Authentication.LoginList[User.Identity.Name] : null;
				var employeeID = profileData != null ? String.Join("", profileData["EmployeeID"]) : null;

				var domainSettings = ConfigUtils.ParsePageSetting("Domain");
				var domainName = domainSettings["Training"];

				TrainingEffectFrame1.Attributes.Add("Src", String.Format("http://{0}/Training_Effect.aspx{1}", domainName, !String.IsNullOrWhiteSpace(employeeID) ? String.Format("?EmployeeID={0}", employeeID) : ""));
			}
		}
	}
}