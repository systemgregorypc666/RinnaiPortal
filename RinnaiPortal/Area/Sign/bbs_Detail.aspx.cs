using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal
{
    public partial class bbs_Detail : System.Web.UI.Page
    {
        private BbsViewModel model = null;
        private BbsRepository _bbsRepo = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PageTitle.Value = "公佈欄資料 > 公佈的內容";
                _bbsRepo = RepositoryFactory.CreateBbsRepo();
                //從QueryString取得 簽核代碼
                string bbsID = String.IsNullOrEmpty(Request["bbs_id"]) ? String.Empty : Request["bbs_id"].ToString();
                //根據查詢的SN 搜尋公告
                model = _bbsRepo.GetBbs(bbsID);
                if (model.PhotoName.ToString() !="")
                {
                    img_Photo.Visible = true;
                    img_Photo.ImageUrl = @"..\..\Upload\" + model.PhotoName;
                }
                else
                {
                    img_Photo.Visible = false;
                }
                if (model.txt_Http.ToString() != "http://" && model.txt_Http.ToString() != "")
                {
                    hyl_Http.Visible = true;
                    hyl_Http.Text = model.txt_Http;
                    hyl_Http.NavigateUrl = model.txt_Http;
                }
                else
                {
                    hyl_Http.Visible = false;
                }

                if (model.UpName.ToString() != "")
                {
                    hyl_Url.Visible = true;
                    hyl_Url.Text = model.UpName;
                    hyl_Url.NavigateUrl = @"..\..\Upload\" + model.UpName;
                }
                else
                {
                    hyl_Url.Visible = false;
                }
               
                lbl_Creator.Text = model.Creator;
                WebUtils.PageDataBind(model, this.Page);
            }
        }
    }
}