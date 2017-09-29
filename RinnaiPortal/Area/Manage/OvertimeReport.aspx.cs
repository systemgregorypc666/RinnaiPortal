using System;
using RinnaiPortal.Repository.Manage;
using RinnaiPortal.Tools;
using RinnaiPortal.Interface;
using RinnaiPortal.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RinnaiPortal.FactoryMethod;

namespace RinnaiPortal.Area.Manage
{
	public partial class OvertimeReport : ManageDataList
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "OvertimeReport"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}

			if (!IsPostBack)
			{
				PageTitle.Value = "加班資料 > 報表";

				// 取得 QueryString 
				var paggerParms = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
				var manageListParms = WebUtils.ParseQueryString<ManageListParms>(Page.Request);
				manageListParms.GridView = ReportGridView;
				manageListParms.TotalRowsCount = totalRowsCount;
				manageListParms.PaginationBar = paginationBar;
				manageListParms.NoDataTip = noDataTip;

				//建構頁面
				ConstructPage(manageListParms, paggerParms, RepositoryFactory.CreateOvertimeReportRepo());

				pageSizeSelect.Text = paggerParms.PageSize.ToString();
				finalStatusSelect.SelectedValue = Request.QueryString.AllKeys.Contains("finalStatus") ? Request.QueryString["finalStatus"].ToString() : "-1";
                StartDateTime.Text = Request.QueryString.AllKeys.Contains("StartDateTime") ? Request.QueryString["StartDateTime"].ToString() : manageListParms.StartDateTime.FormatDatetimeNullable();
                EndDateTime.Text = Request.QueryString.AllKeys.Contains("EndDateTime") ? Request.QueryString["EndDateTime"].ToString() : manageListParms.EndDateTime.FormatDatetimeNullable(); 
                    //manageListParms.EndDateTime.FormatDatetimeNullable();

                Session["StartDateTime"] = StartDateTime.Text;
                Session["EndDateTime"] = EndDateTime.Text;
			}
			else
			{
				//the second into page, IsPostBack == true
				//set ParentClassMember queryText = queryTextBox.Text
				//queryText = queryTextBox.Text;
			}
		}

		protected void FinalStatusSelect_SelectedIndexChanged(object sender, EventArgs e)
		{
			var finalStatusDropDownList = sender as DropDownList;
			if (finalStatusDropDownList == null) { Response.Write("頁面上並沒有提供簽核狀態元件!".ToAlertFormat()); return; }

			string queryString = WebUtils.ConstructQueryString("finalStatus", finalStatusDropDownList.SelectedValue, Request);
			string url = Request.Url.LocalPath;
			Response.Redirect(String.Concat(url, queryString));
		}
	}
}