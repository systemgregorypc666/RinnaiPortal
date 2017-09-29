using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository.Sign;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Sign
{
    public partial class GroupDataList : SignDataList
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "GroupDataList"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            if (!IsPostBack)
            {
                PageTitle.Value = "群組資料 > 列表";

                // 取得 QueryString 
                var paggerParms = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
                var signListParms = WebUtils.ParseQueryString<SignListParms>(Page.Request);
                signListParms.GridView = GroupGridView;
                signListParms.TotalRowsCount = totalRowsCount;
                signListParms.PaginationBar = paginationBar;
                signListParms.NoDataTip = noDataTip;

                //建構頁面
                ConstructPage(signListParms, paggerParms, RepositoryFactory.CreateGroupRepo());

                pageSizeSelect.Text = paggerParms.PageSize.ToString();
                queryTextBox.Text = signListParms.QueryText;
            }

        }
    }
}