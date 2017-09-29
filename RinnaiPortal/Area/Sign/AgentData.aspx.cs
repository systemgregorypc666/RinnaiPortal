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
	public partial class AgentData : System.Web.UI.Page
	{
		private AgentViewModel model = null;
		private AgentRepository _agentRepo = null;
		private RootRepository _rootRepo = null;
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "AgentData"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			if (!IsPostBack)
			{
				_agentRepo = RepositoryFactory.CreateAgentRepo();
				_rootRepo = RepositoryFactory.CreateRootRepo();
				//從QueryString取得 部門代碼
				string sn = String.IsNullOrEmpty(Request["sn"]) ? String.Empty : Request["sn"].ToString();

				//將員工資料 與下拉式選單綁定
				ViewUtils.SetOptions(EmployeeID_FK, _rootRepo.GetEmployee());

				PageTitle.Value = "代理簽核資料 > 新增";

				if (!String.IsNullOrWhiteSpace(sn))
				{
					//將 viewModel 的值綁定到 頁面上
					WebUtils.PageDataBind(_agentRepo.GetAgentData(sn), this.Page);
					EmployeeName.Value = EmployeeID_FK.Text;

					PageTitle.Value = "代理簽核資料 > 編輯";
				}
			}
		}

		protected void EmployeeID_FK_SelectedIndexChanged(object sender, EventArgs e)
		{
			EmployeeName.Value = EmployeeID_FK.Text;
		}

		protected void SaveBtn_Click(object sender, EventArgs e)
		{
			_agentRepo = RepositoryFactory.CreateAgentRepo();
			//取得頁面資料
			model = WebUtils.ViewModelMapping<AgentViewModel>(this.Page);

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
				if (model.SN <= 0)
				{
					_agentRepo.CreateData(model);
					successRdUrl = @"AgentDataList.aspx?orderField=CreateDate&descending=True";
					responseMessage = "新增成功!";
				}
				else
				{
					model.SN = Int32.Parse(Request["sn"].ToString());
					_agentRepo.EditData(model);
					successRdUrl = @"AgentDataList.aspx?orderField=ModifyDate&descending=True";
					responseMessage = "編輯成功!";
				}
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