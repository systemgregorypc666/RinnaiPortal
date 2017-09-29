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

namespace RinnaiPortal.Area.Sign
{
	public partial class DepartmentData : System.Web.UI.Page
	{
		private DepartmentViewModel model = null;
		private DepartmentRepository _departmentRepo = null;
		private RootRepository _rootRepo = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "DepartmentData"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			if (!IsPostBack)
			{
				_departmentRepo = RepositoryFactory.CreateDepartmentRepo();
				_rootRepo = RepositoryFactory.CreateRootRepo();
				//從QueryString取得 部門代碼
				string deptID = String.IsNullOrEmpty(Request["DepartmentID"]) ? String.Empty : Request["DepartmentID"].ToString();

				//將主管資料 與下拉式選單綁定
				ViewUtils.SetOptions(ChiefID_FK, _rootRepo.GetEmployee());
				//部門資料 與下拉式選單綁定
				ViewUtils.SetOptions(UpperDepartmentID, _rootRepo.GetDepartment());
				//將員工資料 與下拉式選單綁定
				ViewUtils.SetOptions(FilingEmployeeID_FK, _rootRepo.GetEmployee());

				PageTitle.Value = "部門基本資料 > 新增";

				if (!String.IsNullOrWhiteSpace(deptID))
				{
					//將 viewModel 的值綁定到 頁面上
					WebUtils.PageDataBind(_departmentRepo.GetDepartmentData(deptID), this.Page);

					DepartmentID.ReadOnly = true;
					ChiefName.Value = ChiefID_FK.Text;
					UpperDepartmentName.Value = UpperDepartmentID.Text; ;
					FilingEmployeeName.Value = FilingEmployeeID_FK.Text;
					PageTitle.Value = "部門基本資料 > 編輯";

				}
			}
		}

		protected void SaveBtn_Click(object sender, EventArgs e)
		{
			_departmentRepo = RepositoryFactory.CreateDepartmentRepo();
			//取得頁面資料
			model = WebUtils.ViewModelMapping<DepartmentViewModel>(this.Page);

			var validator = new Validator();
			var validResult = validator.ValidateModel(model);
			if (!validResult.IsValid)
			{
				Response.Write(validResult.ErrorMessage.ToString().ToAlertFormat());
				return;
			}

			//btn處理
			ViewUtils.ButtonOff(SaveBtn, CoverBtn);

			//存檔
			var responseMessage = "";
			var successRdUrl = String.Empty;

			try
			{
				if (String.IsNullOrWhiteSpace(Request["DepartmentID"]))
				{
					_departmentRepo.CreateData(model);
					successRdUrl = @"DepartmentDataList.aspx?orderField=CreateDate&descending=True";
					responseMessage = "新增成功!";
				}
				else
				{
					_departmentRepo.EditData(model);
					successRdUrl = @"DepartmentDataList.aspx?orderField=ModifyDate&descending=True";
					responseMessage = "編輯成功!";
				}

				//btn處理
				ViewUtils.ButtonOn(SaveBtn, CoverBtn);
				responseMessage = responseMessage.ToAlertAndRedirect(successRdUrl);
			}
			catch (Exception ex)
			{
				responseMessage = String.Concat("存檔失敗!\r\n錯誤訊息: ", ex.Message).ToAlertFormat();
				ViewUtils.ShowRefreshBtn(CoverBtn, RefreshBtn);
			}
			finally
			{
				Response.Write(responseMessage);
			}
		}


		protected void ChiefID_FK_SelectedIndexChanged(object sender, EventArgs e)
		{
			ChiefName.Value = ChiefID_FK.Text;
		}

		protected void UpperDepartmentID_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpperDepartmentName.Value = UpperDepartmentID.Text;
		}

		protected void FilingEmployeeID_FK_SelectedIndexChanged(object sender, EventArgs e)
		{
			FilingEmployeeName.Value = FilingEmployeeID_FK.Text;
		}

		protected void Disabled_CheckedChanged(object sender, EventArgs e)
		{
			DisabledDate.Text = Disabled.Checked ? DateTime.Now.FormatDatetime() : String.Empty;
		}

	}
}