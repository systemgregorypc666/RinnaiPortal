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

namespace RinnaiPortal
{
    public partial class bbs_content : System.Web.UI.Page
    {
        private BbsViewModel model = null;
        private BbsRepository _bbsRepo = null;
        private RootRepository _rootRepo = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "bbs_contentList"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }
            if (!IsPostBack)
            {
                _bbsRepo = RepositoryFactory.CreateBbsRepo();
                _rootRepo = RepositoryFactory.CreateRootRepo();
                //從QueryString取得 公佈欄代碼
                string bbsID = String.IsNullOrEmpty(Request["BbsID"]) ? String.Empty : Request["BbsID"].ToString();

                PageTitle.Value = "公佈欄 > 新增";

                if (!String.IsNullOrWhiteSpace(bbsID))
                {
                    model = _bbsRepo.GetBbs(bbsID);
                    PageTitle.Value = "公佈欄 > 編輯";

                    if (model.PhotoName != "")
                    {
                        HyperLink_FILENAME1.Visible = true;
                        HyperLink_FILENAME1.NavigateUrl = @"..\..\Upload\" + model.PhotoName;
                        Button_DelFILE1.Visible = true;
                        File_Photo.Visible = false;
                    }
                    else
                    {
                        HyperLink_FILENAME1.Visible = false;
                        Button_DelFILE1.Visible = false;
                        File_Photo.Visible = true;
                    }

                    if (model.UpName != "")
                    {
                        HyperLink_FILENAME2.Visible = true;
                        HyperLink_FILENAME2.NavigateUrl = @"..\..\Upload\" + model.UpName;
                        Button_DelFILE2.Visible = true;
                        File_Up1.Visible = false;
                    }
                    else
                    {
                        HyperLink_FILENAME2.Visible = false;
                        Button_DelFILE2.Visible = false;
                        File_Up1.Visible = true;
                    }

                  WebUtils.PageDataBind(_bbsRepo.GetBbsData(bbsID), this.Page);
                }
            }
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            _bbsRepo = RepositoryFactory.CreateBbsRepo();
            //取得頁面資料
            model = WebUtils.ViewModelMapping<BbsViewModel>(this.Page);
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
            //if (!Check_Null())
            //{
            //    return;
            //}
            try
            {
                if (String.IsNullOrWhiteSpace(Request["BbsID"]))
                {
                    BatchFileUpload1(File_Photo);
                    BatchFileUpload2(File_Up1);
                    _bbsRepo.CreateData(model);
                    successRdUrl = @"bbs_contentList.aspx?orderField=CreateDate&descending=True";
                    responseMessage = "新增成功!";
                }
                else
                {
                    string filepath1 = string.Empty;
                    if (File_Photo.Visible == false && Button_DelFILE1.Visible == true)
                    {
                        filepath1 = _bbsRepo.Find_Photo(model.bbs_id.ToString());
                        
                    }
                    else if (!string.IsNullOrEmpty(File_Photo.PostedFile.FileName))
                    {
                        BatchFileUpload1(File_Photo);
                        filepath1 = File_Photo.FileName;
                    }
                    model.PhotoName = filepath1;

                    string filepath2 = string.Empty;
                    if (File_Up1.Visible == false && Button_DelFILE2.Visible == true)
                    {
                        filepath2 = _bbsRepo.Find_Url(model.bbs_id.ToString());

                    }
                    else if (!string.IsNullOrEmpty(File_Up1.PostedFile.FileName))
                    {
                        BatchFileUpload2(File_Up1);
                        filepath2 = File_Up1.FileName;
                    }
                    model.UpName = filepath2;

                    _bbsRepo.EditData(model);
                    successRdUrl = @"bbs_contentList.aspx?orderField=ModifyDate&descending=True";
                    responseMessage = "編輯成功!";
                }
                //btn處理
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

        protected bool Check_Null()
        {
            string strErr = "";

            if (txt_Title.Text == "")
                strErr += "主題、";
            if (DefaultStartDateTime.Text == "")
                strErr += "張貼起始日、";
            if (DefaultEndDateTime.Text == "")
                strErr += "張貼結束日、";
            if (txt_Content.Text == "")
                strErr += "公告內容、";

            if (strErr != "")
            {
                strErr = strErr.Substring(0, strErr.Length - 1);
                ScriptManager.RegisterClientScriptBlock(this, typeof(string), "click", "alert('" + strErr + " 不可為空白！')", true);

                return false;
            }

            return true;
        }

        protected void BatchFileUpload1(FileUpload myFL1)
        {
            //model = WebUtils.ViewModelMapping<BbsViewModel>(this.Page);
            String savePath = Server.MapPath(@"..\..\Upload\");
            if (myFL1.HasFile)
            {
                int tFileLength = myFL1.PostedFile.ContentLength;
                //if (tFileLength <= 2048000)  //限制檔案大小
                //{
                String fileName1 = myFL1.FileName;
                savePath = savePath + fileName1;
                myFL1.SaveAs(savePath);
                //Label1.Text += "上傳成功，檔名---- " + fileName;
                //}
                //else
                //{
                //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "click", "alert('" + myFL.FileName + "檔案大小超過2Mb！');", true);
                //    return;
                //}
                
                model.PhotoName = fileName1;
                
            }

        }

        protected void BatchFileUpload2(FileUpload myFL2)
        {
            //model2 = WebUtils.ViewModelMapping<BbsViewModel>(this.Page);
            String savePath = Server.MapPath(@"..\..\Upload\");
            if (myFL2.HasFile)
            {
                int tFileLength = myFL2.PostedFile.ContentLength;
                //if (tFileLength <= 2048000)  //限制檔案大小
                //{
                String fileName2 = myFL2.FileName;
                savePath = savePath + fileName2;
                myFL2.SaveAs(savePath);
                //Label1.Text += "上傳成功，檔名---- " + fileName;
                //}
                //else
                //{
                //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "click", "alert('" + myFL.FileName + "檔案大小超過2Mb！');", true);
                //    return;
                //}

                model.UpName = fileName2;
                
            }

        }

        protected void Button_DelFILE1_Command(object sender, CommandEventArgs e)
        {
            _bbsRepo = RepositoryFactory.CreateBbsRepo();
            //取得頁面資料
            model = WebUtils.ViewModelMapping<BbsViewModel>(this.Page);
            String filepath1 = _bbsRepo.Find_Photo(model.bbs_id.ToString());
            del_file(filepath1);
            File_Photo.Visible = true;
            Button_DelFILE1.Visible = false;
            HyperLink_FILENAME1.Visible = false;
        }

        protected void Button_DelFILE2_Command(object sender, CommandEventArgs e)
        {
            _bbsRepo = RepositoryFactory.CreateBbsRepo();
            //取得頁面資料
            model = WebUtils.ViewModelMapping<BbsViewModel>(this.Page);
            String filepath2 = _bbsRepo.Find_Url(model.bbs_id.ToString());
            del_file(filepath2);
            File_Up1.Visible = true;
            Button_DelFILE2.Visible = false;
            HyperLink_FILENAME2.Visible = false;
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
    }
}