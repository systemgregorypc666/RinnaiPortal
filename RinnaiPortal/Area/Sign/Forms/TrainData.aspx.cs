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
    //簽核頁面中的受訓單
    public partial class TrainData : System.Web.UI.Page
    {
        private TrainViewModel model = null;
        private TrainDetailRepository _trainDetaiRepo = null;
        protected void Page_Load(object sender, EventArgs e)
        {          
            if (!IsPostBack)
            {
                _trainDetaiRepo = RepositoryFactory.CreateTrainDetailRepo();
                //從QueryString取得 簽核代碼
                string docID = String.IsNullOrEmpty(Request["SignDocID"]) ? String.Empty : Request["SignDocID"].ToString();
                //根據查詢的 簽核代碼 搜尋忘刷單
                model = _trainDetaiRepo.GetData(docID);
                WebUtils.PageDataBind(model, this.Page);
                Signed.NavigateUrl = "~/Area/Sign/WorkflowDetail.aspx?signDocID=" + docID;
            }
            PageInit();
        }

        protected void PageInit()
        {
            if (!String.IsNullOrEmpty(Request["SignDocID"]))
            {
                PlaceHolder1.Controls.Clear();
                var QuestionDataList = _trainDetaiRepo.QueryQuestionDataBySignDocID(Request["SignDocID"]);
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
                    mylblName.Width = 300;
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
                        txtAns.Height = 80;
                        PlaceHolder1.Controls.Add(txtAns);
                    }
                    else if (row["ANSTYPE"].ToString() == "N")//選分數
                    {
                        TextBox txtAns = new TextBox();
                        txtAns.Text = "";
                        txtAns.ID = "textbox" + row["serial_no"].ToString();
                        txtAns.Text = row["ANS"].ToString();
                        //txtAns.TabIndex = (i + 1).ToString();
                        //txtAns.TextMode = TextBoxMode.MultiLine;
                        txtAns.Width = 50;
                        txtAns.Height = 50;
                        PlaceHolder1.Controls.Add(txtAns);
                    }
                    return true;
                });
            }
        }
    }
}