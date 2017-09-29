using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository.Sign.Forms;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Sign.Forms
{
    //簽核頁面中的忘刷單清單
    public partial class ForgotPunchListD : System.Web.UI.Page
    {
        private ForgotPunchViewModel model = null;
        private ForgotPunchRepository _forgotRepo = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "ForgotPunchList"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            if (!IsPostBack)
            {
                _forgotRepo = RepositoryFactory.CreateForgotPunchRepo();
                //從QueryString取得 簽核代碼
                string docID = String.IsNullOrEmpty(Request["SignDocID"]) ? String.Empty : Request["SignDocID"].ToString();
                //根據查詢的 簽核代碼 搜尋忘刷單
                model = _forgotRepo.GetForgotPunchForm(docID);
                WebUtils.PageDataBind(model, this.Page);
            }
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            //'XX'型別 必須置於有 runat=server 的表單標記之中
        }

    }
}