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
	public partial class GroupData : System.Web.UI.Page
	{
		private GroupViewModel model = null;
		private GroupRepository _groupRepo = null;
		private RootRepository _rootRepo = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "GroupData"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			if (!IsPostBack)
			{
				_groupRepo = RepositoryFactory.CreateGroupRepo();
				_rootRepo = RepositoryFactory.CreateRootRepo();
				//從QueryString取得 群組代碼
				string groupType = String.IsNullOrEmpty(Request["GroupType"]) ? String.Empty : Request["GroupType"].ToString();

				//存取資源  與下拉式選單綁定
				ViewUtils.SetOptions(Resource, _rootRepo.GetSystemArchitecture());

				PageTitle.Value = "群組資料 > 新增";

				if (!String.IsNullOrWhiteSpace(groupType))
				{
					//將 viewModel 的值綁定到 頁面上
					WebUtils.PageDataBind(_groupRepo.GetGroupData(groupType), this.Page);

					GroupType.ReadOnly = true;
					PageTitle.Value = "群組資料 > 編輯";

				}
			}
		}
		protected void SaveBtn_Click(object sender, EventArgs e)
		{
			_groupRepo = RepositoryFactory.CreateGroupRepo();
			//取得頁面資料
			model = WebUtils.ViewModelMapping<GroupViewModel>(this.Page);

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
				if (String.IsNullOrWhiteSpace(Request["GroupType"]))
				{
					_groupRepo.CreateData(model);
					successRdUrl = @"GroupDataList.aspx?orderField=CreateDate&descending=True";
					responseMessage = "新增成功!";
				}
				else
				{
					_groupRepo.EditData(model);
					successRdUrl = @"GroupDataList.aspx?orderField=ModifyDate&descending=True";
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