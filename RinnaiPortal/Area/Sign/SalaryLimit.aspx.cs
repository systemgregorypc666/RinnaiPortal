using RinnaiPortal.Extensions;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository.Sign;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ValidationAPI;

namespace RinnaiPortal.Area.Sign
{
	public partial class SalaryLimit : System.Web.UI.Page
	{
		private SalaryLimitViewModel model = null;
		private SalaryLimitRepository _salaryRepo = null;
		private RootRepository _rootRepo = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "SalaryLimit"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			if (!IsPostBack)
			{
				_salaryRepo = RepositoryFactory.CreateSalaryLimitRepo();
				_rootRepo = RepositoryFactory.CreateRootRepo();
				//從QueryString取得 部門代碼

				PageTitle.Value = "基本設定 > 結薪日資料維護";

				var data = _salaryRepo.GetLimitData("1");
				if (data != null)
				{
					//將 viewModel 的值綁定到 頁面上
					WebUtils.PageDataBind(data, this.Page);
				}
			}
		}

		protected void SaveBtn_Click(object sender, EventArgs e)
		{
			_salaryRepo = RepositoryFactory.CreateSalaryLimitRepo();
			//取得頁面資料
			model = WebUtils.ViewModelMapping<SalaryLimitViewModel>(this.Page);

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
			string result = "存檔成功!";
			try
			{
				_salaryRepo.SaveData(model);
				ViewUtils.ButtonOn(SaveBtn, CoverBtn);
				Response.ShowMessageAndRefresh(result.ToAlertFormat());
			}
			catch (Exception ex)
			{
				result = String.Concat("存檔失敗!\r\n錯誤訊息: ", ex.Message);
				Response.Write(result.ToAlertFormat());
				ViewUtils.ShowRefreshBtn(CoverBtn, RefreshBtn);
			}


		}
	}
}