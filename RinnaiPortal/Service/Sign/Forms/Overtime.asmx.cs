using MailerAPI;
using NLog;
using RinnaiPortal.Extensions;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Repository;
using RinnaiPortal.Repository.Sign.Forms;
using RinnaiPortal.Repository.SmartMan;
using RinnaiPortal.Tools;
using RinnaiPortal.Tools.Sign.Forms;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace RinnaiPortal
{
    /// <summary>
    /// Summary description for Overtime
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
    [System.Web.Script.Services.ScriptService]
    public class Overtime : System.Web.Services.WebService
    {
        private NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private OvertimeRepository _overtimeRepo { get; set; }
        private RootRepository _rootRepo { get; set; }
        private SmartManRepository _smartRepo { get; set; }
        private ProcessWorkflowRepository _pwfRepo { get; set; }

        public Overtime()
        {
            _overtimeRepo = RepositoryFactory.CreateOvertimeRepo();
            _rootRepo = RepositoryFactory.CreateRootRepo();
            _smartRepo = RepositoryFactory.CreateSmartManRepo();
            _pwfRepo = RepositoryFactory.CreateProcessWorkflowRepo();

            GlobalDiagnosticsContext.Set("User", User.Identity.Name);
        }

        //根據選擇資料產生明細
        [WebMethod(enableSession: true)]
        public void CreateTableByDefaultValue()
        {
            //設定輸出格式為json格式
            this.Context.Response.ContentType = "application/json";
            //post
            var dataFields = this.Context.Request.Form;
            Dictionary<string, string> model = new Dictionary<string, string>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //20161101 要提高上限，否則物件較大時會產生例外
            serializer.MaxJsonLength = int.MaxValue;
            var responseObj = new Dictionary<string, string>();
            string responseMessage = String.Empty;
            object logKey = new object();
            try
            {
                if (dataFields == null || !dataFields.HasKeys())
                {
                    throw new Exception();
                }

                foreach (var key in dataFields.Keys)
                {
                    logKey = key;
                    model.Add(key.ToString(), dataFields[key.ToString()].ToString());
                }

                var data = _overtimeRepo.GetDefaultData(model);

                var dicHtml = constructRows(data);

                responseMessage = serializer.Serialize(dicHtml);
            }
            catch (Exception ex)
            {
                responseObj.Add("ErrorKey", logKey.ToString());
                responseObj.Add("ErrorMessage", ex.Message);
                responseObj.Add("Keys", String.Join(",", this.Context.Request.Form.AllKeys));
                responseMessage = serializer.Serialize(responseObj);
                _log.Error(responseMessage);
                this.Context.Response.StatusCode = 500;
            }
            finally
            {
                this.Context.Response.Write(responseMessage);
            }
        }

        //根據選擇人員資料 回傳部門資料
        [WebMethod(enableSession: true)]
        public void GetDepartmentData()
        {
            //設定輸出格式為json格式
            this.Context.Response.ContentType = "application/json";
            //post
            var dataFields = this.Context.Request.Form;
            Dictionary<string, string> conditionFormat = new Dictionary<string, string>();
            var responseObj = new Dictionary<string, string>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //20161101 要提高上限，否則物件較大時會產生例外
            serializer.MaxJsonLength = int.MaxValue;

            string responseMessage = String.Empty;
            try
            {
                if (dataFields == null || !dataFields.HasKeys()) { throw new Exception(); }

                var data = _rootRepo.QueryForDepartmentByEmpID(dataFields["EmployeeID"]);

                if (data == null) { throw new Exception("查無部門資料!"); }
                responseObj.Add("DepartmentID", data["DepartmentID"].ToString());
                responseObj.Add("DepartmentName", data["DepartmentName"].ToString());
            }
            catch (Exception ex)
            {
                responseObj.Add("ErrorMessage", ex.Message);
                responseObj.Add("Keys", String.Join(",", this.Context.Request.Form.AllKeys));
                _log.Error(responseObj);
                this.Context.Response.StatusCode = 500;
            }
            finally
            {
                responseMessage = serializer.Serialize(responseObj);
                this.Context.Response.Write(responseMessage);
            }
        }

        //草稿存檔 (新增/編輯)
        [WebMethod(enableSession: true)]
        public void SaveData()
        {
            //處理 request jsonData
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //20161101 要提高上限，否則物件較大時會產生例外
            serializer.MaxJsonLength = int.MaxValue;
            var jsonData = String.Empty;
            Context.Request.InputStream.Position = 0;

            using (var inputStream = new StreamReader(Context.Request.InputStream))
            {
                jsonData = inputStream.ReadToEnd();
            }
            var data = serializer.Deserialize<List<Dictionary<string, string>>>(jsonData);
            var model = dataMapping(data);
            var modelList = model.RinnaiForms;

            var overtimeModelList = model.RinnaiForms.Cast<OvertimeViewModel>();
            var punchDate = overtimeModelList.First().StartDateTime.Value;
            //var HolidayType = "2";
            // var holiday = _smartRepo.QueryForHoliday(punchDate.ToString("yyyyMMdd"),overtimeModelList.First().EmployeeID_FK);
            //var HolidayType = _smartRepo.QueryForHoliday(punchDate.ToString("yyyyMMdd"),overtimeModelList.First().EmployeeID_FK);
            //var dutyWorkData = _smartRepo.QueryForDutyWork(punchDate.ToString("yyyyMMdd"), overtimeModelList.First().EmployeeID_FK);

            var responseObj = new Dictionary<string, string>();
            //var result = new Dictionary<string, string>() { { "Message", null } };
            try
            {
                //驗證規則
                responseObj = OvertimeRulesSavedata(model);

                // 有異常 message
                if (!String.IsNullOrWhiteSpace(responseObj["ErrorMessage"])) { throw new Exception("Do Not Pass Overtime Validation!"); }

                if (String.IsNullOrWhiteSpace(model.SignDocID))
                {
                    _overtimeRepo.CreateData(modelList);
                    model.SignDocID = model.RinnaiForms.First().SignDocID_FK;
                }
                else
                {
                    _overtimeRepo.EditData(modelList);
                }
                responseObj.Add("SignDocID", model.SignDocID);

                if (DateTime.Compare(punchDate.Date, DateTime.Now.Date) >= 0) //20161230 修正若加班日 >= 今日才通知主管
                {
                    //send mail
                    var adminMail = System.Web.Configuration.WebConfigurationManager.AppSettings["AdminMail"];
                    MailInfo info = new MailInfo()
                    {
                        DomainPattern = ConfigUtils.ParsePageSetting("Pattern")["DomainPattern"],
                        AddresseeTemp = System.Web.Configuration.WebConfigurationManager.AppSettings["MailTemplate"],
                        Subject = String.Format("系統提醒!簽核單號 : {0} 已經產生,請儘速檢視!", model.SignDocID),
                    };
                    info.CC.Add(String.Format(info.AddresseeTemp, User.Identity.Name));

                    // 主管簽核/會簽 mail address
                    model.ChiefDeptIDs.Distinct().All(deptID =>
                    {
                        var deptData = _rootRepo.QueryForDepartmentByDeptID(deptID.ToString());
                        var empData = _rootRepo.QueryForEmployeeByEmpID(deptData["ChiefID_FK"].ToString());
                        var chiefAD = empData["ADAccount"].ToString();
                        ////取得當前簽核部門資料，即支援單位主管部門(上層)Start
                        //var upperDeptData = _pwfRepo.FindUpperDeptData(deptData["DepartmentID"].ToString());
                        //var chiefDeptID = upperDeptData[deptData["ChiefID_FK"].ToString()];
                        //var upperdeptData = _rootRepo.QueryForDepartmentByDeptID(chiefDeptID.ToString());
                        //var empData_upper = _rootRepo.QueryForEmployeeByEmpID(upperdeptData["ChiefID_FK"].ToString());
                        //var UpperChiefAD = empData_upper["ADAccount"].ToString();
                        //info.To += String.Format(info.AddresseeTemp, chiefAD) + "," + String.Format(info.AddresseeTemp, UpperChiefAD) + ",";
                        ////取得當前簽核部門資料，即支援單位主管部門(上層)End

                        info.To += String.Format(info.AddresseeTemp, chiefAD) + ",";
                        return true;
                    });

                    info.To = info.To.TrimEnd(new char[] { ',' });
                    info.Body.Append(MailTools.BodyToTable(String.Format("系統提醒!簽核單號 : {0} 明細如下 : <br /> ", model.SignDocID)));
                    info.Body.Append(MailTools.BodyToTable(constructTable(model.RinnaiForms.Cast<OvertimeViewModel>())));
                    info.Body.Append("此件為系統發送，請勿回覆!<br /><br />");
                    if (!String.IsNullOrWhiteSpace(responseObj["message_alert"]))
                    {
                        var stringAlert = responseObj["message_alert"].Replace("\r", "<br />");
                        info.Body.Append("<span style='color:red;font-size:large;'>" + stringAlert + "</span><br />");
                    }
                    if (PublicRepository.CurrentWorkflowMode == Enums.WorkflowTypeEnum.RELEASE)
                        new Mailer(info).SendMail();
                    _log.Trace(String.Format("MailTo : {0}\r\ncc : {1}\r\nTitle : {2}\r\nContent : {3}\r\n", info.To, String.Join(",", info.CC), info.Subject, info.Body));
                }
            }
            catch (Exception ex)
            {
                responseObj.Add("ErrorMessage", ex.Message);
            }
            finally
            {
                //設定輸出格式為json格式
                Context.Response.ContentType = "application/json";
                Context.Response.Write(serializer.Serialize(responseObj));
            }
        }

        //送出
        [WebMethod(enableSession: true)]
        public void ApplyForm()
        {
            var empID = String.Empty;
            if (Authentication.LoginList.ContainsKey(User.Identity.Name))
            {
                empID = Authentication.LoginList[User.Identity.Name]["EmployeeID"].ToString();
            }

            //處理 request jsonData
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //20161101 要提高上限，否則物件較大時會產生例外
            serializer.MaxJsonLength = int.MaxValue;

            var jsonData = String.Empty;
            Context.Request.InputStream.Position = 0;
            //取得連入 HTTP 實體主體的內容。 取得表單送來的資料
            using (var inputStream = new StreamReader(Context.Request.InputStream))
            {
                jsonData = inputStream.ReadToEnd();
            }
            //可序列化為List的字典物件型別
            var data = serializer.Deserialize<List<Dictionary<string, string>>>(jsonData);
            var model = dataMapping(data);

            Dictionary<string, string> responseObj = null;
            var responseMessage = String.Empty;
            try
            {
                //驗證加班規則
                responseObj = OvertimeRulesValidate(model);

                var overtimeModelList = model.RinnaiForms.Cast<OvertimeViewModel>();
                var punchDate = overtimeModelList.First().StartDateTime.Value;
                //var overtimeList =
                var creatDate = _rootRepo.Find_CreatDate(model.SignDocID);
                //overtimeList.Single(overtime =>
                //{
                //    creatDate = overtime["CreatDate"].ToDateTimeFormateString("");
                //    return true;
                //});
                // 有異常 message
                if (!String.IsNullOrWhiteSpace(responseObj["ErrorMessage"])) { throw new Exception("Do Not Pass Overtime Validation!"); }
                model.EmployeeID_FK = empID;
                model.Modifier = User.Identity.Name;
                _overtimeRepo.SubmitData(model);
                responseObj.Add("Message", "簽核已送出!");

                MailInfo info = new MailInfo()
                {
                    AddresseeTemp = System.Web.Configuration.WebConfigurationManager.AppSettings["MailTemplate"],
                    Subject = String.Format("系統提醒!簽核單號 : {0} 已經送達!", model.SignDocID),
                    DomainPattern = ConfigUtils.ParsePageSetting("Pattern")["DomainPattern"],
                };

                // 主管簽核/會簽 mail
                model.ChiefDeptIDs.Distinct().All(deptID =>
                {
                    var deptData = _rootRepo.QueryForDepartmentByDeptID(deptID.ToString());
                    var empData = _rootRepo.QueryForEmployeeByEmpID(deptData["ChiefID_FK"].ToString());
                    var chiefAD = empData["ADAccount"].ToString();
                    info.To += String.Format(info.AddresseeTemp, chiefAD) + ",";

                    return true;
                });
                var portalDomain = ConfigUtils.ParsePageSetting("Domain")["Portal"];
                //20170217 修正若加班日不是 >= 今日，通知主管此為後補單據
                if (DateTime.Compare(punchDate.Date, creatDate.ToDateTime().Date) >= 0)
                {
                    var body =
                        MailTools.BodyToTable(
                            String.Format(
                                @"系統提醒!簽核單號 : {0} 已經送達，請儘速檢視!<br /><a href='{1}/Area/Sign/ProcessWorkflowList.aspx?queryText={0}'>連結</a> <br />此件為系統發送，請勿回覆!",
                                model.SignDocID, portalDomain));

                    info.Body.Append(body);
                }
                else
                {
                    var body =
                        MailTools.BodyToTable(
                            String.Format(
                                @"<span style='color:red;font-size:medium;'>此加班單為事後補單!</span><br />系統提醒!簽核單號 : {0} 已經送達，請儘速檢視!<br /><a href='{1}/Area/Sign/ProcessWorkflowList.aspx?queryText={0}'>連結</a> <br />此件為系統發送，請勿回覆!",
                                model.SignDocID, portalDomain));

                    info.Body.Append(body);
                }
                info.CC = new List<string>() { String.Format(info.AddresseeTemp, User.Identity.Name) };
                info.To = info.To.TrimEnd(new char[] { ',' });
                if (PublicRepository.CurrentWorkflowMode == Enums.WorkflowTypeEnum.RELEASE)
                    new Mailer(info).SendMail();

                _log.Trace(String.Format("MailTo : {0}\r\ncc : {1}\r\nTitle : {2}\r\nContent : {3}\r\n", info.To, String.Join(",", info.CC), info.Subject, info.Body));

                responseMessage = serializer.Serialize(responseObj);
            }
            catch (Exception ex)
            {
                responseObj.Add("ActualException", ex.Message);
                responseMessage = serializer.Serialize(responseObj);
                _log.Error(responseMessage);
                this.Context.Response.StatusCode = 500;
            }
            finally
            {
                //設定輸出格式為json格式
                this.Context.Response.ContentType = "application/json";
                this.Context.Response.Write(responseMessage);
            }
        }

        public Dictionary<string, string> OvertimeRulesSavedata(ProcessWorkflowViewModel model)
        {
            var result = new Dictionary<string, string>() { { "ErrorMessage", null }, { "message_alert", null } };
            var responseObj = new Dictionary<string, string>();
            var overtimeModelList = model.RinnaiForms.Cast<OvertimeViewModel>();
            var punchDate = overtimeModelList.First().StartDateTime.Value;

            #region 判斷一， 20170217 add事後申請警示訊息

            if (model.SignDocID.ToString() == "")
            {
                if (DateTime.Compare(punchDate.Date, DateTime.Now.Date) < 0)
                {
                    result["message"] = String.Format("如因工作需求有加班必要，應需事先提出申請!");
                }
            }
            else
            {
                //punchDate.Date => 加班日 ，creatDate => 申請日
                var creatDate = _rootRepo.Find_CreatDate(model.SignDocID);
                if (DateTime.Compare(punchDate.Date, creatDate.ToDateTime().Date) < 0)
                {
                    result["message"] = String.Format("如因工作需求有加班必要，應需事先提出申請!");
                }
            }

            #endregion 判斷一， 20170217 add事後申請警示訊息

            overtimeModelList.All(overtime =>
            {
                var verifyModel = new OvertimeVerifyViewModel() { empID = overtime.EmployeeID_FK };

                #region 判斷二， 查詢志元的員工個人行事曆檔

                // 班別、假日查詢
                var dutyWorkData = _smartRepo.QueryForDutyWork(overtime.StartDateTime.Value.ToDateTimeFormateString("yyyyMMdd"), overtime.EmployeeID_FK);

                if (dutyWorkData == null)
                    throw new Exception(String.Format("於志元系統中查無員工編號{0}的員工個人行事曆檔!", overtime.EmployeeID_FK));
                else
                {
                    var empData = _smartRepo.QueryForEmployee(overtime.EmployeeID_FK);
                    var holiday = _smartRepo.QueryForHoliday(overtime.StartDateTime.Value.ToDateTimeFormateString("yyyyMMdd"));
                    if (empData == null)
                        throw new Exception(String.Format("於簽核系統中查無員工編號{0}的員工個人基本資料檔!", overtime.EmployeeID_FK));
                    //verifyModel.workType = empData["WORKTYPE"].ToString();
                    //overtime.IsHoliday = holiday != null;

                    #region 三，判斷加班日是否為假日以及國定假日(會增加提示訊息)

                    //班別
                    verifyModel.workType = dutyWorkData["WORKTYPE"].ToString();
                    //加班日是否為假日
                    overtime.IsHoliday = (dutyWorkData["HOLIDAY"].ToString() == "Y");
                    //加班日是否為國定假日
                    if (dutyWorkData["H_TYPE"].ToString() == "2")
                        result["message"] = String.Format("工號:{0} 國定假日因工作急迫性需求才加班，請以「全日」加班申請。", overtime.EmployeeID_FK);

                    #endregion 三，判斷加班日是否為假日以及國定假日(會增加提示訊息)
                }

                #endregion 判斷二， 查詢志元的員工個人行事曆檔

                // 加班的時間
                verifyModel.startTime = DateTime.ParseExact(overtime.StartDateTime.Value.ToString("HHmmss"), "HHmmss", null);
                verifyModel.endTime = DateTime.ParseExact(overtime.EndDateTime.Value.ToString("HHmmss"), "HHmmss", null);

                // 班別規定工作時間
                var workTimeData = _smartRepo.QueryForWorkTime(verifyModel.workType);
                // 班別規定上午上班時間
                verifyModel.onWorkTime = DateTime.ParseExact(workTimeData["MORNINGTIME"].ToString(), "HHmm", null);
                // 班別規定上午下班時間
                verifyModel.noonTime = DateTime.ParseExact(workTimeData["NOONTIME"].ToString(), "HHmm", null);
                // 班別規定下午上班時間
                verifyModel.afternoonWorkTime = DateTime.ParseExact(workTimeData["afternoontime"].ToString(), "HHmm", null);
                // 班別規定下班時間
                verifyModel.offWorkTime = DateTime.ParseExact(workTimeData["OFFWORKTIME"].ToString(), "HHmm", null);
                // 班別規定加班時間
                verifyModel.addWorkTime = DateTime.ParseExact(workTimeData["ADDWORKTIME"].ToString(), "HHmm", null);

                bool isException = false;
                var range = new TimeSpan();

                #region 判斷四 #003 判斷是否已經申請過加班並且請領方式不同

                //判斷是否已經申請過加班並且請領方式不同
                string beforePayTypeValue = _rootRepo.CheckHasOverTimeThenPayTypeSame(overtime);
                if (!string.IsNullOrEmpty(beforePayTypeValue))
                    throw new Exception(String.Format(@"提醒 {0} 工號:{1} 請領方式不相同! 時間於{2} 已有請領方式為'{3}'!", overtime.EmployeeName, overtime.EmployeeID_FK, overtime.StartDateTime, beforePayTypeValue));

                #endregion 判斷四 #003 判斷是否已經申請過加班並且請領方式不同

                #region 判斷五 判斷是否加班超過40H

                //判斷是否加班超過40H
                var OverWork_Form = _rootRepo.QueryOverWork(overtime.StartDateTime.Value.ToDateTimeFormateString("yyyyMMdd"), overtime.EmployeeID_FK);
                //20170310 add增加含國定假日加班總時數欄位
                //var AllOverWork_Form = _rootRepo.QueryAllOverWork(overtime.StartDateTime.Value.ToDateTimeFormateString("yyyyMMdd"), overtime.EmployeeID_FK);
                var OverWork_smart = _smartRepo.QueryOverWork(overtime.StartDateTime.Value.ToDateTimeFormateString("yyyyMMdd"), overtime.EmployeeID_FK);
                //20170310 add增加含國定假日加班總時數欄位
                var AllOverWork_smart = _smartRepo.QueryAllOverWork(overtime.StartDateTime.Value.ToDateTimeFormateString("yyyyMMdd"), overtime.EmployeeID_FK);
                var TotalOver = double.Parse(OverWork_Form["total"].ToString()) + double.Parse(OverWork_smart["totalH"].ToString());
                if (TotalOver >= 40)
                    result["message_alert"] += String.Format("提醒 工號:{0} 本月您申請加班已累積達40H(不含本次)，法規上限為46H!\r", overtime.EmployeeID_FK);

                #endregion 判斷五 判斷是否加班超過40H

                //Start儲存草稿時計算時數Start
                if (dutyWorkData != null)
                {
                    if (!overtime.IsHoliday)
                    {
                        //平常日
                        if (verifyModel.startTime <= verifyModel.onWorkTime && verifyModel.endTime <= verifyModel.onWorkTime)
                        {
                            range = RangeComputor.BeforeOnWorkRange(verifyModel);
                        }
                        else if (verifyModel.startTime >= verifyModel.addWorkTime && verifyModel.endTime >= verifyModel.addWorkTime || "G".Equals(verifyModel.workType))
                        {
                            if ("G".Equals(verifyModel.workType))
                            {
                                range = RangeComputor.OvertimeRangeG(verifyModel);
                                isException = true;
                            }
                            else
                            {
                                //檢查申請時差是否大於1小時
                                range = RangeComputor.AfterAddWorkRange(verifyModel);
                            }
                        }
                    }
                    else
                    {
                        //假日
                        //判斷加班區間
                        if (verifyModel.startTime < verifyModel.afternoonWorkTime)
                        {
                            range = RangeComputor.MorningRange(verifyModel);
                        }
                        else if (verifyModel.startTime >= verifyModel.afternoonWorkTime && verifyModel.startTime <= verifyModel.addWorkTime)
                        {
                            range = RangeComputor.AfternoonRange(verifyModel);
                        }
                        else if (verifyModel.startTime <= verifyModel.addWorkTime)
                        {
                            range = RangeComputor.OvertimeRange(verifyModel);
                        }
                    }

                    overtime.TotalHours = isException ? range.Hours + ((int)(range.Minutes / 30)) * 0.5 : range.Hours;
                }
                //End儲存草稿時計算時數End

                return true;
            });
            return result;
        }

        public Dictionary<string, string> OvertimeRulesValidate(ProcessWorkflowViewModel model)
        {
            var result = new Dictionary<string, string>() { { "ErrorMessage", null }, { "message_alert", null } };

            var overtimeModelList = model.RinnaiForms.Cast<OvertimeViewModel>();
            //var overtimeList = _rootRepo.QueryForOvertimeFormDataBySignDocID(signDocID);
            var punchDate = overtimeModelList.First().StartDateTime.Value;
            string signDocID = overtimeModelList.First().SignDocID_FK.ToString();
            var overtimeList = _rootRepo.QueryForOvertimeFormDataBySignDocID(signDocID);
            // 根據加班日期找出已送出加班單列表，排除草稿
            var alreadyApplyData = _rootRepo.QueryForOvertimeFormDataByDate(punchDate, "1", "5");
            //依照日期找出該日所有加班人員
            var dailyPunchData = _smartRepo.QueryForDailyOnOffByDate(punchDate.ToString("yyyyMMdd"));
            //判斷智源是否存有當日打卡紀錄
            if (dailyPunchData == null)
            {
                result["ErrorMessage"] = "於志元系統中查無當日刷卡紀錄的刷卡記錄!";
                return result;
            }
            overtimeList.All(overtime =>
            {
                //判斷志元是否存有員工打卡紀錄
                var empPunchData = dailyPunchData.SingleOrDefault(punch => punch["EmployeCD"].Equals(overtime["EmployeeID_FK"].ToString()));
                if (empPunchData == null)
                {
                    result["ErrorMessage"] = String.Format("於志元系統中查無{0}的刷卡記錄!", overtime["EmployeeID_FK"].ToString());
                    return false;
                }

                // 判斷是否已經申請過當日加班
                if (alreadyApplyData != null)
                {
                    // 用員工編號過濾，已送出加班單列表
                    var empFilert = alreadyApplyData.Where(row => row["EmployeeID_FK"].ToString().Equals(overtime["EmployeeID_FK"].ToString()));
                    if (empFilert != null)
                    {
                        if (empFilert.Any(punched => !overtime["PayTypeKey"].ToString().Equals(punched["PayTypeKey"].ToString())))
                        {
                            result["ErrorMessage"] = String.Format("工號: {0},加班支付類型不同於前次紀錄!", overtime["EmployeeID_FK"].ToString());
                            return false;
                        }

                        if (empFilert.Any(punched =>
                            DateTimeCompare.IsCovered(
                                punched["StartDateTime"].ToString().ToDateTime(), punched["EndDateTime"].ToString().ToDateTime(),
                                overtime["StartDateTime"].ToString().ToDateTime(), overtime["EndDateTime"].ToString().ToDateTime())))
                        {
                            result["ErrorMessage"] = String.Format("工號: {0},加班申請時間與前次加班申請時間相同!", overtime["EmployeeID_FK"].ToString());
                            return false;
                        }
                    }
                }

                var verifyModel = new OvertimeVerifyViewModel() { empID = overtime["EmployeeID_FK"].ToString() };
                // 班別、假日查詢
                var dutyWorkData = _smartRepo.QueryForDutyWork(overtime["StartDateTime"].ToDateTimeFormateString("yyyyMMdd"), overtime["EmployeeID_FK"].ToString());
                //志元員工資料查詢
                var empData = _smartRepo.QueryForEmployee(overtime["EmployeeID_FK"].ToString());
                //志元假日別定義查詢
                var holiday = _smartRepo.QueryForHoliday(overtime["StartDateTime"].ToDateTimeFormateString("yyyyMMdd"));
                if (dutyWorkData == null)
                {
                    if (empData == null)
                    {
                        result["ErrorMessage"] = String.Format("於志元系統中查無員工編號{0}的員工個人行事曆檔、員工個人基本資料檔!", overtime["EmployeeID_FK"].ToString());
                        return false;
                    }

                    verifyModel.workType = empData["WORKTYPE"].ToString();
                    overtime["IsHoliday"] = holiday != null;
                }
                else
                {
                    verifyModel.workType = dutyWorkData["WORKTYPE"].ToString();
                    overtime["IsHoliday"] = "Y".Equals(dutyWorkData["HOLIDAY"].ToString());
                }

                //if (DateTime.Compare(overtime["StartDateTime"].ToDateTimeFormateString("yyyyMMdd").ToDateTime(), overtime["EndDateTime"].ToDateTimeFormateString("yyyyMMdd").ToDateTime()) != 0)
                //{
                //    result["ErrorMessage"] = String.Format("員工編號{0}的加班申請日期需為相同日期!", overtime["EmployeeID_FK"].ToString());
                //    return false;
                //}

                // 申請時間
                verifyModel.startTime = DateTime.ParseExact(overtime["StartDateTime"].ToDateTimeFormateString("HHmmss"), "HHmmss", null);
                verifyModel.endTime = DateTime.ParseExact(overtime["EndDateTime"].ToDateTimeFormateString("HHmmss"), "HHmmss", null);

                // 規定工作時間
                var workTimeData = _smartRepo.QueryForWorkTime(verifyModel.workType);
                // 規定上午上班時間
                verifyModel.onWorkTime = DateTime.ParseExact(workTimeData["MORNINGTIME"].ToString(), "HHmm", null);
                // 規定上午下班時間
                verifyModel.noonTime = DateTime.ParseExact(workTimeData["NOONTIME"].ToString(), "HHmm", null);
                // 規定下午上班時間
                verifyModel.afternoonWorkTime = DateTime.ParseExact(workTimeData["afternoontime"].ToString(), "HHmm", null);
                // 規定下班時間
                verifyModel.offWorkTime = DateTime.ParseExact(workTimeData["OFFWORKTIME"].ToString(), "HHmm", null);
                // 規定加班時間
                verifyModel.addWorkTime = DateTime.ParseExact(workTimeData["ADDWORKTIME"].ToString(), "HHmm", null);

                // 實際打卡時間
                verifyModel.punchIN = DateTime.ParseExact(empPunchData["begintime"].ToString(), "HHmm", null);
                verifyModel.punchOUT = DateTime.ParseExact(empPunchData["endtime"].ToString(), "HHmm", null);

                //20161222 一例一休-例假日卡控(20161223實施)
                var HolidayType = dutyWorkData["H_TYPE"].ToString();

                bool isException = false;
                var range = new TimeSpan();
                //根據班別(WorkType)判斷打卡時間是否為假日
                if (overtime["IsHoliday"].ToString() == "False")
                {
                    // 工作日加班
                    //20170301 add業務、服務、配送員平日不得申請加班卡控。
                    if (empData["JOBFAMILY"].ToString() == "202" || empData["JOBFAMILY"].ToString() == "203" || empData["JOBFAMILY"].ToString() == "204")
                    {
                        result["ErrorMessage"] = String.Format("員工編號{0}，業務、服務、配送員平日不得申請加班!", overtime["EmployeeID_FK"].ToString());
                        return false;
                    }

                    // 根據申請時間判斷是否為上班時間
                    //if (verifyModel.startTime >= verifyModel.onWorkTime && verifyModel.startTime <= verifyModel.offWorkTime)
                    if (verifyModel.startTime > verifyModel.onWorkTime && verifyModel.startTime < verifyModel.offWorkTime)
                    {
                        result["ErrorMessage"] = String.Format("員工編號{0}，加班(起)落於規定上班時間區間!", overtime["EmployeeID_FK"].ToString());
                        return false;
                    }
                    //if (verifyModel.endTime <= verifyModel.offWorkTime && verifyModel.endTime >= verifyModel.onWorkTime)
                    if (verifyModel.endTime < verifyModel.offWorkTime && verifyModel.endTime > verifyModel.onWorkTime)
                    {
                        result["ErrorMessage"] = String.Format("員工編號{0}，加班(迄)落於規定上班時間區間!", overtime["EmployeeID_FK"].ToString());
                        return false;
                    }

                    if (verifyModel.startTime <= verifyModel.onWorkTime && verifyModel.endTime <= verifyModel.onWorkTime)
                    {
                        // 根據打卡時間、規定上班時間 判斷申請時間是否正確
                        if (verifyModel.startTime < verifyModel.punchIN || verifyModel.endTime < verifyModel.punchIN)
                        {
                            result["ErrorMessage"] = String.Format("員工編號{0}，加班申請時間與加班起算時間或打卡時間不符!", verifyModel.empID);
                            return false;
                        }
                        else
                        {
                            //20170425 add
                            var range_punch = new TimeSpan();
                            range_punch = verifyModel.startTime - verifyModel.punchIN;
                            if (range_punch >= new TimeSpan(1, 0, 0))
                            {
                                result["ErrorMessage"] = String.Format("員工編號{0}，加班申請時間有少申報，請修正!", verifyModel.empID);
                                return false;
                            }
                        }

                        range = RangeComputor.BeforeOnWorkRange(verifyModel);
                    }
                    else if (verifyModel.startTime >= verifyModel.addWorkTime && verifyModel.endTime >= verifyModel.addWorkTime || "G".Equals(verifyModel.workType))
                    {
                        // 根據打卡時間、加班起算時間 判斷申請時間是否正確
                        if (verifyModel.startTime > verifyModel.punchOUT || verifyModel.endTime > verifyModel.punchOUT)
                        {
                            result["ErrorMessage"] = String.Format("員工編號{0}，加班申請時間與加班起算時間或打卡時間不符!", verifyModel.empID);
                            return false;
                        }
                        else
                        {
                            //20170425 add
                            var range_punch = new TimeSpan();
                            range_punch = verifyModel.punchOUT - verifyModel.endTime;
                            if (range_punch >= new TimeSpan(1, 0, 0))
                            {
                                result["ErrorMessage"] = String.Format("員工編號{0}，加班申請時間有少申報，請修正!", verifyModel.empID);
                                return false;
                            }
                        }

                        if ("G".Equals(verifyModel.workType))
                        {
                            range = RangeComputor.OvertimeRangeG(verifyModel);
                            isException = true;
                        }
                        else
                        {
                            //檢查申請時差是否大於1小時
                            range = RangeComputor.AfterAddWorkRange(verifyModel);
                        }
                    }

                    //檢查申請時差是否大於1小時
                    if (range < new TimeSpan(1, 0, 0) && !isException)
                    {
                        result["ErrorMessage"] = String.Format("員工編號{0}，加班工時未達1小時標準!", verifyModel.empID);
                        return false;
                    }
                }
                else
                {
                    // #0005 若要取消假日不能送單於這裡註解  20170804改成只有小遇可送by 俊晨
                    if (HolidayType == "0")
                    {
                        result["ErrorMessage"] = String.Format("員工編號{0}的加班申請日期為例假日，依規定不得申請加班!", overtime["EmployeeID_FK"].ToString());
                        return false;
                    }

                    //20170303 add同舊版加班單：業務、服務、配送員假日加班要在規定上班時間內
                    if (empData["JOBFAMILY"].ToString() == "202" || empData["JOBFAMILY"].ToString() == "203" || empData["JOBFAMILY"].ToString() == "204")
                    {
                        if (verifyModel.startTime < verifyModel.onWorkTime || verifyModel.startTime > verifyModel.offWorkTime)
                        {
                            result["ErrorMessage"] = String.Format("員工編號{0}，業務、服務、配送員假日加班(起)應落於規定上班時間區間!", overtime["EmployeeID_FK"].ToString());
                            return false;
                        }

                        if (verifyModel.endTime > verifyModel.offWorkTime || verifyModel.endTime < verifyModel.onWorkTime)
                        {
                            result["ErrorMessage"] = String.Format("員工編號{0}，業務、服務、配送員假日加班(迄)應落於規定下班時間區間!", overtime["EmployeeID_FK"].ToString());
                            return false;
                        }
                    }
                    if (HolidayType != "1" && HolidayType != "2") //國定假日與休息日不卡控刷卡時間
                    {
                        // 根據打卡時間判斷申請時間是否正確
                        if (verifyModel.startTime < verifyModel.punchIN || verifyModel.startTime > verifyModel.punchOUT)
                        {
                            result["ErrorMessage"] = String.Format("員工編號{0}，加班申請時間與打卡時間不符!", overtime["EmployeeID_FK"].ToString());
                            return false;
                        }
                        if (verifyModel.endTime > verifyModel.punchOUT || verifyModel.endTime < verifyModel.punchIN)
                        {
                            result["ErrorMessage"] = String.Format("員工編號{0}，加班申請時間與打卡時間不符!", overtime["EmployeeID_FK"].ToString());
                            return false;
                        }
                    }

                    //判斷加班區間
                    if (verifyModel.startTime < verifyModel.afternoonWorkTime)
                    {
                        range = RangeComputor.MorningRange(verifyModel);
                    }
                    else if (verifyModel.startTime >= verifyModel.afternoonWorkTime && verifyModel.startTime <= verifyModel.addWorkTime)
                    {
                        range = RangeComputor.AfternoonRange(verifyModel);
                    }
                    else if (verifyModel.startTime <= verifyModel.addWorkTime)
                    {
                        range = RangeComputor.OvertimeRange(verifyModel);
                    }

                    //檢查時差是否大於1小時
                    if (range < new TimeSpan(1, 0, 0))
                    {
                        result["ErrorMessage"] = String.Format("員工編號{0}，假日加班工時未達1小時!請洽，03-3322101#6804-6806。", overtime["EmployeeID_FK"].ToString());
                        return false;
                    }
                }

                var TotalHours = isException ? range.Hours + ((int)(range.Minutes / 30)) * 0.5 : range.Hours;

                //一例一休增加start
                if (HolidayType == "2")//20161227 國定假日依照員工規定工作時間卡控時間區間(8H or 8H up)
                {
                    if (TotalHours < 8)
                    {
                        result["ErrorMessage"] = String.Format("員工編號{0}，國定假日因工作急迫性需求才加班，請以「全日」加班申請。", overtime["EmployeeID_FK"].ToString());
                        return false;
                    }
                    else
                    {
                        if (TotalHours == 8)
                        {
                            if (verifyModel.startTime != verifyModel.onWorkTime || verifyModel.endTime != verifyModel.offWorkTime)
                            {
                                result["ErrorMessage"] = String.Format("員工編號{0}，國定假日加班8小時加班區間應為：" + verifyModel.onWorkTime + "~" + verifyModel.offWorkTime + " !", overtime["EmployeeID_FK"].ToString());
                                return false;
                            }
                        }
                        //20161228 修正判斷
                        //if (overtime.TotalHours == 12)
                        //{
                        //    if (verifyModel.startTime != verifyModel.onWorkTime || verifyModel.endTime != verifyModel.addWorkTime.AddHours(4))
                        //    {
                        //        result["ErrorMessage"] = String.Format("員工編號{0}，國定假日加班12小時加班區間應為：" + verifyModel.onWorkTime + "~" + verifyModel.addWorkTime.AddHours(4) + " !", overtime.EmployeeID_FK);
                        //        return false;
                        //    }
                        //}
                        if (TotalHours > 8 && TotalHours <= 12)
                        {
                            //if (verifyModel.startTime != verifyModel.onWorkTime || (verifyModel.endTime != verifyModel.addWorkTime.AddHours(1) && verifyModel.endTime != verifyModel.addWorkTime.AddHours(2) && verifyModel.endTime != verifyModel.addWorkTime.AddHours(3) && verifyModel.endTime != verifyModel.addWorkTime.AddHours(4)))
                            //{
                            //    result["ErrorMessage"] = String.Format("員工編號{0}，國定假日加班12小時加班應在：\r" + verifyModel.onWorkTime + "~" + verifyModel.addWorkTime.AddHours(1) + "\r" + verifyModel.onWorkTime + "~" + verifyModel.addWorkTime.AddHours(2) + "\r" + verifyModel.onWorkTime + "~" + verifyModel.addWorkTime.AddHours(3) + "\r" + verifyModel.onWorkTime + "~" + verifyModel.addWorkTime.AddHours(4) + "\r區間內!", overtime.EmployeeID_FK);
                            //    return false;
                            //}
                            // 根據打卡時間、加班起算時間 判斷申請時間是否正確
                            if (verifyModel.startTime > verifyModel.punchOUT || verifyModel.endTime > verifyModel.punchOUT)
                            {
                                result["ErrorMessage"] = String.Format("員工編號{0}，加班申請時間與加班起算時間或打卡時間不符!", verifyModel.empID);
                                return false;
                            }
                            else
                            {
                                //20170425 add
                                var range_punch = new TimeSpan();
                                range_punch = verifyModel.punchOUT - verifyModel.endTime;
                                if (range_punch >= new TimeSpan(1, 0, 0))
                                {
                                    result["ErrorMessage"] = String.Format("員工編號{0}，加班申請時間有少申報，請修正!", verifyModel.empID);
                                    return false;
                                }
                            }

                            if (verifyModel.startTime != verifyModel.onWorkTime)
                            {
                                result["ErrorMessage"] = String.Format("員工編號{0}，國定假日加班申請起始時間應為：" + verifyModel.onWorkTime + " !", overtime["EmployeeID_FK"].ToString());
                                return false;
                            }
                            else
                            {
                                if (verifyModel.endTime != verifyModel.addWorkTime.AddHours(1) && verifyModel.endTime != verifyModel.addWorkTime.AddHours(2) && verifyModel.endTime != verifyModel.addWorkTime.AddHours(3) && verifyModel.endTime != verifyModel.addWorkTime.AddHours(4))
                                {
                                    result["ErrorMessage"] = String.Format("員工編號{0}，國定假日加班時間(迄)應為：\r" + verifyModel.addWorkTime.AddHours(1) + "、\r" + verifyModel.addWorkTime.AddHours(2) + "、\r" + verifyModel.addWorkTime.AddHours(3) + "、\r" + verifyModel.addWorkTime.AddHours(4) + " !", overtime["EmployeeID_FK"].ToString());
                                    return false;
                                }
                            }
                        }
                        if (TotalHours > 12)
                        {
                            result["ErrorMessage"] = String.Format("員工編號{0}，國定假日加班最多12小時!", verifyModel.empID);
                            return false;
                        }
                    }
                }
                else if (HolidayType == "1")//20161227 休息日日依照員工規定工作時間卡控時間區間(4H or 8H or 12H)
                {
                    if (TotalHours != 4 && TotalHours != 8 && TotalHours != 12)
                    {
                        result["ErrorMessage"] = String.Format("員工編號{0}，休息日加班時數只能在4小時、8小時或12小時區間內!", overtime["EmployeeID_FK"].ToString());
                        return false;
                    }
                    else
                    {
                        if (TotalHours == 4)
                        {
                            if (verifyModel.startTime != verifyModel.onWorkTime || verifyModel.endTime != verifyModel.noonTime)
                            {
                                result["ErrorMessage"] = String.Format("員工編號{0}，休息日加班4小時加班區間應為：" + verifyModel.onWorkTime + "~" + verifyModel.noonTime + " !", overtime["EmployeeID_FK"].ToString());
                                return false;
                            }
                        }
                        if (TotalHours == 8)
                        {
                            if (verifyModel.startTime != verifyModel.onWorkTime || verifyModel.endTime != verifyModel.offWorkTime)
                            {
                                result["ErrorMessage"] = String.Format("員工編號{0}，休息日加班8小時加班區間應為：" + verifyModel.onWorkTime + "~" + verifyModel.offWorkTime + " !", overtime["EmployeeID_FK"].ToString());
                                return false;
                            }
                        }
                        if (TotalHours == 12)
                        {
                            if (verifyModel.startTime != verifyModel.onWorkTime || verifyModel.endTime != verifyModel.addWorkTime.AddHours(4))
                            {
                                result["ErrorMessage"] = String.Format("員工編號{0}，休息日加班12小時加班區間應為：" + verifyModel.onWorkTime + "~" + verifyModel.addWorkTime.AddHours(4) + " !", overtime["EmployeeID_FK"].ToString());
                                return false;
                            }
                        }
                    }
                }
                //一例一休增加end

                return true;
            });

            return result;
        }

        /// <summary>
        /// 讀取加班明細
        /// </summary>
        [WebMethod(enableSession: true)]
        public void QueryOvertimeData()
        {
            //設定輸出格式為json格式
            this.Context.Response.ContentType = "application/json";
            string signDocID = string.Empty;
            //post
            signDocID = this.Context.Request.Form["SignDocID"];
            var tableData = _overtimeRepo.QueryOvertimeFormData(signDocID);
            //var jsonStrData = JsonConvert.SerializeObject(tableData);

            //OverDetailsModel jsonModel = JsonConvert.DeserializeObject<OverDetailsModel>(jsonStrData);
            //foreach (var item in jsonModel.Data)
            //{
            //    var test = item.DepartmentName;

            //}
            var listData = dataMapping(tableData);
            var dicHtml = constructRows(listData);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //20161101 要提高上限，否則物件較大時會產生例外
            serializer.MaxJsonLength = int.MaxValue;

            //輸出json格式
            this.Context.Response.Write(serializer.Serialize(dicHtml));
        }

        private string constructOptions(Dictionary<string, string> types, string value = null)
        {
            string result = String.Empty;
            foreach (var type in types)
            {
                if (type.Key.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    result += String.Format(@"<option value='{0}' selected>{1}</option>", type.Key, type.Value);
                }
                else
                {
                    result += String.Format(@"<option value='{0}'>{1}</option>", type.Key, type.Value);
                }
            }

            return result;
        }

        private string constructEmployeeOptions(DataTable table, string value = null)
        {
            StringBuilder result = new StringBuilder("<option value=''>請選擇</option>");
            foreach (DataRow item in table.Rows)
            {
                result.AppendFormat(
        @"<option value='{0}' nationalType='{1}' {2}>{3} {4}</option>",
                item["EmployeeID"], item["NationalType"],
                item["EmployeeID"].ToString().Equals(value, StringComparison.OrdinalIgnoreCase) ? "selected" : "",
                item["EmployeeID"], item["EmployeeName"]);
            }

            return result.ToString();
        }

        private Dictionary<string, string> constructRows(List<OvertimeViewModel> data)
        {
            var result = new Dictionary<string, string>();
            int seq = 0;
            foreach (var datum in data)
            {
                seq++;
                result.Add(seq.ToString(), String.Format(
        @"  <tr>
	<td class='text-center check vertical-middle'><input type='checkbox' /></td>
	<td class='employeeID' hidden='true'><input type='text' class='form-control input-sm' value='{0}' disabled></td>
	<td class='employeeName'><select class='form-control'>{1}</select></td>
	<td class='startDateTime'><div class='date' id='datetimepicker'><input type='text' class='form-control' data-date-format='YYYY-MM-DD HH:mm:00' value='{2}'></div></td>
	<td class='endDateTime'><div class='date' id='datetimepicker'><input type='text' class='form-control' data-date-format='YYYY-MM-DD HH:mm:00' value='{3}'></div></td>
	<td class='realendDateTime'><label >{12}</td>
    <td class='supportDept'><select class='form-control'> {4} </select></td>
	<td class='payType'><select class='form-control'> {5} </select></td>
	<td class='mealOrder'><select class='form-control'> {6} </select></td>
	<td class='note'><input type='text' class='form-control input-sm' value='{7}'></td>
	<input class='sn' type='hidden' value={8} />
	<input class='nationalType' type='hidden' value='{9}' />
	<input class='departmentID' type='hidden' value='{10}' />
	<input class='departmentName' type='hidden' value='{11}' />
	</tr>",
          datum.EmployeeID_FK,
          constructEmployeeOptions(_rootRepo.QueryForEmployee(), datum.EmployeeID_FK),
          datum.StartDateTime.FormatDatetimeNullable(),
          datum.EndDateTime.FormatDatetimeNullable(),
          constructOptions(_rootRepo.GetDepartment(), datum.SupportDeptID_FK),
          constructOptions(_overtimeRepo.CreatePayType(), datum.PayTypeKey),
          constructOptions(_overtimeRepo.CreateMealOrderType(), datum.MealOrderKey),
          datum.Note,
          datum.SN,
          datum.NationType,
          datum.DepartmentID_FK,
          datum.DepartmentName,
          datum.RealEndDateTime
        ));
            }
            return result;
        }

        private DataTable constructTable(IEnumerable<OvertimeViewModel> data)
        {
            var result = new DataTable();
            result.Columns.AddRange(new DataColumn[]
			{
				new DataColumn("員工編號"),
				new DataColumn("員工姓名"),
				new DataColumn("加班起"),
				new DataColumn("加班迄"),
                //20170221 add 經理指示mail也要看到法定加班時數
                new DataColumn("法定加班時數<br>(上限46H、不含本次)"),
                //20170310 add 經理指示mail也要看到本月加班累計時數
                new DataColumn("本月加班累計時數<br>(不含本次)"),
				new DataColumn("支援單位"),
				new DataColumn("報酬類型"),
				new DataColumn("餐別"),
				new DataColumn("加班原因")
			});

            foreach (var datum in data)
            {
                var OverWork_Form = _rootRepo.QueryOverWork(datum.StartDateTime.Value.ToDateTimeFormateString("yyyyMMdd"), datum.EmployeeID_FK);
                var OverWork_smart = _smartRepo.QueryOverWork(datum.StartDateTime.Value.ToDateTimeFormateString("yyyyMMdd"), datum.EmployeeID_FK);
                //20170310 add增加含國定假日加班總時數欄位
                var AllOverWork_Form = _rootRepo.QueryAllOverWork(datum.StartDateTime.Value.ToDateTimeFormateString("yyyyMMdd"), datum.EmployeeID_FK);
                var AllOverWork_smart = _smartRepo.QueryAllOverWork(datum.StartDateTime.Value.ToDateTimeFormateString("yyyyMMdd"), datum.EmployeeID_FK);
                var TotalOver = double.Parse(OverWork_Form["total"].ToString()) + double.Parse(OverWork_smart["totalH"].ToString());
                var TotalAllOver = double.Parse(AllOverWork_Form["total"].ToString()) + double.Parse(AllOverWork_smart["totalH"].ToString());
                result.Rows.Add(
                    datum.EmployeeID_FK,
                    _rootRepo.QueryForEmployeeByEmpID(datum.EmployeeID_FK)["EmployeeName"].ToString(),
                    datum.StartDateTime.FormatDatetimeNullable(),
                    datum.EndDateTime.FormatDatetimeNullable(),
                    TotalOver,
                    TotalAllOver,
                    datum.SupportDeptName,
                    datum.PayTypeValue,
                    datum.MealOrderValue,
                    datum.Note);
            }
            return result;
        }

        private ProcessWorkflowViewModel dataMapping(List<Dictionary<string, string>> data)
        {
            var currentDateTime = DateTime.Now;
            var status = 2; //待簽核

            var result = new ProcessWorkflowViewModel()
            {
                SendDate = DateTime.Now,
                FinalStatus = status,
                CreateDate = currentDateTime,
            };
            foreach (var datum in data.Where(row => row.Count == 1))
            {
                if (datum.ContainsKey("applyID")) { result.EmployeeID_FK = datum["applyID"]; }
                if (datum.ContainsKey("signDocID")) { result.SignDocID = datum["signDocID"]; }
                if (datum.ContainsKey("currentSignLevelDeptName")) { result.CurrentSignLevelDeptName = datum["currentSignLevelDeptName"]; }
                if (datum.ContainsKey("formSeries")) { result.FormSeries = datum["formSeries"]; }
                if (datum.ContainsKey("currentSignLevelDeptID"))
                {
                    result.CurrentSignLevelDeptID_FK = datum["currentSignLevelDeptID"];
                    result.ChiefDeptIDs.Add(datum["currentSignLevelDeptID"]);
                    //_rootRepo = RepositoryFactory.CreateRootRepo();
                    var deptData = _rootRepo.QueryForDepartmentByDeptID(datum["currentSignLevelDeptID"]);
                    result.ChiefIDs.Add(deptData["ChiefID_FK"].ToString());
                }
            }

            #region 取出簽核規則檔

            //根據表單系列決定 SignType Data
            var signTypeData = _rootRepo.QueryForSignTypeDataBySeries(result.FormSeries);
            if (signTypeData == null) { return null; }
            //簽核規則ID
            result.RuleID_FK = signTypeData["SignID_FK"].ToString();
            //簽核規則理所對應的表單ID
            result.FormID_FK = Int32.Parse(signTypeData["FormID"].ToString());

            #endregion 取出簽核規則檔

            foreach (var datum in data.Where(row => row.Count > 1))
            {
                result.RinnaiForms.Add(new OvertimeViewModel()
                {
                    SN = Int32.Parse(datum["sn"]),
                    SignDocID_FK = !String.IsNullOrWhiteSpace(result.SignDocID) ? result.SignDocID : null,
                    FormID_FK = result.FormID_FK, //加班單
                    ApplyID_FK = datum.ContainsKey("applyID") ? datum["applyID"] : null,
                    ApplyDateTime = currentDateTime,
                    EmployeeID_FK = datum["employeeID"],
                    EmployeeName = datum["employeeName"],
                    StartDateTime = datum["startDateTime"].ToDateTimeNullable(),
                    EndDateTime = datum["endDateTime"].ToDateTimeNullable(),
                    SupportDeptID_FK = datum["supportDeptID_FK"],
                    SupportDeptName = datum["supportDeptName"],
                    PayTypeKey = datum["payTypeKey"],
                    PayTypeValue = datum["payTypeValue"],
                    MealOrderKey = datum["mealOrderKey"],
                    MealOrderValue = datum["mealOrderValue"],
                    Note = datum["note"],
                    //RealEndDateTime = datum["RealEndDateTime1"],
                    Creator = User.Identity.Name,
                    CreateDate = currentDateTime,
                    Modifier = User.Identity.Name,
                    ModifyDate = currentDateTime,
                    NationType = datum["nationalType"],
                    RuleID_FK = result.RuleID_FK,
                    DepartmentID_FK = datum["departmentID"],
                    DepartmentName = datum["departmentName"],
                    FormSeries = result.FormSeries
                });
                result.ChiefDeptIDs.Add(datum["departmentID"]);
                var deptData = _rootRepo.QueryForDepartmentByDeptID(datum["departmentID"]);
                result.ChiefIDs.Add(deptData["ChiefID_FK"].ToString());
            }

            return result;
        }

        private List<OvertimeViewModel> dataMapping(DataTable dtTable)
        {
            if (dtTable == null) { return null; }
            var result = new List<OvertimeViewModel>();
            foreach (DataRow row in dtTable.Rows)
            {
                result.Add(new OvertimeViewModel()
                {
                    SN = Int32.Parse(row["SN"].ToString()),
                    EmployeeID_FK = row["EmployeeID"].ToString(),
                    EmployeeName = row["EmployeeName"].ToString(),
                    DepartmentID_FK = row["DepartmentID"].ToString(),
                    DepartmentName = row["DepartmentName"].ToString(),
                    StartDateTime = row["StartDateTime"].ToString().ToDateTimeNullable(),
                    EndDateTime = row["EndDateTime"].ToString().ToDateTimeNullable(),
                    SupportDeptID_FK = row["SupportDeptID"].ToString(),
                    SupportDeptName = row["SupportDeptName"].ToString(),
                    PayTypeKey = row["PayTypeKey"].ToString(),
                    MealOrderKey = row["MealOrderKey"].ToString(),
                    NationType = row["NationalType"].ToString(),
                    Note = row["Note"].ToString(),
                    RealEndDateTime = row["RealEndDateTime1"].ToString(),
                });
            }

            return result;
        }
    }
}