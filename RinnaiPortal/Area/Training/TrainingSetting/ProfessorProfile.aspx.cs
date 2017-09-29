using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Training.BasicSetting
{
	public partial class ProfessorProfile : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "ProfessorProfile"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			PageTitle.Value = "講師基本資料新增";
			if (!IsPostBack)
			{
                var domainSettings = ConfigUtils.ParsePageSetting("Domain");
                var domainName = domainSettings["Training"];
                ProfessorProfileFrame1.Attributes.Add("Src", String.Format("http://{0}/Teacher_Data_Add.aspx", domainName));
			}
		}
	}
}