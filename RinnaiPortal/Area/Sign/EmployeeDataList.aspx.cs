using DBTools;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Sign
{
    public partial class EmployeeDataList : SignDataList
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "EmployeeDataList"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            if (!IsPostBack)
            {
                PageTitle.Value = "員工基本資料 > 列表";

                // 取得 QueryString 
                var paggerParms = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
                var signListParms = WebUtils.ParseQueryString<SignListParms>(Page.Request);
                signListParms.GridView = EmployeeGridView;
                signListParms.TotalRowsCount = totalRowsCount;
                signListParms.PaginationBar = paginationBar;
                signListParms.NoDataTip = noDataTip;

                //建構頁面
                ConstructPage(signListParms, paggerParms, RepositoryFactory.CreateEmployeeRepo());

                pageSizeSelect.Text = paggerParms.PageSize.ToString();
                queryTextBox.Text = signListParms.QueryText;
            }

        }
    }
}