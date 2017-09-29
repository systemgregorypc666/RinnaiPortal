using MailerAPI;
using RinnaiPortal.Enums;
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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RinnaiPortal.Area.Sign.Forms
{
    public partial class TrainDetail02 : System.Web.UI.Page
    {
        private NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private TrainViewModel model = null;
        private RootRepository _rootRepo = null;
        private TrainDetailRepository _trainDetaiRepo = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            _trainDetaiRepo = RepositoryFactory.CreateTrainDetailRepo();
            _rootRepo = RepositoryFactory.CreateRootRepo();
            if (!Authentication.HasResource(User.Identity.Name, "TrainDetail02"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }
            if (!IsPostBack)
            {
                string signDocID = String.IsNullOrEmpty(Request["SignDocID_FK"]) ? String.Empty : Request["SignDocID_FK"].ToString();
                PageTitle.Value = "受訓心得報告維護 > 編輯";
                FormSeries.Value = "Train";

                string CLID = String.IsNullOrEmpty(Request["CLID"]) ? String.Empty : Request["CLID"].ToString();
                string SID = String.IsNullOrEmpty(Request["SID"]) ? String.Empty : Request["SID"].ToString();
                var employeeData = _rootRepo.QueryForEmployeeByEmpID(SID);
                string DepartmentID_FK = employeeData != null ? employeeData["DepartmentID_FK"].ToString() : String.Empty;
                model = _trainDetaiRepo.GetDetail(CLID, SID, DepartmentID_FK);

                // 取得 QueryString
                //var paggerParms = WebUtils.ParseQueryString<PaggerParms>(Page.Request);
                //var signListParms = WebUtils.ParseQueryString<SignListParms>(Page.Request);
                //signListParms.Member = Authentication.GetMemberViewModel(User.Identity.Name);
                //signListParms.GridView = TrainDetailGridView;
                //signListParms.PaginationBar = paginationBar;
                //paggerParms.OrderField = "qno";

                ////建構頁面
                //ConstructPage(signListParms, paggerParms, RepositoryFactory.CreateTrainDetailRepo());
                WebUtils.PageDataBind(model, this.Page);
                if (!String.IsNullOrWhiteSpace(signDocID))
                {
                    //將 viewModel 的值綁定到 頁面上
                    WebUtils.PageDataBind(_trainDetaiRepo.GetData(signDocID), this.Page);

                    SignDocID_FK.Text = signDocID;
                    PageTitle.Value = "表單編輯作業 > 忘刷單";
                }
            }
            PageInit();
        }

        protected void PageInit()
        {
            //編輯受訓心得
            if (!String.IsNullOrEmpty(Request["SignDocID_FK"]))
            {
                txterror.Text = "";
                PlaceHolder1.Controls.Clear();
                var QuestionDataList = _trainDetaiRepo.QueryQuestionDataBySignDocID(Request["SignDocID_FK"]);
                QuestionDataList.All(row =>
                {
                    PlaceHolder1.Controls.Add(new LiteralControl("<hr style='margin:6px;height:1px;border:0px;background-color:#D5D5D5;color:#D5D5D5;'/>"));
                    PlaceHolder1.Controls.Add(new LiteralControl("&nbsp;&nbsp"));
                    //序號
                    Label myLabel = new Label();
                    myLabel.Text = "";
                    myLabel.Text = row["serial_no"].ToString();
                    myLabel.ID = "Label" + row["serial_no"].ToString();
                    //myLabel.BackColor = "#FFFFFF";
                    myLabel.Width = 10;
                    PlaceHolder1.Controls.Add(myLabel);
                    PlaceHolder1.Controls.Add(new LiteralControl("&nbsp;&nbsp"));
                    //題目
                    Label mylblName = new Label();
                    mylblName.Text = "";
                    mylblName.Text = row["CODENAME"].ToString();
                    mylblName.ID = "LabelName" + row["serial_no"].ToString();
                    mylblName.Width = 400;
                    PlaceHolder1.Controls.Add(mylblName);
                    PlaceHolder1.Controls.Add(new LiteralControl("&nbsp;&nbsp"));
                    //答案
                    if (row["ANSTYPE"].ToString() == "C")//填文字
                    {
                        TextBox txtAns = new TextBox();
                        txtAns.Text = "";
                        txtAns.ID = "textbox" + row["serial_no"].ToString();
                        txtAns.Text = row["ANS"].ToString();
                        //txtAns.TabIndex = (i + 1).ToString();
                        txtAns.TextMode = TextBoxMode.MultiLine;
                        txtAns.Width = 500;
                        txtAns.Height = 100;
                        PlaceHolder1.Controls.Add(txtAns);
                    }
                    return true;
                });
            }
            //新增受訓心得
            if (!String.IsNullOrEmpty(Request["CLID"]) && !String.IsNullOrEmpty(Request["SID"]))
            {
                txterror.Text = "";
                PlaceHolder1.Controls.Clear();
                var QuestionDataList = _trainDetaiRepo.QueryQuestionDataByTableID("02");
                int i = 0;
                QuestionDataList.All(row =>
                {
                    PlaceHolder1.Controls.Add(new LiteralControl("<hr style='margin:6px;height:1px;border:0px;background-color:#D5D5D5;color:#D5D5D5;'/>"));
                    PlaceHolder1.Controls.Add(new LiteralControl("&nbsp;&nbsp"));
                    //序號
                    Label myLabel = new Label();
                    myLabel.Text = "";
                    myLabel.Text = row["serial_no"].ToString();
                    myLabel.ID = "Label" + row["serial_no"].ToString();
                    //myLabel.BackColor = "#FFFFFF";
                    myLabel.Width = 10;
                    PlaceHolder1.Controls.Add(myLabel);
                    //if (Int32.Parse(row["serial_no"].ToString()) < 10)
                    //{
                    //    PlaceHolder1.Controls.Add(new LiteralControl("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp"));
                    //}
                    //else
                    //{
                    PlaceHolder1.Controls.Add(new LiteralControl("&nbsp;&nbsp;&nbsp;&nbsp"));
                    //}

                    //題目
                    Label mylblName = new Label();
                    mylblName.Text = "";
                    mylblName.Text = row["CODENAME"].ToString();
                    mylblName.ID = "LabelName" + row["serial_no"].ToString();
                    mylblName.Width = 400;
                    PlaceHolder1.Controls.Add(mylblName);
                    PlaceHolder1.Controls.Add(new LiteralControl("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp"));
                    //答案
                    if (row["ANSTYPE"].ToString() == "C")//填文字
                    {
                        TextBox txtAns = new TextBox();
                        txtAns.Text = "";
                        txtAns.ID = "textbox" + row["serial_no"].ToString();
                        //txtAns.TabIndex = (i + 1).ToString();
                        txtAns.TextMode = TextBoxMode.MultiLine;
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

                    //PlaceHolder1.Controls.Add(new LiteralControl("<BR/>"));
                    i = i + 1;
                    return true;
                });
            }
        }

        public TrainViewModel PageDataBind()
        {
            var model = viewModelMapping(this.Page);
            return model;
        }

        private TrainViewModel viewModelMapping(Page page)
        {
            if (_rootRepo == null) { _rootRepo = RepositoryFactory.CreateRootRepo(); }

            var model = WebUtils.ViewModelMapping<TrainViewModel>(page);
            //根據表單系列決定 SignType Data
            var signTypeData = _rootRepo.QueryForSignTypeDataBySeries(model.FormSeries);
            model.Creator = User.Identity.Name;
            model.Modifier = User.Identity.Name;
            model.FormID_FK = Int32.Parse(signTypeData["FormID"].ToString());
            model.RuleID_FK = signTypeData["SignID_FK"].ToString();

            return model;
        }

        /// <summary>
        /// 表單送出觸發事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            //送簽文件ID
            string signID = Request["SignDocID_FK"];

            //取得資料庫連線字串並建立DB模型
            _trainDetaiRepo = RepositoryFactory.CreateTrainDetailRepo();
            //欲作業的表格對象代碼
            string trainTableID = ((int)TrainingTableTypeEnum.TABLE02).ToString().PadLeft(2, '0');

            //取得頁面資料
            model = PageDataBind();

            #region #0015 取得登入者ID 擴充可幫他人填寫受訓心得表

            var empDc = ConnectionFactory.GetPortalDC();
            var empRepos = new EmployeeRepository(empDc, new RootRepository(empDc));
            EmployeeViewModel empResult = empRepos.GetEmployeeDataByADAccount(User.Identity.Name);
            (model as RinnaiForms).ApplyID_FK = empResult.EmployeeID;

            #endregion #0015 取得登入者ID 擴充可幫他人填寫受訓心得表

            List<DataRow> QuestionDataList = _trainDetaiRepo.QueryQuestionDataByTableID(trainTableID);

            String chkString = "";
            String ansString = "";
            txterror.Text = "";

            #region 遍歷所有題目 判斷是否有填答案

            QuestionDataList.All(row =>
            {
                //如果題目填答型態為文字
                if (row["ANSTYPE"].ToString() == "C")
                {
                    TextBox tmpbox = PlaceHolder1.FindControl("textbox" + row["serial_no"].ToString()) as TextBox;
                    ansString = tmpbox.Text;
                    if (ansString == "")
                        chkString += "第" + row["serial_no"].ToString() + "題 不可空白!";
                }
                //如果題目填答型態為數字
                else if (row["ANSTYPE"].ToString() == "N")
                {
                    RadioButtonList tmprbl = (RadioButtonList)PlaceHolder1.FindControl("rbl" + row["serial_no"].ToString());
                    ansString = tmprbl.SelectedValue;
                    if (ansString == "")
                        chkString += "第" + row["serial_no"].ToString() + "題 不可空白!";
                }
                return true;
            });

            #endregion 遍歷所有題目 判斷是否有填答案

            //有沒填的就直接回要面並顯示警示訊息
            if (chkString != "")
            {
                txterror.Text = chkString;
                return;
            }

            //回傳至頁面的訊息
            var responseMessage = "";
            try
            {
                var modelList = new List<RinnaiForms>();
                modelList.Add(model);

                //新增畫面(即未傳入簽核ID)
                if (String.IsNullOrEmpty(signID))
                {
                    #region 建立簽核(送出簽核)

                    _trainDetaiRepo.CreateSignData(modelList);

                    #endregion 建立簽核(送出簽核)

                    string classID = Request["CLID"];
                    string studentID = Request["SID"];

                    #region 先刪後增

                    #region 刪除答案

                    //刪除課程與簽核的關聯檔
                    _trainDetaiRepo.FromTableNameDeleteValue("FORM_SIGN", classID, studentID, trainTableID);
                    //===刪除全部原有答案===
                    //文字型態
                    _trainDetaiRepo.FromTableNameDeleteValue("CHARACTER_ANSWER", classID, studentID, trainTableID);
                    //數字型態
                    _trainDetaiRepo.FromTableNameDeleteValue("NUMERIC_ANSWER", classID, studentID, trainTableID);

                    #endregion 刪除答案

                    #region 新增答案

                    QuestionDataList.All(row =>
                    {
                        //文字類型
                        if (row["ANSTYPE"].ToString() == "C")
                        {
                            //新增
                            TextBox tmpbox = PlaceHolder1.FindControl("textbox" + row["serial_no"].ToString()) as TextBox;
                            _trainDetaiRepo.AddAnswer_C(Request["CLID"].ToString(), Request["SID"].ToString(), row["TABLE_ID"].ToString(), row["QNO"].ToString(), row["serial_no"].ToString(), tmpbox.Text, User.Identity.Name);
                        }
                        //數字類型
                        else if (row["ANSTYPE"].ToString() == "N")
                        {
                            //新增
                            RadioButtonList tmprbl = (RadioButtonList)PlaceHolder1.FindControl("rbl" + row["serial_no"].ToString());
                            _trainDetaiRepo.AddAnswer_N(Request["CLID"].ToString(), Request["SID"].ToString(), row["TABLE_ID"].ToString(), row["QNO"].ToString(), row["serial_no"].ToString(), tmprbl.SelectedValue, User.Identity.Name);
                        }
                        return true;
                    });

                    #endregion 新增答案

                    #endregion 先刪後增

                    string EID = _trainDetaiRepo.Find_EDITIONID(trainTableID);
                    bool hasEmptySignFormRecords = _trainDetaiRepo.CheckHasSignFormRecords(Request["CLID"].ToString(), trainTableID, EID);

                    //寫入空白單記錄
                    if (!hasEmptySignFormRecords)
                        _trainDetaiRepo.AddRECORDS(Request["CLID"].ToString(), trainTableID, EID);

                    #region 建立關聯簽核主檔

                    _trainDetaiRepo.AddFormSign(Request["CLID"].ToString(), Request["SID"].ToString(), trainTableID, false, modelList);

                    #endregion 建立關聯簽核主檔
                }

                //編輯畫面
                else
                {
                    //送出簽核
                    _trainDetaiRepo.EditData(modelList);

                    //刪除原有答案
                    string strCLID = _trainDetaiRepo.Find_CLID(Request["SignDocID_FK"]);
                    string strSID = _trainDetaiRepo.Find_SID(Request["SignDocID_FK"]);

                    _trainDetaiRepo.FromTableNameDeleteValue("CHARACTER_ANSWER", strCLID, strSID, trainTableID);
                    _trainDetaiRepo.FromTableNameDeleteValue("NUMERIC_ANSWER", strCLID, strSID, trainTableID);

                    QuestionDataList.All(row =>
                    {
                        if (row["ANSTYPE"].ToString() == "C")//填文字
                        {
                            //新增
                            TextBox tmpbox = PlaceHolder1.FindControl("textbox" + row["serial_no"].ToString()) as TextBox;
                            _trainDetaiRepo.AddAnswer_C(strCLID, strSID, row["TABLE_ID"].ToString(), row["QNO"].ToString(), row["serial_no"].ToString(), tmpbox.Text, User.Identity.Name);
                        }
                        else if (row["ANSTYPE"].ToString() == "N")
                        {
                            //新增
                            RadioButtonList tmprbl = (RadioButtonList)PlaceHolder1.FindControl("rbl" + row["serial_no"].ToString());
                            _trainDetaiRepo.AddAnswer_N(strCLID, strSID, row["TABLE_ID"].ToString(), row["QNO"].ToString(), row["serial_no"].ToString(), tmprbl.SelectedValue, User.Identity.Name);
                        }
                        return true;
                    });

                    String EID = _trainDetaiRepo.Find_EDITIONID(trainTableID);
                    bool hasEmptySignFormRecords = _trainDetaiRepo.CheckHasSignFormRecords(strCLID, trainTableID, EID);
                    //寫入空白單記錄
                    if (!hasEmptySignFormRecords)
                        _trainDetaiRepo.AddRECORDS(strCLID, trainTableID, EID);
                }
                responseMessage = "送出成功!";

                #region 寄送email通知

                MailInfo mailInfo = new MailInfo()
                {
                    AddresseeTemp = System.Web.Configuration.WebConfigurationManager.AppSettings["MailTemplate"],
                    Subject = String.Format("系統提醒!簽核單號 : {0} 已經送達!", model.SignDocID_FK),
                    DomainPattern = ConfigUtils.ParsePageSetting("Pattern")["DomainPattern"],
                };

                // 主管簽核 會簽 mail
                var deptData = _rootRepo.QueryForDepartmentByDeptID(model.DepartmentID_FK.ToString());
                var empData = _rootRepo.QueryForEmployeeByEmpID(deptData["ChiefID_FK"].ToString());
                var chiefAD = empData["ADAccount"].ToString();
                mailInfo.To += String.Format(mailInfo.AddresseeTemp, chiefAD) + ",";

                var portalDomain = ConfigUtils.ParsePageSetting("Domain")["Portal"];

                string body =
                    MailTools.BodyToTable(
                        String.Format(
                            @"系統提醒!簽核單號 : {0} 已經送達，請儘速檢視!<br /><a href='{1}/Area/Sign/ProcessWorkflowList.aspx?queryText={0}'>連結</a> <br />此件為系統發送，請勿回覆!",
                            model.SignDocID_FK, portalDomain));

                mailInfo.Body.Append(body);

                mailInfo.CC = new List<string>() { String.Format(mailInfo.AddresseeTemp, User.Identity.Name) };
                mailInfo.To = mailInfo.To.TrimEnd(new char[] { ',' });
                if (PublicRepository.CurrentWorkflowMode == Enums.WorkflowTypeEnum.RELEASE)
                    new Mailer(mailInfo).SendMail();

                #endregion 寄送email通知

                _log.Trace(String.Format("MailTo : {0}\r\ncc : {1}\r\nTitle : {2}\r\nContent : {3}\r\n", mailInfo.To, String.Join(",", mailInfo.CC), mailInfo.Subject, mailInfo.Body));
                //btn處理
                ViewUtils.ButtonOn(SaveBtn, CoverBtn);
                responseMessage = responseMessage.ToAlertAndRedirect(@"/Area/Sign/WorkflowQueryList.aspx?orderField=CreateDate&descending=True");
            }
            catch (Exception ex)
            {
                responseMessage = String.Concat("送出失敗!\r\n錯誤訊息: ", ex.Message).ToAlertFormat();
                ViewUtils.ShowRefreshBtn(CoverBtn, RefreshBtn);
            }
            finally
            {
                Response.Write(responseMessage);
            }
        }

        //public override void VerifyRenderingInServerForm(Control control)
        //{
        //    //處理'GridView' 的控制項 'GridView' 必須置於有 runat=server 的表單標記之中
        //}
    }
}