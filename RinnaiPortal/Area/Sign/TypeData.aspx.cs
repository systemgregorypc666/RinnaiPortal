using RinnaiPortal.Repository;
using RinnaiPortal.ViewModel;
using RinnaiPortal.Tools;
using RinnaiPortal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ValidationAPI;
using RinnaiPortal.FactoryMethod;

namespace RinnaiPortal.Area.Sign
{
	public partial class TypeData : System.Web.UI.Page
	{
		private TypeViewModel model = null;
		private TypeRepository _typeRepo = null;
		private RootRepository _rootRepo = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "TypeData"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			if (!IsPostBack)
			{
				_typeRepo = RepositoryFactory.CreateTypeRepo();
				_rootRepo = RepositoryFactory.CreateRootRepo();
				//從QueryString取得 表單簽核類型
				string formIDTxt = String.IsNullOrEmpty(Request["FormID"]) ? String.Empty : Request["FormID"].ToString();

				//將部門資料 與下拉式選單綁定
				ViewUtils.SetOptions(FilingDepartmentID_FK, _rootRepo.GetDepartment());
				//代理人資料 與下拉式選單綁定
				ViewUtils.SetOptions(SignID_FK, _rootRepo.GetSignProcedure());

				PageTitle.Value = "表單類型資料 > 新增";

				if (!String.IsNullOrWhiteSpace(formIDTxt))
				{
					int formID;
					if (!Int32.TryParse(formIDTxt,out formID)) { return; }
					//將 viewModel 的值綁定到 頁面上
					WebUtils.PageDataBind(_typeRepo.GetTypeData(formID), this.Page);

					FilingDepartmentName.Value = FilingDepartmentID_FK.Text;
					PageTitle.Value = "表單類型資料 > 編輯";
				}
			}
		}

		protected void FilingDepartmentID_FK_SelectedIndexChanged(object sender, EventArgs e)
		{
			FilingDepartmentName.Value = FilingDepartmentID_FK.Text;
		}

		protected void SignName_SelectedIndexChanged(object sender, EventArgs e)
		{
			SignID_FK.Text = SignID_FK.SelectedValue.ToString();
		}

		protected void SaveBtn_Click(object sender, EventArgs e)
		{
			_typeRepo = RepositoryFactory.CreateTypeRepo();
			//取得頁面資料
			model = WebUtils.ViewModelMapping<TypeViewModel>(this.Page);

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
				if (String.IsNullOrWhiteSpace(Request["FormID"]))
				{
					_typeRepo.CreateData(model);
					successRdUrl = @"TypeDataList.aspx?orderField=CreateDate&descending=True";
					responseMessage = "新增成功!";
				}
				else
				{
					//編輯資料
					_typeRepo.EditData(model);
					successRdUrl = @"TypeDataList.aspx?orderField=ModifyDate&descending=True";
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

	}
}