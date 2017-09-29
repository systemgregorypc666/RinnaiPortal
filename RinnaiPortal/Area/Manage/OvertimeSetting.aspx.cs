using DBTools;
using RinnaiPortal.Area.Sign;
using RinnaiPortal.Extensions;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository.Manage;
using RinnaiPortal.Repository.SmartMan;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ValidationAPI;

namespace RinnaiPortal.Area.Manage
{
	public partial class OvertimeSetting : System.Web.UI.Page
	{
		private OvertimeSettingRepository _repo = RepositoryFactory.CreateOvertimeSettingRepo();
		private SmartManRepository _smartmanRepo = RepositoryFactory.CreateSmartManRepo();
		private RootRepository _rootRepo = RepositoryFactory.CreateRootRepo();
		private AutoInsertHandler _handler = null;


		protected void Page_Load(object sender, EventArgs e)
		{
            if (!Authentication.HasResource(User.Identity.Name, "OvertimeReport"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			if (!IsPostBack)
			{

				//從QueryString取得 簽核文件代碼
				if (String.IsNullOrWhiteSpace(Request["SignDocID"]))
				{
					Response.Write("需要簽核文件代碼!".ToAlertFormat());
					return;
				}


				// 取得 QueryString
				var paggerParms = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
				var manageListParms = WebUtils.ParseQueryString<ManageListParms>(Page.Request);
				manageListParms.GridView = OvertimeSettingGridView;
				manageListParms.PaginationBar = paginationBar;

				//查詢計薪資料
				ViewUtils.SetOptions(PayRange, _smartmanRepo.QueryForPayRange().ToDictionary(k => k.Key, v => v.Value));

				//根據查詢的 簽核代碼 搜尋加班單
				var pagination = _repo.GetPagination(manageListParms, paggerParms);

				if (pagination == null) { return; }

				//設定 gridView Source
				ViewUtils.SetGridView(OvertimeSettingGridView, pagination.Data);

				//Pagination Bar Generator
				string paginationHtml = WebUtils.GetPagerNumericString(pagination, Request);
				paginationBar.InnerHtml = paginationHtml;
                

                    
			}
		}

		protected void SaveBtn_Click(object sender, EventArgs e)
		{

			//btn處理
			ViewUtils.ButtonOff(SaveBtn, CoverBtn);


			//存檔
			var responseMessage = "";
			var successRdUrl = String.Empty;

			try
			{
				var signDocID = Request["SignDocID"];
				_handler = RepositoryFactory.CreateAutoInsert(signDocID);

				var payRange = _handler.ParsePayRange(PayRange.SelectedValue);
				var columns = OvertimeSettingGridView.Columns.Cast<DataControlField>();               
                //var paggerParms = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
                //var manageListParms = WebUtils.ParseQueryString<ManageListParms>(Page.Request);
                //var pagination = _repo.GetPagination(manageListParms, paggerParms);
                //ViewUtils.SetGridView(OvertimeSettingGridView, pagination.Data);
			    var rows = OvertimeSettingGridView.Rows.Cast<GridViewRow>();
                        
				List<MultiConditions> dmlList = rows.Select(row =>
				{
					var controls = row.Controls.Cast<Control>();
					var data = controls.Zip(columns, (control, column) =>
					 {
						 var value = ((Label)control.FindControl(column.HeaderText)).Text;
						 return new KeyValuePair<string, object>(column.HeaderText, value);
					 }).ToDictionary(k => k.Key, v => v.Value);

					// ViewModelMapping 
					var model = WebUtils.ViewModelMapping<OvertimeViewModel>(data);

					// check employee is settle account or not
					if (_handler.IsSettledAccounts(model.EmployeeID_FK, payRange))
					{
						throw new Exception(String.Format("員工{0}已經結過{1}月薪資", model.EmployeeID_FK, payRange.Date.ToString("yyyyMM")));
					}
					// get dml
					return _handler.GetOvertimeDML(model, payRange);

				}).ToList();

				var workFlowRepo = RepositoryFactory.CreateProcessWorkflowRepo();
				//insert index:0 XACT_ABORT ON
				dmlList.Insert(0, _handler.GetXACTABORTON());
				//update autoinsert true
				dmlList.Add(_repo.GetAutoInserDML(signDocID, User.Identity.Name));
				//insert into db
                //workFlowRepo.ExecuteSQL(dmlList);

                responseMessage = "成功寫入SmartMan".ToAlertAndCloseAndRedirect(@"OvertimeReport.aspx?finalStatus=6&StartDateTime=" + Session["StartDateTime"] + "&EndDateTime=" + Session["EndDateTime"] + "&orderField=AutoInsert&descending=False");
			}
			catch (Exception ex)
			{
				responseMessage = String.Concat("寫入SmartMan失敗!\r\n錯誤訊息: ", ex.Message).ToAlertFormat();
				ViewUtils.ShowRefreshBtn(CoverBtn, RefreshBtn);
			}
			finally
			{
				Response.Write(responseMessage);
			}
		}

	}
}