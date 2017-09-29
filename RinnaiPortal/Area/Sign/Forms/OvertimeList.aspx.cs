using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository.Sign.Forms;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.ViewModel.Sign.Forms;
using RinnaiPortal.Repository.SmartMan;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Sign.Forms
{
    //簽核頁面中的加班單清單
    public partial class OvertimeList : SignDataList
    {
        private ProcessWorkflowViewModel model = null;
        private OvertimeRepository _overitmeRepo = null;
        private SmartManRepository _smartRepo = null;
        private RootRepository _rootRepo = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "OvertimeList"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            if (!IsPostBack)
            {
                _overitmeRepo = RepositoryFactory.CreateOvertimeRepo();
                _rootRepo = RepositoryFactory.CreateRootRepo();
                _smartRepo = RepositoryFactory.CreateSmartManRepo();

                // 取得 QueryString
                var paggerParms = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
                var signListParms = WebUtils.ParseQueryString<SignListParms>(Page.Request);
                signListParms.GridView = OvertimeGridView;
                signListParms.PaginationBar = paginationBar;

                //根據查詢的 簽核代碼 搜尋加班單
                var pagination = _overitmeRepo.GetOvertimeListPagination(signListParms, paggerParms);

                if (pagination == null) { return; }
                
                //設定 gridView Source
                ViewUtils.SetGridView(OvertimeGridView, pagination.Data);

                //Pagination Bar Generator
                string paginationHtml = WebUtils.GetPagerNumericString(pagination, Request);
                paginationBar.InnerHtml = paginationHtml;


                model = _overitmeRepo.GetWorkflowData(signListParms.SignDocID);
                WebUtils.PageDataBind(model, this.Page);
                Signed.NavigateUrl = "~/Area/Sign/WorkflowDetail.aspx?signDocID=" + signListParms.SignDocID;
                if (model == null) { return; }
                var employeeData = _rootRepo.QueryForEmployeeByEmpID(model.EmployeeID_FK);
                ApplyName.Text = employeeData != null ? employeeData["EmployeeName"].ToString() : String.Empty;
            }
        }

        protected void OvertimeGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lb1_total1 = (Label)e.Row.FindControl("lbl_total");
                Label lbl_Alltotal = (Label)e.Row.FindControl("lbl_Alltotal");
                Label lbl_EmployeeID1 = (Label)e.Row.FindControl("lbl_EmployeeID");
                Label lbl_StartDateTime1 = (Label)e.Row.FindControl("lbl_StartDateTime");

                var OverWork_Form = _rootRepo.QueryOverWork(DateTime.Parse(lbl_StartDateTime1.Text).ToString("yyyyMMdd"), lbl_EmployeeID1.Text);
                var OverWork_smart = _smartRepo.QueryOverWork(DateTime.Parse(lbl_StartDateTime1.Text).ToString("yyyyMMdd"), lbl_EmployeeID1.Text);
                //20170310 add增加含國定假日加班總時數欄位
                var AllOverWork_Form = _rootRepo.QueryAllOverWork(DateTime.Parse(lbl_StartDateTime1.Text).ToString("yyyyMMdd"), lbl_EmployeeID1.Text);
                var AllOverWork_smart = _smartRepo.QueryAllOverWork(DateTime.Parse(lbl_StartDateTime1.Text).ToString("yyyyMMdd"), lbl_EmployeeID1.Text);
                var TotalOver = double.Parse(OverWork_Form["total"].ToString()) + double.Parse(OverWork_smart["totalH"].ToString());
                var TotalAllOver = double.Parse(AllOverWork_Form["total"].ToString()) + double.Parse(AllOverWork_smart["totalH"].ToString());
                lb1_total1.Text = TotalOver.ToString();
                lbl_Alltotal.Text = TotalAllOver.ToString();
            }

        }



    }
}