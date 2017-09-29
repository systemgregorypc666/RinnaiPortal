using MailerAPI;
using RinnaiPortal.Extensions;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Sign
{
    public partial class WorkflowQueryList : SignDataList
    {
        private RootRepository _rootRepo = null;
        private Mailer _mailer = null;
        private NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Authentication.HasResource(User.Identity.Name, "WorkflowQueryList"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            if (!IsPostBack)
            {
                PageTitle.Value = "簽核紀錄查詢 > 列表";

                getData();
            }
        }

        public void getData()
        {
            // 取得 QueryString
            var paggerParms = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
            var signListParms = WebUtils.ParseQueryString<SignListParms>(Page.Request);
            // Page Data init
            signListParms.GridView = WorkflowQueryGridView;
            signListParms.TotalRowsCount = totalRowsCount;
            signListParms.PaginationBar = paginationBar;
            signListParms.NoDataTip = noDataTip;

            // Get Member Data
            signListParms.Member = Authentication.GetMemberViewModel(User.Identity.Name);

            //建構頁面
            ConstructPage(signListParms, paggerParms, RepositoryFactory.CreateWorkflowQueryRepo());

            pageSizeSelect.Text = paggerParms.PageSize.ToString();
            finalStatusSelect.SelectedValue = Request.QueryString.AllKeys.Contains("finalStatus") ? Request.QueryString["finalStatus"].ToString() : "-1";
            queryTextBox.Text = signListParms.QueryText;
        }

        public string GetEditUrl(object docID, object formID)
        {
            _rootRepo = RepositoryFactory.CreateRootRepo();
            if (String.IsNullOrWhiteSpace((string)docID))
            {
                return "無此簽核號碼".ToAlertFormat();
            }
            else
            {
                var profileData = Authentication.LoginList.ContainsKey(User.Identity.Name) ? Authentication.LoginList[User.Identity.Name] : null;
                var employeeID = profileData != null ? String.Join("", profileData["EmployeeID"]) : null;

                //簽核紀錄查詢 > 列表的編輯Url
                var pageSet = ConfigUtils.ParsePageSetting("WorkflowEditSetting");
                var result = String.Format(pageSet[formID.ToString()], docID, employeeID);
                return result;
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

        /// <summary>GridView.RowDataBound 事件.NET Framework (current version)  發生於資料列中的資料繫結 GridView 控制項。
        /// /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void WorkflowQueryGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //判斷是否為資料列
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.DataItem != null)
                {
                    //Cell中對每一個Controls去搜尋
                    foreach (Control cmdControl in ((DataControlFieldCell)e.Row.Cells[10]).Controls)
                    {
                        //確認是否是LinkButton (如果使用的是Button or ImageButton 記得要修改)
                        if ((cmdControl) is LinkButton)
                        {
                            LinkButton delButton = (LinkButton)cmdControl.FindControl("Del");
                            LinkButton pumpButton = (LinkButton)cmdControl.FindControl("Pump");
                            HyperLink editButton = (HyperLink)cmdControl.FindControl("Edit");

                            //利用CommandName的不同去分別每一種按鈕，才能去做之後的變化，
                            //在此是直接將"編輯"的按鈕，全部都隱藏起來 <=編輯是在前端判斷 jQuery
                            string status = DataBinder.Eval(e.Row.DataItem, "FinalStatus").ToString();


                            //編輯 ，若狀態為 1.草稿 4.駁回 就顯示

                            if (status == "1" || status == "4")
                            {
                                if (editButton != null)
                                    editButton.Visible = true;
                            }
                            else
                            {
                                if (editButton != null)
                                    editButton.Visible = false;
                            }

                            //刪除 ，若狀態為 2.待簽核 3.核准  5.取消 6.結案 7.歸檔 就隱藏 ，為 1.草稿 4.駁回 就顯示

                            if (status != "1" && status != "4")
                            {
                                if (delButton != null)
                                    delButton.Visible = false;
                            }
                            else
                            {
                                if (delButton != null)
                                    delButton.Visible = true;
                            }
                            //抽回 ，若狀態為 2.待簽核  就顯示
                            if (status == "2")
                            {
                                if (pumpButton != null)
                                    pumpButton.Visible = true;
                            }
                            else
                            {
                                if (pumpButton != null)
                                    pumpButton.Visible = false;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// GridView.RowDeleting 事件.NET Framework (current version)  發生於按一下的資料列的 [刪除] 按鈕前, GridView 控制項刪除資料列。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void WorkflowQueryGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            _rootRepo = RepositoryFactory.CreateRootRepo();
            string DocId = WorkflowQueryGridView.DataKeys[e.RowIndex].Values["SignDocID"].ToString();

            var responseMessage = String.Empty;
            try
            {
                if (!String.IsNullOrEmpty(DocId))
                {
                    _rootRepo.DelData(DocId);
                    Response.Write("<script language=javascript>alert('刪除成功!')</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script language=javascript>alert('刪除失敗!\r\n錯誤訊息: '" + ex.Message + ")</script>");
            }
            finally
            {
                Response.Write("<script language=javascript>window.location.href='WorkflowQueryList.aspx'</script>");
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

        /// <summary>
        /// GridView.RowCommand 事件 .NET Framework (current version)  發生於在按一下按鈕時 GridView 控制項。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void WorkflowQueryGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            _rootRepo = RepositoryFactory.CreateRootRepo();
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            string DocId = WorkflowQueryGridView.DataKeys[row.RowIndex].Values["SignDocID"].ToString();
            string FormID = WorkflowQueryGridView.DataKeys[row.RowIndex].Values["FormID_FK"].ToString();
            string employeeID = WorkflowQueryGridView.DataKeys[row.RowIndex].Values["EmployeeID_FK"].ToString();
            string UserName = User.Identity.Name.ToString();
            string CurrentSignLevelDeptID = WorkflowQueryGridView.DataKeys[row.RowIndex].Values["CurrentSignLevelDeptID_FK"].ToString();
            string Remainder = WorkflowQueryGridView.DataKeys[row.RowIndex].Values["Remainder"].ToString();
            string ChiefID = WorkflowQueryGridView.DataKeys[row.RowIndex].Values["ChiefID_Up"].ToString();
            if (e.CommandName == "Pump")
            {
                //Response.Write("<script language=javascript>alert('" + UserName + "')</script>");
                var responseMessage = String.Empty;
                try
                {
                    MailInfo info = new MailInfo()
                    {
                        AddresseeTemp = System.Web.Configuration.WebConfigurationManager.AppSettings["MailTemplate"],
                        DomainPattern = ConfigUtils.ParsePageSetting("Pattern")["DomainPattern"],
                    };
                    _rootRepo.PumpData(DocId, FormID, employeeID, UserName, CurrentSignLevelDeptID, Remainder);
                    info.To = (string)_rootRepo.QueryForEmployeeByEmpID(ChiefID)["ADAccount"];

                    info.Subject = String.Format("系統提醒!簽核單號 : {0} 已經由填單者抽單!", DocId);
                    info.Body.AppendFormat("{0}{1}", info.Subject, "此件為系統發送，請勿回覆!");

                    //mail
                    _mailer = new Mailer(info);
                    if (PublicRepository.CurrentWorkflowMode == Enums.WorkflowTypeEnum.RELEASE)
                    _mailer.SendMail();
                    //log
                    var cc = String.Join(",", info.To);
                    _log.Trace(String.Format("MailTo : {0}\r\ncc : {1}\r\nTitle : {2}\r\nContent : {3}\r\n", info.To, cc, info.Subject, info.Body));

                    Response.Write("<script language=javascript>alert('成功抽單!')</script>");
                }
                catch (Exception ex)
                {
                    Response.Write("<script language=javascript>alert('抽單失敗!\r\n錯誤訊息: '" + ex.Message + ")</script>");
                }
                finally
                {
                    Response.Write("<script language=javascript>window.location.href='WorkflowQueryList.aspx'</script>");
                }
            }
        }
    }
}