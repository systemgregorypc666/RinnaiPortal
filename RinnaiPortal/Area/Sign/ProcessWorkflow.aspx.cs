using MailerAPI;
using NLog;
using RinnaiPortal.Area.Sign.Handlers;
using RinnaiPortal.Extensions;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository;
using RinnaiPortal.Repository.Sign.Forms;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Sign
{
    /// <summary>
    /// 主管簽核頁面
    /// </summary>
    public partial class ProcessWorkflow : System.Web.UI.Page
    {
        private ProcessWorkflowRepository _processwfRepo = null;
        private RootRepository _rootRepo = null;
        private NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private Mailer _mailer = null;
        private TrainDetailRepository _trainDetaiRepo = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            _trainDetaiRepo = RepositoryFactory.CreateTrainDetailRepo();
            if (!Authentication.HasResource(User.Identity.Name, "ProcessWorkflowList"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            if (!IsPostBack)
            {
                _processwfRepo = RepositoryFactory.CreateProcessWorkflowRepo();
                _rootRepo = RepositoryFactory.CreateRootRepo();

                //從QueryString取得 簽核文件代碼
                if (String.IsNullOrWhiteSpace(Request["SignDocID"]))
                {
                    Response.Write("需要簽核文件代碼!".ToAlertFormat());
                    return;
                }

                ProcessWorkflowViewModel model = _processwfRepo.GetWorkflowDataAndCheck(Request["SignDocID"], User.Identity.Name);
                if (model == null)
                {
                    Response.Write("查無簽核流程資料!".ToAlertFormat());
                    return;
                }

                //將 viewModel 的值綁定到 頁面上
                WebUtils.PageDataBind(model, this.Page);
                SignDocID_FK.Value = model.SignDocID;

                FormContent1.Attributes.Add("Src", ConstructAspxPage(model.SignDocID, model.FormID_FK));
            }
            PageInit();
        }

        /// <summary>
        /// page 初始化設定
        /// </summary>
        protected void PageInit()
        {
            //送簽文件ID
            string signID = Request["SignDocID"];
            if (String.IsNullOrWhiteSpace(signID))
            {
                Response.Write("需要簽核文件代碼!".ToAlertFormat());
                return;
            }

            //課程代碼
            string strCLID = _trainDetaiRepo.Find_CLID(signID);
            //填單員工號
            string strSID = _trainDetaiRepo.Find_SID(signID);

            if (!String.IsNullOrEmpty(strCLID) && !String.IsNullOrEmpty(strSID))
            {
                txterror.Text = "";
                PlaceHolder1.Controls.Clear();
                var QuestionDataList = _trainDetaiRepo.QueryChiefDataBySignDocID(Request["SignDocID"]);
                QuestionDataList.All(row =>
                {
                    // PlaceHolder1.Controls.Add(new LiteralControl("<hr style='margin:6px;height:1px;border:0px;background-color:#D5D5D5;color:#D5D5D5;'/>"));
                    PlaceHolder1.Controls.Add(new LiteralControl("&nbsp;&nbsp"));
                    //序號
                    Label myLabel = new Label();
                    myLabel.Text = "";
                    myLabel.Text = row["serial_no"].ToString();
                    myLabel.ID = "Label" + row["serial_no"].ToString();
                    //myLabel.BackColor = "#FFFFFF";
                    myLabel.Width = 10;
                    myLabel.CssClass = "col-sm-1 control-label";
                    PlaceHolder1.Controls.Add(myLabel);
                    // PlaceHolder1.Controls.Add(new LiteralControl("&nbsp;&nbsp;&nbsp;&nbsp"));

                    //題目
                    Label mylblName = new Label();
                    mylblName.Text = "";
                    mylblName.Text = row["CODENAME"].ToString();
                    mylblName.ID = "LabelName" + row["serial_no"].ToString();
                    mylblName.Width = 300;
                    mylblName.CssClass = "col-sm-1 control-label";
                    PlaceHolder1.Controls.Add(mylblName);
                    //PlaceHolder1.Controls.Add(new LiteralControl("&nbsp;&nbsp;&nbsp;&nbsp"));
                    //答案
                    if (row["ANSTYPE"].ToString() == "C")//填文字
                    {
                        TextBox txtAns = new TextBox();
                        txtAns.Text = "";
                        txtAns.ID = "textbox" + row["serial_no"].ToString();
                        //txtAns.TabIndex = (i + 1).ToString();
                        txtAns.TextMode = TextBoxMode.MultiLine;
                        txtAns.Text = row["ANS"].ToString();
                        txtAns.Width = 500;
                        txtAns.Height = 100;
                        PlaceHolder1.Controls.Add(txtAns);
                    }
                    else if (row["ANSTYPE"].ToString() == "N")//選分數
                    {
                        RadioButtonList rblAns = new RadioButtonList();
                        rblAns.ID = "rbl" + row["serial_no"].ToString();
                        for (int j = 10; j >= 1; j--)
                        {
                            rblAns.Items.Add(new ListItem("&nbsp;&nbsp" + j.ToString() + "&nbsp;&nbsp", j.ToString()));
                        }
                        rblAns.RepeatColumns = 10;
                        rblAns.RepeatLayout = RepeatLayout.Flow;
                        rblAns.RepeatDirection = RepeatDirection.Vertical;
                        rblAns.Width = 650;
                        //rblAns.Height = 80;

                        PlaceHolder1.Controls.Add(rblAns);
                        //txtAns.Width = 50;
                    }

                    //題號
                    Label QNOLabel = new Label();
                    QNOLabel.Text = "";
                    QNOLabel.Text = row["QNO"].ToString();
                    QNOLabel.ID = "QNOLabel" + row["serial_no"].ToString();
                    QNOLabel.Visible = false;
                    PlaceHolder1.Controls.Add(QNOLabel);
                    //題型
                    Label ANSTYPELabel = new Label();
                    ANSTYPELabel.Text = "";
                    ANSTYPELabel.Text = row["ANSTYPE"].ToString();
                    ANSTYPELabel.ID = "ANSTYPELabel" + row["serial_no"].ToString();
                    ANSTYPELabel.Visible = false;
                    PlaceHolder1.Controls.Add(ANSTYPELabel);
                    return true;
                });
            }
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            _processwfRepo = RepositoryFactory.CreateProcessWorkflowRepo();
            _rootRepo = RepositoryFactory.CreateRootRepo();

            //取得頁面資料

            //簽核擔當
            ProcessWorkflowDetailViewModel model = PageDataBind();
            ProcessWorkflowViewModel model_M = _processwfRepo.GetWorkflowDataAndCheck(Request["SignDocID"], User.Identity.Name);

            //btn處理
            ViewUtils.ButtonOff(SaveBtn, CoverBtn);

            var sectionChief = new SectionChief(null);

            #region #0012 忘刷單的時間如果不符合邏輯，在確認簽核的時候主管要看到明顯提示 (目前針對桃園所做設定)

            //判斷是否為忘刷單
            if (model_M.FormID_FK == 1)
            {
                //判斷登入者是否為桃園所的員工
                EmployeeRepository employeeRepository = RepositoryFactory.CreateEmployeeRepo();
                EmployeeViewModel empInfo = employeeRepository.GetEmployeeDataByADAccount(Context.User.Identity.Name);
                if (empInfo.DepartmentID_FK == "3910")
                {
                    bool isInRule = _processwfRepo.CheckFogotPunchTimeHasInRule(Request["SignDocID"]);
                    if (!isInRule)
                    {
                        ClientScriptManager cs = Page.ClientScript;
                        cs.RegisterClientScriptBlock(this.GetType(), "PopupScript", "var fpTimeIsSuccess = false;", true);
                        return;
                    }
                }
            }

            #endregion #0012 忘刷單的時間如果不符合邏輯，在確認簽核的時候主管要看到明顯提示 (目前針對桃園所做設定)

            try
            {
                if (model_M.FormID_FK == 3 && model.Status == 3)
                {
                    //txterror.Text = "123";
                    //Response.Write("未填寫單位主管意見".ToAlertFormat());
                    _trainDetaiRepo = RepositoryFactory.CreateTrainDetailRepo();
                    //取得頁面資料

                    model = PageDataBind();
                    var QuestionDataList = _trainDetaiRepo.QueryQuestionDataByChief("02");
                    String chkString = "";
                    String ansString = "";
                    QuestionDataList.All(row =>
                   {
                       if (row["ANSTYPE"].ToString() == "C")//填文字
                       {
                           TextBox tmpbox = PlaceHolder1.FindControl("textbox" + row["serial_no"].ToString()) as TextBox;
                           ansString = tmpbox.Text;
                           if (ansString == "")
                           {
                               chkString += row["CODENAME"].ToString() + " 不可空白!";
                           }
                       }
                       else if (row["ANSTYPE"].ToString() == "N")
                       {
                           //RadioButtonList tmprbl = PlaceHolder1.FindControl("rbl" + row["serial_no"].ToString()) as RadioButtonList;
                           RadioButtonList tmprbl = (RadioButtonList)PlaceHolder1.FindControl("rbl" + row["serial_no"].ToString());
                           ansString = tmprbl.SelectedValue;
                           if (ansString == "")
                           {
                               chkString += row["CODENAME"].ToString() + " 不可空白!";
                           }
                       }
                       return true;
                   });
                    if (chkString != "")
                    {
                        Response.Write(chkString.ToAlertFormat());
                        return;
                    }
                    else
                    {
                        //儲存主管意見
                        var successRdUrl = String.Empty;
                        string strCLID = _trainDetaiRepo.Find_CLID(Request["SignDocID"]);
                        string strSID = _trainDetaiRepo.Find_SID(Request["SignDocID"]);
                        //刪除記錄調查表的主管意見
                        _trainDetaiRepo.DelChiefAnswer("CHARACTER_ANSWER", strCLID, strSID, "02");
                        _trainDetaiRepo.DelChiefAnswer("NUMERIC_ANSWER", strCLID, strSID, "02");
                        QuestionDataList.All(row =>
                       {
                           if (row["ANSTYPE"].ToString() == "C")//填文字
                           {
                               //新增
                               TextBox tmpbox = PlaceHolder1.FindControl("textbox" + row["serial_no"].ToString()) as TextBox;
                               //ansString = tmpbox.Text;
                               _trainDetaiRepo.AddAnswer_C(strCLID, strSID, row["TABLE_ID"].ToString(), row["QNO"].ToString(), row["serial_no"].ToString(), tmpbox.Text, "Portal");
                           }
                           else if (row["ANSTYPE"].ToString() == "N")
                           {
                               //新增
                               RadioButtonList tmprbl = (RadioButtonList)PlaceHolder1.FindControl("rbl" + row["serial_no"].ToString());
                               //ansString = tmprbl.SelectedValue;
                               _trainDetaiRepo.AddAnswer_N(strCLID, strSID, row["TABLE_ID"].ToString(), row["QNO"].ToString(), row["serial_no"].ToString(), tmprbl.SelectedValue, "Portal");
                           }
                           return true;
                       });
                    }
                }

                //主管簽核
                sectionChief.SignOff(model);

                MailInfo info = new MailInfo()
                {
                    AddresseeTemp = System.Web.Configuration.WebConfigurationManager.AppSettings["MailTemplate"],
                    DomainPattern = ConfigUtils.ParsePageSetting("Pattern")["DomainPattern"],
                };

                var mainData = _processwfRepo.GetWorkflowData(model.SignDocID_FK, null);
                DataRow deptData = null;
                //送簽人AD帳號
                var signSender = (string)_rootRepo.QueryForEmployeeByEmpID(mainData.EmployeeID_FK)["ADAccount"];

                switch (mainData.FinalStatus)
                {
                    //待簽核
                    default:
                    case 2:
                        //尋找目前待簽主管AD帳號
                        deptData = _rootRepo.QueryForDepartmentByDeptID(mainData.CurrentSignLevelDeptID_FK);
                        info.To = (string)_rootRepo.QueryForEmployeeByEmpID((string)deptData["chiefID_FK"])["ADAccount"];

                        info.Subject = String.Format("系統提醒!簽核單號 : {0} 已經送達，請儘速處理!", model.SignDocID_FK);
                        info.Body.AppendFormat("{0}{1}", info.Subject, "此件為系統發送，請勿回覆!");
                        break;
                    //駁回
                    case 4:
                        info.To = signSender;

                        info.Subject = String.Format("系統提醒!簽核單號 : {0} 已被駁回，請儘速修改!", model.SignDocID_FK);
                        info.Body.AppendFormat("{0}{1}", info.Subject, "此件為系統發送，請勿回覆!");
                        break;
                    //結案
                    case 6:

                        #region #0013 加班時間小於結薪日，通知email給總務手動要寫入志元 直接判斷Table=>OvertimeForm的AutoInsert

                        //加班單的時候傳的參數為2，方能判斷是否送來簽核的是加班單
                        if (model_M.FormID_FK == 2)
                        {
                            string sid = Request["SignDocID"].ToString();
                            //取得結薪日
                            DateTime salaryLimit = Convert.ToDateTime(_rootRepo.GetSalaryLimit()["LimitDate"]);
                            Overtime overSev = new Overtime();
                            List<OvertimeViewModel> overDataList = new List<OvertimeViewModel>();
                            List<OvertimeViewModel> resultDataList = new List<OvertimeViewModel>();

                            //取得加班資料明細
                            OvertimeRepository _overtimeRepo = RepositoryFactory.CreateOvertimeRepo();
                            DataTable tableData = _overtimeRepo.QueryOvertimeFormData(sid);
                            var rows = tableData.Select();


                            overDataList = _overtimeRepo.dataMapping(tableData);
                            foreach (var over in overDataList)
                            {
                                if (!over.AutoInsert)
                                    resultDataList.Add(over);
                                //DateTime startDate = (DateTime)over.StartDateTime;
                                //int mathDate = new TimeSpan(startDate.Ticks - salaryLimit.Ticks).Days;
                                //if (mathDate <= 0)
                                //resultDataList.Add(over);
                            }

                            //info.To = "wenhua.yu";
                            info.To = "juncheng.liu";
                            info.CC = new List<string>() { "juncheng.liu" };
                            foreach (var ov in resultDataList)
                            {
                                info.Subject = String.Format("系統提醒!加班單號 : {0} [{1}-{2}]手動轉志元通知!", sid, ov.EmployeeID_FK, ov.EmployeeName);
                                DateTime ovStartTime = (DateTime)ov.StartDateTime;
                                StringBuilder mailBody = new StringBuilder();
                                mailBody.AppendLine(@"<p>[通知] 加班單 手動轉志元通知。</p>");
                                mailBody.AppendLine(@"<p>&nbsp;</p>");
                                mailBody.AppendLine(@"<p>Dear&nbsp;遇玟樺：</p>");
                                mailBody.AppendLine(@"<p>需手動轉入志元的加班單如下：</p>");
                                mailBody.AppendLine(@"<p>加班單號：<span style=""color:#0000ff;"">OT201707290010</span></p>");
                                mailBody.AppendLine(@"<p>加班日：" + ovStartTime.ToFullTaiwanDate() + "</p>");
                                //mailBody.AppendLine(@"<p>結薪日：<span style=""color:#ff0000;"">" + salaryLimit.ToFullTaiwanDate() + "</span></p>");
                                mailBody.AppendLine(@"<p>員工ID：" + ov.EmployeeID_FK + "</p>");
                                mailBody.AppendLine(@"<p>員工姓名：" + ov.EmployeeName + "</p>");
                                mailBody.AppendLine(@"<p><a href=""http://portal.rinnai.com.tw/Area/Manage/OvertimeReport.aspx"">Portal</a></p>");
                                info.Body = mailBody;
                                _mailer = new Mailer(info);
                                //if (PublicRepository.CurrentWorkflowMode == Enums.WorkflowTypeEnum.RELEASE)
                                _mailer.SendMail();
                            }
                        }

                        #endregion #0013 加班時間小於結薪日，通知email給總務手動要寫入志元 直接判斷Table=>OvertimeForm的AutoInsert

                        //尋找歸檔人員
                        var fillingDept = (string)_rootRepo.QueryForDepartmentByFormID(mainData.FormID_FK)["DepartmentID"];
                        deptData = _rootRepo.QueryForDepartmentByDeptID(fillingDept);
                        //20170327 修改不寄給歸檔人員
                        //info.To = (string)_rootRepo.QueryForEmployeeByEmpID((string)deptData["FilingEmployeeID_FK"])["ADAccount"];
                        info.To = signSender;
                        //info.CC.Add(signSender);

                        info.Subject = String.Format("系統提醒!簽核單號 : {0} 已通過審核並結案，請儘速確認!", model.SignDocID_FK);
                        info.Body.AppendFormat("{0}{1}", info.Subject, "此件為系統發送，請勿回覆!");
                        break;
                }
                GlobalDiagnosticsContext.Set("User", User.Identity.Name);
                //mail
                _mailer = new Mailer(info);
                if (PublicRepository.CurrentWorkflowMode == Enums.WorkflowTypeEnum.RELEASE)
                    _mailer.SendMail();
                //log
                var cc = String.Join(",", info.CC);
                _log.Trace(String.Format("MailTo : {0}\r\ncc : {1}\r\nTitle : {2}\r\nContent : {3}\r\n", info.To, cc, info.Subject, info.Body));

                Response.Write("已送出簽核".ToAlertAndRedirect("/Area/Sign/ProcessWorkflowList.aspx?orderField=ModifyDate&descending=True"));
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message.ToAlertFormat());
            }
            finally
            {
                //btn處理
                SaveBtn.ButtonOn(CoverBtn);
            }
        }

        public ProcessWorkflowDetailViewModel PageDataBind()
        {
            _rootRepo = RepositoryFactory.CreateRootRepo();
            var empData = _rootRepo.QueryForEmployeeByADAccount(User.Identity.Name);
            var model = WebUtils.ViewModelMapping<ProcessWorkflowDetailViewModel>(this.Page);
            model.ChiefID_FK = empData != null ? empData["EmployeeID"].ToString() : String.Empty;
            model.Modifier = User.Identity.Name;
            return model;
        }

        public string ConstructAspxPage(string docID, int formID)
        {
            if (String.IsNullOrWhiteSpace(docID))
            {
                Response.Redirect(@"/PageNotFound.aspx");
                return null;
            }
            else
            {
                //待簽核作業 > 列表的簽核Url
                var pageSet = ConfigUtils.ParsePageSetting("ProcessSetting");
                var result = String.Format(pageSet[formID.ToString()], docID);
                return result;
            }
        }
    }
}