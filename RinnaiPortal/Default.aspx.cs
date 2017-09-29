using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign;
using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal
{
    public partial class _Default : DefaultDataList
    {
        private RootRepository _rootRepo = null;

        //private NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private BbsRepository _bbsRepo = null;

        //Logical.Notice no = new global::MyforepiNotice_Project.Logical.Notice();
        protected void Page_Load(object sender, EventArgs e)
        {
            _rootRepo = RepositoryFactory.CreateRootRepo();
            if (!IsPostBack)
            {
                //1.Daily
                // 取得 QueryString
                var employeeData = _rootRepo.QueryForEmployeeByADAccount(User.Identity.Name);

             

                var paggerParms_Daily = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
                var signListParms_Daily = WebUtils.ParseQueryString<SignListParms>(Page.Request);
                signListParms_Daily.Member = Authentication.GetMemberViewModel(User.Identity.Name);
                signListParms_Daily.EmployeeID_FK = employeeData["EmployeeID"].ToString();

                signListParms_Daily.PageName = "Default";
                //signListParms_Daily.Member.EmployeeID = employeeData["EmployeeID"].ToString();
                signListParms_Daily.GridView = DailyGridView;
                //signListParms_Daily.PaginationBar_Daily = paginationBar_Daily;
                //建構頁面
                ConstructPage_Daily(signListParms_Daily, paggerParms_Daily, RepositoryFactory.CreateMonthlyRepo());
                //2.bbs
                // 取得 QueryString
                var paggerParms = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
                var signListParms = WebUtils.ParseQueryString<SignListParms>(Page.Request);
                signListParms.Member = Authentication.GetMemberViewModel(User.Identity.Name);
                signListParms.GridView = BbsGridView;
                //signListParms.TotalRowsCount = totalRowsCount;
                signListParms.PaginationBar = paginationBar;
                //signListParms.NoDataTip = noDataTip;

                //建構頁面
                ConstructPage(signListParms, paggerParms, RepositoryFactory.CreateBbsListRepo());
                Monthly.NavigateUrl = "Monthly_Result.aspx?UserID=" + employeeData["EmployeeID"].ToString();
                //pageSizeSelect.Text = paggerParms.PageSize.ToString();
                //queryTextBox.Text = signListParms.QueryText;


            }
        }

        protected void DailyGridView_RowDataBound(object sender, GridViewRowEventArgs e)
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

        //protected void fuck_Click(object sender, EventArgs e)
        //{
        //    GlobalDiagnosticsContext.Set("User", User.Identity.Name);
        //    _log.Trace("hi");
        //}
    }
}