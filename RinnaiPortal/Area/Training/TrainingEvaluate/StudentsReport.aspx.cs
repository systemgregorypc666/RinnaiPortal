using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Training.TrainingEvaluate
{
	public partial class StudentsReport : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "StudentsReport"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			PageTitle.Value = @"受訓心得報告表維護";
			if (!IsPostBack)
			{
				var profileData = Authentication.LoginList.ContainsKey(User.Identity.Name) ? Authentication.LoginList[User.Identity.Name] : null;
				var employeeID = profileData != null ? String.Join("", profileData["EmployeeID"]) : null;

				var domainSettings = ConfigUtils.ParsePageSetting("Domain");
				var domainName = domainSettings["Training"];

				StudentsReportFrame1.Attributes.Add("Src", String.Format("http://{0}/webform12.aspx{1}", domainName, !String.IsNullOrWhiteSpace(employeeID) ? String.Format("?EmployeeID={0}", employeeID) : ""));
			}
		}
	}
}