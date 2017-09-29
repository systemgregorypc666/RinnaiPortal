using DBTools;
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
using RinnaiPortal.FactoryMethod;
using System.Web.Script.Serialization;
using MailerAPI;

namespace RinnaiPortal.Area.Sign
{
    public partial class bbs_contentList : SignDataList
    {
        //private RootRepository _rootRepo = null;
        private BbsRepository _bbsRepo = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "bbs_contentList"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }
            if (!IsPostBack)
            {
                PageTitle.Value = "公佈欄資料 > 列表";

                // 取得 QueryString 
                var paggerParms = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
                var signListParms = WebUtils.ParseQueryString<SignListParms>(Page.Request);
                signListParms.GridView = BbsGridView;
                signListParms.TotalRowsCount = totalRowsCount;
                signListParms.PaginationBar = paginationBar;
                signListParms.NoDataTip = noDataTip;

                //建構頁面
                ConstructPage(signListParms, paggerParms, RepositoryFactory.CreateBbsRepo());

                pageSizeSelect.Text = paggerParms.PageSize.ToString();
                queryTextBox.Text = signListParms.QueryText;

            }
        }
        protected void BbsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            _bbsRepo = RepositoryFactory.CreateBbsRepo();
            string bbsId = BbsGridView.DataKeys[e.RowIndex].Values["bbs_id"].ToString();
            var responseMessage = String.Empty;
            try
            {
                if (!String.IsNullOrEmpty(bbsId))
                {
                    String filepath1 = _bbsRepo.Find_Photo(bbsId);
                    String filepath2 = _bbsRepo.Find_Url(bbsId);
                    del_file(filepath1);
                    del_file(filepath2);
                    _bbsRepo.DelData(bbsId);
                    Response.Write("<script language=javascript>alert('刪除成功!')</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script language=javascript>alert('刪除失敗!\r\n錯誤訊息: '" + ex.Message + ")</script>");
            }
            finally
            {
                Response.Write("<script language=javascript>window.location.href='bbs_contentList.aspx'</script>");
            }
        }

        // 刪除檔案
        protected void del_file(string del_url)
        {
            String savePath = Server.MapPath(@"..\..\Upload\");
            try
            {
                del_url = savePath + del_url;
                System.IO.File.Delete(del_url);
            }
            catch { }
        }

        //public string GetFormUrl(object bbsID)
        //{
        //    _rootRepo = RepositoryFactory.CreateRootRepo();
        //    if (String.IsNullOrWhiteSpace((string)bbsID.ToString()))
        //    {
        //        return "無此主題".ToAlertFormat();
        //    }
        //    else
        //    {
        //        //簽核紀錄查詢 > 列表的表單明細Url
        //        var pageSet = ConfigUtils.ParsePageSetting("bbs_Detail");
        //        var result = String.Format(pageSet[bbsID.ToString()], bbsID);
        //        return result;
        //    }
        //}
    }
}