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
	public partial class ProcedureData : System.Web.UI.Page
	{
		private ProcedureViewModel model = null;
		private ProcedureRepository _procedureRepo = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "ProcedureData"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			if (!IsPostBack)
			{
				_procedureRepo = RepositoryFactory.CreateProcedureRepo();
				//從QueryString取得 簽核代碼
				string signID = String.IsNullOrEmpty(Request["SignID"]) ? String.Empty : Request["SignID"].ToString();

				PageTitle.Value = "組織階層資料 > 新增";
				if (!String.IsNullOrWhiteSpace(signID))
				{
					//將 viewModel 的值綁定到 頁面上
					WebUtils.PageDataBind(_procedureRepo.GetProcedureData(signID), this.Page);

					SignID.ReadOnly = true;
					PageTitle.Value = "組織階層資料 > 編輯";
				}
			}
		}

		protected void SaveBtn_Click(object sender, EventArgs e)
		{
			_procedureRepo = RepositoryFactory.CreateProcedureRepo();
			//取得頁面資料
			model = WebUtils.ViewModelMapping<ProcedureViewModel>(this.Page);

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
				if (String.IsNullOrWhiteSpace(Request["SignID"]))
				{
					_procedureRepo.CreateData(model);
					successRdUrl = @"ProcedureDataList.aspx?orderField=CreateDate&descending=True";
					responseMessage = "新增成功!";
				}
				else
				{
					_procedureRepo.EditData(model);
					successRdUrl = @"ProcedureDataList.aspx?orderField=ModifyDate&descending=True";
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

		protected void Disabled_CheckedChanged(object sender, EventArgs e)
		{
			DisabledDate.Text = Disabled.Checked ? DateTime.Now.FormatDatetime("yyyy-MM-dd HH:mm:00") : String.Empty;
		}


	}
}