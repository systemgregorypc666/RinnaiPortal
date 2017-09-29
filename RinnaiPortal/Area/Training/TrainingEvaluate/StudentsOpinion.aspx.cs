using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Training.TrainingEvaluate
{
    public partial class StudentsOpinion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "StudentsOpinion"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            PageTitle.Value = @"學員意見調查表維護";
            if (!IsPostBack)
            {
                var profileData = Authentication.LoginList.ContainsKey(User.Identity.Name) ? Authentication.LoginList[User.Identity.Name] : null;
                var employeeID = profileData != null ? String.Join("", profileData["EmployeeID"]) : null;

                var domainSettings = ConfigUtils.ParsePageSetting("Domain");
                var domainName = domainSettings["Training"];
                StudentsOpinionFrame1.Attributes.Add("Src", String.Format("http://{0}/WebForm12.aspx{1}", domainName, !String.IsNullOrWhiteSpace(employeeID) ? String.Format("?EmployeeID={0}", employeeID) : ""));
            }
        }
    }
}