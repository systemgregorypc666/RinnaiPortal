using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository.Sign.Forms;
using RinnaiPortal.Interface;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RinnaiPortal.Extensions;

namespace RinnaiPortal.Area.Sign.Forms
{
    public partial class TrainDetail01 : SignDataList
    {
        private TrainViewModel model = null;
        private TrainDetailRepository _trainDetaiRepo = null;
        private RootRepository _rootRepo = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            _trainDetaiRepo = RepositoryFactory.CreateTrainDetailRepo();
            _rootRepo = RepositoryFactory.CreateRootRepo();
            if (!Authentication.HasResource(User.Identity.Name, "TrainDetail01"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }
            if (!IsPostBack)
            {
                PageTitle.Value = "學員意見調查維護 > 編輯";

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
                SaveBtn.Attributes.Add("onclick ", "return confirm( '提醒：送出後就不可更改，確定要送出嗎?');");

            }
            PageInit();

        }
        protected void PageInit()
        {
            if (!String.IsNullOrEmpty(Request["CLID"]) && !String.IsNullOrEmpty(Request["SID"]))
            {
                txterror.Text = "";
                PlaceHolder1.Controls.Clear();
                var QuestionDataList = _trainDetaiRepo.QueryQuestionDataByTableID("01");
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
                        txtAns.Width = 450;
                        txtAns.Height = 80;
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
            else
            { }

        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            _trainDetaiRepo = RepositoryFactory.CreateTrainDetailRepo();
            var QuestionDataList = _trainDetaiRepo.QueryQuestionDataByTableID("01");
            String chkString = "";
            String ansString = "";
            txterror.Text = "";
            if (!String.IsNullOrEmpty(Request["CLID"]) && !String.IsNullOrEmpty(Request["SID"]))
            {
                QuestionDataList.All(row =>
                {
                    if (row["ANSTYPE"].ToString() == "C")//填文字
                    {
                        TextBox tmpbox = PlaceHolder1.FindControl("textbox" + row["serial_no"].ToString()) as TextBox;
                        ansString = tmpbox.Text;
                        if (ansString == "")
                        {
                            chkString += "第" + row["serial_no"].ToString() + "題 不可空白!";
                        }

                    }
                    else if (row["ANSTYPE"].ToString() == "N")
                    {
                        //RadioButtonList tmprbl = PlaceHolder1.FindControl("rbl" + row["serial_no"].ToString()) as RadioButtonList;
                        RadioButtonList tmprbl = (RadioButtonList)PlaceHolder1.FindControl("rbl" + row["serial_no"].ToString());
                        ansString = tmprbl.SelectedValue;
                        if (ansString == "")
                        {
                            chkString += "第" + row["serial_no"].ToString() + "題 不可空白!";
                        }
                    }

                    return true;
                });
            }
            if (chkString != "")
            {
                txterror.Text = chkString;
            }
            else
            {
                var responseMessage = "";
                var successRdUrl = String.Empty;
                try
                {
                    var modelList = new List<RinnaiForms>();
                    modelList.Add(model);
                    //刪除原有答案
                    _trainDetaiRepo.FromTableNameDeleteValue("CHARACTER_ANSWER", Request["CLID"], Request["SID"], "01");
                    _trainDetaiRepo.FromTableNameDeleteValue("NUMERIC_ANSWER", Request["CLID"], Request["SID"], "01");
                    QuestionDataList.All(row =>
                    {
                        if (row["ANSTYPE"].ToString() == "C")//填文字
                        {
                            //新增
                            TextBox tmpbox = PlaceHolder1.FindControl("textbox" + row["serial_no"].ToString()) as TextBox;
                            //ansString = tmpbox.Text;
                            _trainDetaiRepo.AddAnswer_C(Request["CLID"].ToString(), Request["SID"].ToString(), row["TABLE_ID"].ToString(), row["QNO"].ToString(), row["serial_no"].ToString(), tmpbox.Text, User.Identity.Name);
                        }
                        else if (row["ANSTYPE"].ToString() == "N")
                        {
                            //新增
                            RadioButtonList tmprbl = (RadioButtonList)PlaceHolder1.FindControl("rbl" + row["serial_no"].ToString());
                            //ansString = tmprbl.SelectedValue;
                            _trainDetaiRepo.AddAnswer_N(Request["CLID"].ToString(), Request["SID"].ToString(), row["TABLE_ID"].ToString(), row["QNO"].ToString(), row["serial_no"].ToString(), tmprbl.SelectedValue, User.Identity.Name);
                        }
                        return true;
                    });
                    String EID = _trainDetaiRepo.Find_EDITIONID("01");
                    bool hasEmptySignFormRecords = _trainDetaiRepo.CheckHasSignFormRecords(Request["CLID"].ToString(), "01", EID);
                    //寫入空白單記錄
                    if (!hasEmptySignFormRecords)
                        _trainDetaiRepo.AddRECORDS(Request["CLID"].ToString(), "01", EID);
                    //寫入記錄調查表的簽核狀態(只有02表有簽核單號)
                    _trainDetaiRepo.AddFormSign(Request["CLID"].ToString(), Request["SID"].ToString(), "01", true, modelList);
                    successRdUrl = @"TrainList01.aspx?orderField=CLID&descending=True";
                    responseMessage = "送出成功!";

                    //btn處理
                    ViewUtils.ButtonOn(SaveBtn, CoverBtn);
                    responseMessage = responseMessage.ToAlertAndRedirect(successRdUrl);

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
        }

        //public override void VerifyRenderingInServerForm(Control control)
        //{
        //    //處理'GridView' 的控制項 'GridView' 必須置於有 runat=server 的表單標記之中
        //}
    }
}