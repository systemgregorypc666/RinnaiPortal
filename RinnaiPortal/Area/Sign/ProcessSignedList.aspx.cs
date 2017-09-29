using DBTools;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Sign
{
    public partial class ProcessSignedList : SignDataList
    {
        private RootRepository _rootRepo = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "ProcessSignedList"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }
            if (!IsPostBack)
            {
                PageTitle.Value = "主管簽核歷史紀錄 > 列表";
                var paggerParms = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
                var signListParms = WebUtils.ParseQueryString<SignListParms>(Page.Request);
                signListParms.Member = Authentication.GetMemberViewModel(User.Identity.Name);
                signListParms.GridView = ProcessSignedGridView;
                signListParms.TotalRowsCount = totalRowsCount;
                signListParms.PaginationBar = paginationBar;
                signListParms.NoDataTip = noDataTip;

                //建構頁面
                ConstructPage(signListParms, paggerParms, RepositoryFactory.CreateSignQueryRepo());

                pageSizeSelect.Text = paggerParms.PageSize.ToString();
                queryTextBox.Text = signListParms.QueryText;
            }
        }

        public string GetFormUrl(object docID, object employeeID, object formID)
        {
            _rootRepo = RepositoryFactory.CreateRootRepo();
            if (String.IsNullOrWhiteSpace((string)docID))
            {
                return "無此簽核號碼".ToAlertFormat();
            }
            else
            {
                //簽核紀錄查詢 > 列表的表單明細Url
                var pageSet = ConfigUtils.ParsePageSetting("WorkflowListSetting");
                var result = String.Format(pageSet[formID.ToString()], docID, employeeID);
                return result;
            }
        }

    }
}