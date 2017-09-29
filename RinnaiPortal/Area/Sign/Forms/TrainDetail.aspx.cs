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
	public partial class TrainDetail : SignDataList
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Authentication.HasResource(User.Identity.Name, "TrainDetail"))
			{
				Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
			}
			if (!IsPostBack)
			{
				// 取得 QueryString 
				var paggerParms = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
				var signListParms = WebUtils.ParseQueryString<SignListParms>(Page.Request);
				signListParms.Member = Authentication.GetMemberViewModel(User.Identity.Name);
				signListParms.GridView = TrainDetailGridView;
				signListParms.PaginationBar = paginationBar;
				paggerParms.OrderField = "qno";


				//建構頁面
				ConstructPage(signListParms, paggerParms, RepositoryFactory.CreateTrainDetailRepo());
			}

		}

		public override void VerifyRenderingInServerForm(Control control)
		{
			//處理'GridView' 的控制項 'GridView' 必須置於有 runat=server 的表單標記之中
		}
	}
}