using MailerAPI;
using RinnaiPortal.Extensions;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository;
using RinnaiPortal.Repository.Sign.Forms;
using RinnaiPortal.Repository.SmartMan;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using ValidationAPI;
using ValidationAPI.ViewModel;

namespace RinnaiPortal.Area.Sign.Forms
{
    public partial class ForgotPunch : System.Web.UI.Page
    {
        private ForgotPunchViewModel model = null;
        private ForgotPunchRepository _forgotRepo = null;
        private RootRepository _rootRepo = null;
        private SmartManRepository _smartRepo = null;
        private DataRow _employeeRow = null;
        private DataRow _dailyOnOff = null;
        private DataRow _workTime = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            _forgotRepo = RepositoryFactory.CreateForgotPunchRepo();
            _rootRepo = RepositoryFactory.CreateRootRepo();
            _smartRepo = RepositoryFactory.CreateSmartManRepo();
            _employeeRow = _smartRepo.QueryForEmployee(EmployeeID_FK.Text);

            if (!Authentication.HasResource(User.Identity.Name, "ForgotPunch"))
            {
                Response.Redirect(@"/account/logon.aspx?ReturnUrl=%2f");
            }

            if (!IsPostBack)
            {
                string signDocID = String.IsNullOrEmpty(Request["SignDocID_FK"]) ? String.Empty : Request["SignDocID_FK"].ToString();

                //將部門資料 與下拉式選單綁定
                ViewUtils.SetOptions(DepartmentID_FK, _rootRepo.GetDepartment());
                //忘刷員工資料 與下拉式選單綁定
                ViewUtils.SetOptions(EmployeeID_FK, _rootRepo.GetEmployee());
                //忘刷類型資料 與下拉式選單綁定
                ViewUtils.SetOptions(PeriodType, _rootRepo.GetPeriodType());

                var employeeData = _rootRepo.QueryForEmployeeByADAccount(User.Identity.Name);
                ApplyID_FK.Text = employeeData != null ? employeeData["EmployeeID"].ToString() : String.Empty;
                ApplyName.Text = employeeData != null ? employeeData["EmployeeName"].ToString() : String.Empty;
                ApplyDateTime.Text = DateTime.Now.FormatDatetime();
                EmployeeID_FK.SelectedValue = employeeData != null ? employeeData["EmployeeID"].ToString() : String.Empty;
                DepartmentID_FK.SelectedValue = employeeData != null ? employeeData["DepartmentID_FK"].ToString() : String.Empty;
                PageTitle.Value = "表單新增作業 > 忘刷單";
                FormSeries.Value = "ForgotPunch";

                if (!String.IsNullOrWhiteSpace(signDocID))
                {
                    //將 viewModel 的值綁定到 頁面上
                    WebUtils.PageDataBind(_forgotRepo.GetForgotPunchForm(signDocID), this.Page);

                    SignDocID_FK.Text = signDocID;
                    PageTitle.Value = "表單編輯作業 > 忘刷單";
                }
            }
        }

        protected void EmployeeID_FK_SelectedIndexChanged(object sender, EventArgs e)
        {
            EmployeeName.Value = EmployeeID_FK.Text;
            var employeeData = _rootRepo.QueryForEmployeeByEmpID(EmployeeName.Value);
            DepartmentID_FK.SelectedValue = employeeData != null ? employeeData["DepartmentID_FK"].ToString() : "請選擇";
        }

        protected void PeriodType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dateTime = DateTime.Now.AddDays(-1);

            if (_employeeRow == null)
            {
                Response.Write("查無SmartMan員工基本資料設定檔，請聯絡總務課!".ToAlertFormat());
                PeriodType.SelectedIndex = 0;
                return;
            }

            _workTime = _smartRepo.QueryForWorkTime(_employeeRow["WORKTYPE"].ToString());
            if (_workTime == null)
            {
                Response.Write("查無SmartMan工時定義檔，請聯絡總務課!".ToAlertFormat());
                PeriodType.SelectedIndex = 0;
                return;
            }

            object dateSender = null;

            switch (PeriodType.SelectedValue)
            {
                case "1":
                    ForgotPunchInDateTime.Attributes.Remove("Disabled");
                    ForgotPunchOutDateTime.Attributes.Add("Disabled", "true");
                    dateSender = ForgotPunchInDateTime;
                    ForgotPunchInDateTime.Text = dateTime.FormatDatetime("yyyy-MM-dd") + " " + _workTime["MORNINGTIME"].ToString().Insert(2, ":") + ":00";
                    ForgotPunchOutDateTime.Text = "";
                    break;

                case "2":
                    ForgotPunchInDateTime.Attributes.Add("Disabled", "true");
                    ForgotPunchOutDateTime.Attributes.Remove("Disabled");
                    dateSender = ForgotPunchOutDateTime;
                    ForgotPunchInDateTime.Text = "";
                    ForgotPunchOutDateTime.Text = dateTime.FormatDatetime("yyyy-MM-dd") + " " + _workTime["OFFWORKTIME"].ToString().Insert(2, ":") + ":00"; ;
                    break;

                case "3":
                    ForgotPunchInDateTime.Attributes.Remove("Disabled");
                    ForgotPunchOutDateTime.Attributes.Remove("Disabled");
                    if (String.IsNullOrWhiteSpace(ForgotPunchInDateTime.Text))
                    {
                        dateSender = ForgotPunchOutDateTime;
                    }
                    else
                    {
                        dateSender = ForgotPunchInDateTime;
                    }
                    ForgotPunchInDateTime.Text = dateTime.FormatDatetime("yyyy-MM-dd") + " " + _workTime["MORNINGTIME"].ToString().Insert(2, ":") + ":00"; ;
                    ForgotPunchOutDateTime.Text = dateTime.FormatDatetime("yyyy-MM-dd") + " " + _workTime["OFFWORKTIME"].ToString().Insert(2, ":") + ":00"; ;
                    break;

                case "4":
                    ForgotPunchInDateTime.Text = "";
                    ForgotPunchOutDateTime.Text = "";
                    break;

                default:
                    ForgotPunchInDateTime.Text = dateTime.FormatDatetime("yyyy-MM-dd") + " " + _workTime["MORNINGTIME"].ToString().Insert(2, ":") + ":00"; ;
                    break;
            }

            SelectedDateTime_TextChanged(dateSender, null);
        }

        public ValidationResult PunchDateTimeValidate()
        {
            var validator = new Validator();
            var validResult = validator.ValidateModel(model);
            if (!validResult.IsValid)
            {
                Response.Write(validResult.ErrorMessage.ToString().ToAlertFormat());
            }

            switch (PeriodType.SelectedValue)
            {
                case "1":
                    if (!model.ForgotPunchInDateTime.HasValue)
                    {
                        validResult.ErrorMessage.AppendLine("轉換上班忘刷日期格式失敗!");
                        validResult.IsValid = false;
                    }
                    if (model.ForgotPunchOutDateTime.HasValue)
                    {
                        validResult.ErrorMessage.AppendLine("下班忘刷時間不可填寫!");
                        validResult.IsValid = false;
                    }
                    break;

                case "2":
                    if (!model.ForgotPunchOutDateTime.HasValue)
                    {
                        validResult.ErrorMessage.AppendLine("轉換下班忘刷日期格式失敗!");
                        validResult.IsValid = false;
                    }
                    if (model.ForgotPunchInDateTime.HasValue)
                    {
                        validResult.ErrorMessage.AppendLine("上班忘刷時間不可填寫!");
                        validResult.IsValid = false;
                    }
                    break;

                case "3":
                    if (!model.ForgotPunchInDateTime.HasValue)
                    {
                        validResult.ErrorMessage.AppendLine("轉換上班忘刷日期格式失敗!");
                        validResult.IsValid = false;
                    }
                    if (!model.ForgotPunchOutDateTime.HasValue)
                    {
                        validResult.ErrorMessage.AppendLine("轉換下班忘刷日期格式失敗!");
                        validResult.IsValid = false;
                    }

                    if (!model.ForgotPunchOutDateTime.HasValue || !model.ForgotPunchInDateTime.HasValue)
                    {
                        validResult.ErrorMessage.AppendLine("上下班忘刷時間均為必填!");
                        validResult.IsValid = false;
                    }
                    break;

                case "4":
                    if (!model.ForgotPunchInDateTime.HasValue && !model.ForgotPunchOutDateTime.HasValue)
                    {
                        validResult.ErrorMessage.AppendFormat("轉換上下班忘刷日期格式失敗!");
                        validResult.IsValid = false;
                    }
                    break;

                default:
                    break;
            }

            return validResult;
        }

        public ForgotPunchViewModel PageDataBind()
        {
            var model = viewModelMapping(this.Page);
            return model;
        }

        private ForgotPunchViewModel viewModelMapping(Page page)
        {
            if (_rootRepo == null) { _rootRepo = RepositoryFactory.CreateRootRepo(); }

            var model = WebUtils.ViewModelMapping<ForgotPunchViewModel>(page);
            //根據表單系列決定 SignType Data
            var signTypeData = _rootRepo.QueryForSignTypeDataBySeries(model.FormSeries);
            model.Creator = User.Identity.Name;
            model.Modifier = User.Identity.Name;
            model.FormID_FK = Int32.Parse(signTypeData["FormID"].ToString());
            model.RuleID_FK = signTypeData["SignID_FK"].ToString();

            return model;
        }

        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            _forgotRepo = RepositoryFactory.CreateForgotPunchRepo();
            //取得頁面資料
            model = PageDataBind();

            var validResult = PunchDateTimeValidate();
            var duplicateResult = IsDuplicateSubmit();
            var theSameDateResult = IsTheSameDate();
            if (!validResult.IsValid || !duplicateResult.IsValid || !theSameDateResult.IsValid)
            {
                var errorMessage = String.Concat(validResult.ErrorMessage, ",", duplicateResult.ErrorMessage, ",", theSameDateResult.ErrorMessage);
                Response.Write(String.Join("\r\n", errorMessage.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)).ToAlertFormat());
                return;
            }

            //btn處理
            ViewUtils.ButtonOff(SaveBtn, CoverBtn);

            var responseMessage = String.Empty;
            try
            {
                var modelList = new List<RinnaiForms>();
                modelList.Add(model);
                //存檔
                if (String.IsNullOrEmpty(Request["SignDocID_FK"]))
                {
                    _forgotRepo.CreateData(modelList);
                    responseMessage = "新增成功!";
                }
                else
                {
                    _forgotRepo.EditData(modelList);
                    responseMessage = "編輯成功!";
                }
                #region #0022忘刷送出新增email通知
                //主管ADAccount
                string chiefID = _forgotRepo.FindChiefID(Request["SignDocID_FK"], User.Identity.Name);
                if (!string.IsNullOrEmpty(chiefID))
                {
                    MailerAPI.MailInfo mailInfo = new MailerAPI.MailInfo()
              {
                  AddresseeTemp = System.Web.Configuration.WebConfigurationManager.AppSettings["MailTemplate"],
                  Subject = String.Format("系統提醒!簽核單號 : {0} 已經送達!", model.SignDocID_FK),
                  //Subject = "系統部測試，請勿理會此郵件，謝謝!",
                  DomainPattern = ConfigUtils.ParsePageSetting("Pattern")["DomainPattern"],
                  CC = new System.Collections.Generic.List<string>() { "juncheng.liu@rinnai.com.tw" },
                  //To = string.Format("{0}@rinnai.com.tw", chiefID)
                  To = string.Format("{0}@rinnai.com.tw", chiefID)
              };
                    var portalDomain = ConfigUtils.ParsePageSetting("Domain")["Portal"];

                    string body =
                        MailTools.BodyToTable(
                            String.Format(
                        @"系統提醒!簽核單號 : {0} 已經送達，請儘速檢視!<br /><a href='{1}/Area/Sign/ProcessWorkflowList.aspx?queryText={0}'>連結</a> <br />此件為系統發送，請勿回覆!",
                        //@"系統部測試，請勿理會此郵件，謝謝!",
                                model.SignDocID_FK, portalDomain));
                    mailInfo.Body.Append(body);
                    if (PublicRepository.CurrentWorkflowMode == Enums.WorkflowTypeEnum.RELEASE)
                        new Mailer(mailInfo).SendMail();
                }
                #endregion

                ViewUtils.ButtonOn(SaveBtn, CoverBtn);
                responseMessage = responseMessage.ToAlertAndRedirect(@"/Area/Sign/WorkflowQueryList.aspx?orderField=CreateDate&descending=True");
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

        private ValidationResult IsDuplicateSubmit()
        {
            var result = new ValidationResult();
            var empID = EmployeeID_FK.SelectedValue;
            var punchIN = ForgotPunchInDateTime.Text.ToDateFormateString("yyyyMMdd");
            var punchOut = ForgotPunchOutDateTime.Text.ToDateFormateString("yyyyMMdd");
            var punchDate = String.IsNullOrWhiteSpace(punchIN) ? punchOut : punchIN;
            var punchData = _smartRepo.QueryForLostCard(empID, punchDate);
            if (punchData != null && punchData.Rows.Count > 0)
            {
                result.IsValid = false;
                result.ErrorMessage.AppendFormat("{0}忘刷單資料，不得重複送出!", punchDate);
            }
            else { result.IsValid = true; }

            return result;
        }

        private ValidationResult IsTheSameDate()
        {
            var result = new ValidationResult() { IsValid = true };
            if ("3".Equals(PeriodType.SelectedValue) && !String.IsNullOrWhiteSpace(ForgotPunchInDateTime.Text) && !String.IsNullOrWhiteSpace(ForgotPunchOutDateTime.Text))
            {
                var punchIN = ForgotPunchInDateTime.Text.ToDateFormateString("yyyyMMdd");
                var punchOut = ForgotPunchOutDateTime.Text.ToDateFormateString("yyyyMMdd");
                if (punchIN != punchOut)
                {
                    result.IsValid = false;
                    result.ErrorMessage.AppendFormat("上下班忘刷日期必須相同!");
                }
            }

            return result;
        }

        protected void SelectedDateTime_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(ForgotPunchInDateTime.Text) && String.IsNullOrWhiteSpace(ForgotPunchOutDateTime.Text)) { return; }

            SaveBtn.Visible = true;

            var send = sender as TextBox;
            if (send == null) { return; }

            var date = String.Empty;
            if ("ForgotPunchOutDateTime".Equals(send.ID))
            {
                date = ForgotPunchOutDateTime.Text.ToDateFormateString("yyyyMMdd");
            }
            else if ("ForgotPunchInDateTime".Equals(send.ID))
            {
                date = ForgotPunchInDateTime.Text.ToDateFormateString("yyyyMMdd");
            }

            selectedDateTimeIdentify(date, "Begin");
        }

        //20161107 修正卡控判斷
        private void selectedDateTimeIdentify(string date, string type)
        {
            _dailyOnOff = _smartRepo.QueryDailyOnOff(date);

            var dutyWorkData = _smartRepo.QueryForDutyWork(date, EmployeeID_FK.SelectedValue);
            var EmpdailyOnOff = _smartRepo.QueryForDailyOnOff(date, EmployeeID_FK.SelectedValue);
            if (_dailyOnOff == null)
            {
                SaveBtn.Visible = false;
                RealPunchIn.Text = "卡鐘資料尚未轉入資料庫!";
                RealPunchOut.Text = "卡鐘資料尚未轉入資料庫!";
                return;
            }
            else
            {
                //20170113 若當天有任何刷卡資料即可填寫忘刷
                RealPunchIn.Text = "查無打卡紀錄!";
                RealPunchOut.Text = "查無打卡紀錄!";
                //return;
            }

            if (dutyWorkData != null)
            {
                if (dutyWorkData["HOLIDAY"].ToString() == "Y" && dutyWorkData["H_TYPE"].ToString() == "0")
                {
                    SaveBtn.Visible = false;
                    RealPunchIn.Text = "忘刷日期為例假日，不可申請!";
                    RealPunchOut.Text = "忘刷日期為例假日，不可申請";
                    return;
                }
            }
            else
            {
                SaveBtn.Visible = false;
                RealPunchIn.Text = "查無班表定義，請聯絡總務課!";
                RealPunchOut.Text = "查無班表定義，請聯絡總務課!";
                return;
            }

            if (EmpdailyOnOff == null)
            {
                //SaveBtn.Visible = false;
                RealPunchIn.Text = "查無打卡紀錄!";
                RealPunchOut.Text = "查無打卡紀錄!";
            }
            else
            {
                var workTime = _smartRepo.QueryForWorkTime(EmpdailyOnOff["WORKTYPE"].ToString());
                if (workTime == null)
                {
                    SaveBtn.Visible = false;
                    RealPunchIn.Text = "查無工時定義，請聯絡總務課!";
                    RealPunchOut.Text = "查無工時定義，請聯絡總務課!";
                    return;
                }

                var beginTimeBase = Int32.Parse(workTime["MORNINGTIME"].ToString());
                var endTimeBase = Int32.Parse(workTime["OFFWORKTIME"].ToString());

                if (!String.IsNullOrWhiteSpace(EmpdailyOnOff["BeginTime"].ToString()) && !String.IsNullOrWhiteSpace(EmpdailyOnOff["EndTime"].ToString()))
                {
                    //SaveBtn.Visible = false;
                    RealPunchIn.Text = String.Concat(EmpdailyOnOff["DUTYDATE"], EmpdailyOnOff["BeginTime"].ToString().PadRight(6, '0')).ToDateFormateString();
                    RealPunchOut.Text = String.Concat(EmpdailyOnOff["DUTYDATE"], EmpdailyOnOff["EndTime"].ToString().PadRight(6, '0')).ToDateFormateString();
                    return;
                }
                else if (String.IsNullOrWhiteSpace(EmpdailyOnOff["BeginTime"].ToString()) && String.IsNullOrWhiteSpace(EmpdailyOnOff["EndTime"].ToString()))
                {
                    RealPunchIn.Text = "查無打卡紀錄";
                    RealPunchOut.Text = "查無打卡紀錄";
                    //if (!"3".Equals(PeriodType.SelectedValue)) { SaveBtn.Visible = false; }
                    return;
                }
                else if (String.IsNullOrWhiteSpace(EmpdailyOnOff["BeginTime"].ToString()) && !String.IsNullOrWhiteSpace(EmpdailyOnOff["EndTime"].ToString()))
                {
                    RealPunchIn.Text = "查無打卡紀錄";
                    RealPunchOut.Text = String.Concat(EmpdailyOnOff["DUTYDATE"], EmpdailyOnOff["BeginTime"].ToString().PadRight(6, '0')).ToDateFormateString();
                    //if (!"1".Equals(PeriodType.SelectedValue)) { SaveBtn.Visible = false; }
                    return;
                }
                else if (!String.IsNullOrWhiteSpace(EmpdailyOnOff["BeginTime"].ToString()) && String.IsNullOrWhiteSpace(EmpdailyOnOff["EndTime"].ToString()))
                {
                    RealPunchIn.Text = String.Concat(EmpdailyOnOff["DUTYDATE"], EmpdailyOnOff["BeginTime"].ToString().PadRight(6, '0')).ToDateFormateString();
                    RealPunchOut.Text = "查無打卡紀錄";
                    //if (!"2".Equals(PeriodType.SelectedValue)) { SaveBtn.Visible = false; }
                    return;
                }
                //else if (Int32.Parse(_dailyOnOff["BeginTime"].ToString()) >= endTimeBase)
                //{
                //    RealPunchIn.Text = "查無打卡紀錄";
                //    RealPunchOut.Text = String.Concat(_dailyOnOff["DUTYDATE"], _dailyOnOff["BeginTime"].ToString().PadRight(6, '0')).ToDateFormateString();
                //    if (!"1".Equals(PeriodType.SelectedValue)) { SaveBtn.Visible = false; }
                //    return;
                //}
                //else if (Int32.Parse(_dailyOnOff["BeginTime"].ToString()) <= beginTimeBase)
                //{
                //    RealPunchIn.Text = String.Concat(_dailyOnOff["DUTYDATE"], _dailyOnOff["BeginTime"].ToString().PadRight(6, '0')).ToDateFormateString();
                //    RealPunchOut.Text = "查無打卡紀錄";
                //    if (!"2".Equals(PeriodType.SelectedValue)) { SaveBtn.Visible = false; }
                //    return;
                //}
                else
                {
                    return;
                }
            }
        }
    }
}