using RinnaiPortal.Repository;
using RinnaiPortal.ViewModel;
using RinnaiPortal.Tools;
using RinnaiPortal.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ValidationAPI;
using RinnaiPortal.FactoryMethod;
using System.Drawing;
using RinnaiPortal.Interface;
using RinnaiPortal.ViewModel.Sign;

namespace RinnaiPortal
{
    public partial class Monthly_Result : DefaultDataList
    {
        private RootRepository _rootRepo = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _rootRepo = RepositoryFactory.CreateRootRepo();

                if (!String.IsNullOrWhiteSpace(Request["UserID"]))
                {
                    //ddl_DeptUser_SelectedIndexChanged(sender,e);
                    var employeeData = _rootRepo.QueryForEmployeeByEmpID(Request["UserID"]);
                    lbl_UserID.Text = employeeData["EmployeeID"].ToString();
                    lbl_UserName.Text = employeeData["EmployeeName"].ToString();
                    ddl_DeptUser.SelectedValue = Request["UserID"];
                    DeptUserName.Value = ddl_DeptUser.Text;
                    
                    //將主管資料 與下拉式選單綁定
                    ViewUtils.SetOptions(ddl_DeptUser, _rootRepo.GetDeptEmployee(employeeData["CostDepartmentID"].ToString()));
                    ddl_DeptUser.Items.Remove(ddl_DeptUser.Items.FindByValue(""));
                    //1.各月出勤統計
                    var paggerParms_Monthly = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
                    var signListParms_Monthly = WebUtils.ParseQueryString<SignListParms>(Page.Request);
                    signListParms_Monthly.Member = Authentication.GetMemberViewModel(User.Identity.Name);
                    signListParms_Monthly.EmployeeID_FK = employeeData["EmployeeID"].ToString();
                    signListParms_Monthly.PageName = "Monthly_Result";
                    signListParms_Monthly.GridView = MonthlyGridView;
                    //signListParms_Daily.PaginationBar_Daily = paginationBar_Daily;
                    //建構頁面
                    ConstructPage(signListParms_Monthly, paggerParms_Monthly, RepositoryFactory.CreateMonthlyRepo());
                    
                    //2.特休
                    var paggerParms_Recreate = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
                    var signListParms_Recreate = WebUtils.ParseQueryString<SignListParms>(Page.Request);
                    signListParms_Recreate.Member = Authentication.GetMemberViewModel(User.Identity.Name);
                    signListParms_Recreate.EmployeeID_FK = employeeData["EmployeeID"].ToString();
                    signListParms_Recreate.PageName = "Recreate_Result";
                    signListParms_Recreate.GridView = RecreateGridView;
                    //signListParms_Daily.PaginationBar_Daily = paginationBar_Daily;
                    //建構頁面
                    ConstructPage(signListParms_Recreate, paggerParms_Recreate, RepositoryFactory.CreateRecreateRepo());
                    //20170524 換休時數
                    var dateTimeNow = DateTime.Now;
                    var t = _rootRepo.Find_ADDOFFHOURS(employeeData["EmployeeID"].ToString(), dateTimeNow.ToString("yyyyMMdd"));
                    lb_ADDOFFHOURS.Text = _rootRepo.Find_ADDOFFHOURS(employeeData["EmployeeID"].ToString(), dateTimeNow.ToString("yyyyMMdd"));
                }
            }
        }

        protected void ddl_DeptUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            DeptUserName.Value = ddl_DeptUser.Text;
            Response.Redirect(@"/Monthly_Result.aspx?UserID=" + ddl_DeptUser.SelectedValue);
        }

        protected void MonthlyGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lb1_OFFWORK3 = (Label)e.Row.FindControl("OFFWORK3");
                if (double.Parse(lb1_OFFWORK3.Text) == 0)
                {
                    lb1_OFFWORK3.Text = "";
                }
                else
                {
                    lb1_OFFWORK3.ForeColor = Color.Red;
                }
                Label lb1_OffWork14 = (Label)e.Row.FindControl("OffWork14");
                if (double.Parse(lb1_OffWork14.Text) == 0)
                {
                    lb1_OffWork14.Text = "";
                }
                else
                {
                    lb1_OffWork14.ForeColor = Color.Red;
                }
                Label lb1_RECREATEDAYS = (Label)e.Row.FindControl("RECREATEDAYS");
                if (double.Parse(lb1_RECREATEDAYS.Text) == 0)
                {
                    lb1_RECREATEDAYS.Text = "";
                }
                Label lb1_OFFHOURS = (Label)e.Row.FindControl("OFFHOURS");
                if (double.Parse(lb1_OFFHOURS.Text) == 0)
                {
                    lb1_OFFHOURS.Text = "";
                }
                Label lb1_OFFWORK2 = (Label)e.Row.FindControl("OFFWORK2");
                if (double.Parse(lb1_OFFWORK2.Text) == 0)
                {
                    lb1_OFFWORK2.Text = "";
                }
                Label lb1_OFFWORK1 = (Label)e.Row.FindControl("OFFWORK1");
                if (double.Parse(lb1_OFFWORK1.Text) == 0)
                {
                    lb1_OFFWORK1.Text = "";
                }
                Label lb1_OFFWORK5M = (Label)e.Row.FindControl("OFFWORK5M");
                if (double.Parse(lb1_OFFWORK5M.Text) == 0)
                {
                    lb1_OFFWORK5M.Text = "";
                }
                else
                {
                    lb1_OFFWORK5M.ForeColor = Color.Red;
                }
                Label lb1_OFFWORK6M = (Label)e.Row.FindControl("OFFWORK6M");
                if (double.Parse(lb1_OFFWORK6M.Text) == 0)
                {
                    lb1_OFFWORK6M.Text = "";
                }
                else
                {
                    lb1_OFFWORK6M.ForeColor = Color.Red;
                }
                Label lb1_OFFWORK9 = (Label)e.Row.FindControl("OFFWORK9");
                if (double.Parse(lb1_OFFWORK9.Text) == 0)
                {
                    lb1_OFFWORK9.Text = "";
                }
                Label lb1_LOSTTIMES = (Label)e.Row.FindControl("LOSTTIMES");
                if (double.Parse(lb1_LOSTTIMES.Text) == 0)
                {
                    lb1_LOSTTIMES.Text = "";
                }
                else
                {
                    lb1_LOSTTIMES.ForeColor = Color.Red;
                }
                Label lb1_OFFWORKHOURS = (Label)e.Row.FindControl("OFFWORKHOURS");
                if (double.Parse(lb1_OFFWORKHOURS.Text) == 0)
                {
                    lb1_OFFWORKHOURS.Text = "";
                }
                Label lb1_OVERWORK1 = (Label)e.Row.FindControl("OVERWORK1");
                if (double.Parse(lb1_OVERWORK1.Text) == 0)
                {
                    lb1_OVERWORK1.Text = "";
                }
                Label lb1_OVERWORK2 = (Label)e.Row.FindControl("OVERWORK2");
                if (double.Parse(lb1_OVERWORK2.Text) == 0)
                {
                    lb1_OVERWORK2.Text = "";
                }
                Label lb1_OVERWORK3 = (Label)e.Row.FindControl("OVERWORK3");
                if (double.Parse(lb1_OVERWORK3.Text) == 0)
                {
                    lb1_OVERWORK3.Text = "";
                }
                Label lb1_OVERWORK4 = (Label)e.Row.FindControl("OVERWORK4");
                if (double.Parse(lb1_OVERWORK4.Text) == 0)
                {
                    lb1_OVERWORK4.Text = "";
                }
                Label lb1_MEALDELAY = (Label)e.Row.FindControl("MEALDELAY");
                if (double.Parse(lb1_MEALDELAY.Text) == 0)
                {
                    lb1_MEALDELAY.Text = "";
                }
                Label lb1_MEALDELAY2 = (Label)e.Row.FindControl("MEALDELAY2");
                if (double.Parse(lb1_MEALDELAY2.Text) == 0)
                {
                    lb1_MEALDELAY2.Text = "";
                }
                Label lb1_OVERWORKHOURS = (Label)e.Row.FindControl("OVERWORKHOURS");
                if (double.Parse(lb1_OVERWORKHOURS.Text) == 0)
                {
                    lb1_OVERWORKHOURS.Text = "";
                }
                Label lb1_ADDHOURS = (Label)e.Row.FindControl("ADDHOURS");
                if (double.Parse(lb1_ADDHOURS.Text) == 0)
                {
                    lb1_ADDHOURS.Text = "";
                }
                //Label lb1_ADDOFFHOURS = (Label)e.Row.FindControl("ADDOFFHOURS");
                //if (double.Parse(lb1_ADDOFFHOURS.Text) == 0)
                //{
                //    lb1_ADDOFFHOURS.Text = "";
                //}

            }
        }
    }
}