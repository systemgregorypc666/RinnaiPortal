using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Sign.Forms
{
	public partial class TrainList03 : SignDataList
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!Authentication.HasResource(User.Identity.Name, "TrainList03"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

			if (!IsPostBack)
			{
                PageTitle.Value = "表單新增作業 > 成效評核追縱維護";
				FormSeries.Value = "Train";
				// 取得 QueryString
				var paggerParms = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
				var signListParms = WebUtils.ParseQueryString<SignListParms>(Page.Request);
				signListParms.Member = Authentication.GetMemberViewModel(User.Identity.Name);
				signListParms.GridView = TrainGridView;
				signListParms.TotalRowsCount = totalRowsCount;
				signListParms.PaginationBar = paginationBar;
				signListParms.NoDataTip = noDataTip;
				paggerParms.OrderField = "CLID";
                //調查表種類代號
                signListParms.TABLE_ID = "03";

				//建構頁面
				ConstructPage(signListParms, paggerParms, RepositoryFactory.CreateTrainRepo());

				pageSizeSelect.Text = paggerParms.PageSize.ToString();
				queryTextBox.Text = signListParms.QueryText;
			}
			else
			{
				//the second into page, IsPostBack == true
				//set ParentClassMember queryText = queryTextBox.Text
				//queryText = queryTextBox.Text;

			}
		}
	}
}