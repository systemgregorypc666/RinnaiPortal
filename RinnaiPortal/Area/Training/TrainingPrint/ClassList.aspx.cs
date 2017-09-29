﻿using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Training.TrainingPrint
{
	public partial class ClassList : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "ClassList"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			PageTitle.Value = @"課程清單";
			if (!IsPostBack)
			{
				var domainSettings = ConfigUtils.ParsePageSetting("Domain");
				var domainName = domainSettings["Training"];
				ClassListFrame1.Attributes.Add("Src", String.Format("http://{0}/TClassList.aspx", domainName));
			}
		}
	}
}