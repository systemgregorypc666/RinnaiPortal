using DBTools;
using RinnaiPortal.ViewModel.Sign.Forms;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.Tools;
using RinnaiPortal.Interface;
using RinnaiPortal.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using RinnaiPortal.FactoryMethod;

namespace RinnaiPortal.Repository.Sign.Forms
{
    public class OvertimeRepository : ISignRepository, IForms
    {
        private DB _dc { get; set; }
        private string status { get; set; }
        private DateTime currentDateTime { get; set; }
        private RootRepository _rootRepo { get; set; }
        private ProcessWorkflowRepository _pwfRepo { get; set; }
        private NLog.Logger _log { get; set; }
        public OvertimeRepository(DB dc, RootRepository rootRepo, ProcessWorkflowRepository pwfRepo)
        {
            _dc = dc;

            //草稿
            status = "1";
            currentDateTime = DateTime.Now;
            _rootRepo = rootRepo;
            _pwfRepo = pwfRepo;
            _log = NLog.LogManager.GetCurrentClassLogger();
        }


        //建立請領方式資料     
        public Dictionary<string, string> CreatePayType()
        {
            return new Dictionary<string, string>()
			{
				{"","請選擇"},
				{"overtimeLeave","換休"},
				{"overtimePay","加班費"},
			};
        }

        //建立訂餐資料
        public Dictionary<string, string> CreateMealOrderType()
        {
            return new Dictionary<string, string>()
			{
				{"","請選擇"},
				{"none","不訂餐"},
				{"Carnivore","葷食"},
				{"Vegan","素食"},
			};
        }

        //建立預設資料
        public List<OvertimeViewModel> GetDefaultData(Dictionary<string, string> model)
        {
            //新增單筆，給予預設值
            if (model.ContainsKey("IsAddRow"))
            {
                var defaultResult = new List<OvertimeViewModel>();
                var empData = _rootRepo.QueryForEmployeeByEmpID(model["NewRowEmpID"]);
                var deptData = _rootRepo.QueryForDepartmentByDeptID(empData["DepartmentID_FK"].ToString());
                var overtime = new OvertimeViewModel()
                {
                    StartDateTime = model["DefaultStartDateTime"].ToDateTimeNullable(),
                    EndDateTime = model["DefaultEndDateTime"].ToDateTimeNullable(),
                    //SupportDeptID_FK = model["DefaultSupportDeptID"],
                    //SupportDeptName = model["DefaultSupportDeptName"],
                    SupportDeptID_FK = deptData["DepartmentID"].ToString(),
                    SupportDeptName = deptData["DepartmentName"].ToString(),
                    Note = model["DefaultNote"],
                    PayTypeKey = model["DefaultPayTypeKey"],
                    PayTypeValue = model["DefaultPayTypeValue"],
                    MealOrderKey = model["DefaultMealOrderKey"],
                    MealOrderValue = model["DefaultMealOrderValue"],
                };

                if (model.ContainsKey("NewRowEmpID"))
                {
                    //var empData = _rootRepo.QueryForEmployeeByEmpID(model["NewRowEmpID"]);
                    if (empData == null) { throw new Exception("查無人員編號!"); }
                    //var deptData = _rootRepo.QueryForDepartmentByDeptID(empData["DepartmentID_FK"].ToString());
                    if (deptData == null) { throw new Exception("查無人員對應部門編號!"); }

                    overtime.EmployeeID_FK = empData["EmployeeID"].ToString();
                    overtime.EmployeeName = empData["EmployeeName"].ToString();
                    overtime.NationType = empData["NationalType"].ToString();
                    overtime.DepartmentID_FK = deptData["DepartmentID"].ToString();
                    overtime.DepartmentName = deptData["DepartmentName"].ToString();
                    overtime.SupportDeptID_FK = deptData["DepartmentID"].ToString();
                    overtime.SupportDeptName = deptData["DepartmentName"].ToString();
                }

                defaultResult.Add(overtime);

                return defaultResult;
            }

            //新增多筆，將資料帶出
            //var strSQL =@"select * from employee where Disabled = 'False' and DepartmentID_FK = @DepartmentID_FK";
            var strSQL = @"select * from employee emp
                        left join Department dept on emp.DepartmentID_FK=dept.DepartmentID
                        where emp.Disabled = 'False' and DepartmentID_FK = @DepartmentID_FK";
            var result = _dc.QueryForDataRows(strSQL, new Conditions() { { "@DepartmentID_FK", model["DefaultDeptID"] } });
            return result.Select(data => new OvertimeViewModel()
            {
                EmployeeID_FK = data["EmployeeID"].ToString(),
                EmployeeName = data["EmployeeName"].ToString(),
                StartDateTime = model["DefaultStartDateTime"].ToDateTimeNullable(),
                EndDateTime = model["DefaultEndDateTime"].ToDateTimeNullable(),
                //SupportDeptID_FK = model["DefaultSupportDeptID"],
                //SupportDeptName = model["DefaultSupportDeptName"],
                SupportDeptID_FK = data["DepartmentID_FK"].ToString(),
                SupportDeptName = data["DepartmentName"].ToString(),
                Note = model["DefaultNote"],
                PayTypeKey = model["DefaultPayTypeKey"],
                PayTypeValue = model["DefaultPayTypeValue"],
                MealOrderKey = model["DefaultMealOrderKey"],
                MealOrderValue = model["DefaultMealOrderValue"],
                NationType = data["NationalType"].ToString(),
                DepartmentID_FK = model["DefaultDeptID"],
                DepartmentName = model["DefaultDeptName"]
            }).ToList();
        }

        //新增草稿
        public void CreateData(List<RinnaiForms> modelList)
        {
            var overtimeList = modelList.Cast<OvertimeViewModel>();
            var vmModel = overtimeList.First();
            vmModel.SignDocID_FK = GetSeqIDUtils.GetSignDocID(vmModel.Creator, "OT");

            if (String.IsNullOrWhiteSpace(vmModel.SignDocID_FK))
            {
                throw new Exception("系統忙碌中請稍候再試");
            }

            //取得簽核人員部門資料
            var deptData = _rootRepo.QueryForDepartmentByDeptID(vmModel.SupportDeptID_FK);
            if (deptData == null)
            {
                throw new Exception("查無簽核人員所屬部門資料!");
            }

            //取得當前簽核部門資料，即支援單位主管部門
            var upperDeptDate = _pwfRepo.FindUpperDeptData(vmModel.SupportDeptID_FK);
            var chiefDeptID = upperDeptDate[deptData["ChiefID_FK"].ToString()];

            //判斷 FlowRule
            if (!_pwfRepo.IsFollowFlowRule(vmModel.RuleID_FK, chiefDeptID))
            {
                throw new Exception("不符合簽核規則，無法新增!");
            }

            var status = 1;
            var procedureData = _rootRepo.QueryForSignProcedureBySignDocID(vmModel.SignDocID_FK);
            var remainder = procedureData != null ? Int32.Parse(procedureData["SignLevel"].ToString()) : -1;
            var manipulationConditions = new List<MultiConditions>();
            var dic = new Conditions()
			{
				{"@SignDocID", vmModel.SignDocID_FK},
				{"@FormID_FK", vmModel.FormID_FK},
				{"@EmployeeID_FK", vmModel.ApplyID_FK},
				{"@CurrentSignLevelDeptID_FK", chiefDeptID},
				{"@FinalStatus", status},
				{"@remainder", remainder},
				{"@Creator", vmModel.Creator},
				{"@CreateDate", currentDateTime},
			};
            //產生簽核主表資料
            var strSQL = _dc.ConstructInsertDML("signform_main", dic);
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            //列表細項資料設定
            overtimeList.All(overtime =>
            {
                //根據加班員工ID 設定加班單部門ID
                var employeeData = _rootRepo.QueryForEmployeeByEmpID(overtime.EmployeeID_FK);
                overtime.DepartmentID_FK = employeeData != null ? employeeData["DepartmentID_FK"].ToString() : String.Empty;
                //設定簽核編號
                overtime.SignDocID_FK = vmModel.SignDocID_FK;
                return true;
            });

            dic = new Conditions()
			{
				{"@SignDocID_FK", vmModel.SignDocID_FK},
				{"@FormID_FK", vmModel.FormID_FK},
				{"@EmployeeID_FK", vmModel.ApplyID_FK},
				{"@CurrentSignLevelDeptID_FK", chiefDeptID},
				{"@FinalStatus", status},
				{"@remainder", remainder},
				{"@Creator_Main", vmModel.Creator},
				{"@CreateDate_Main", currentDateTime},
				{"@LogDatetime", DateTime.Now},
			};
            //加入 log
            strSQL = _dc.ConstructInsertDML("signform_log", dic);
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });


            int seq = 0;
            foreach (var overtime in overtimeList)
            {
                seq++;
                dic = new Conditions()
				{
					{"@FormID_FK", overtime.FormID_FK},
					{String.Format("@SignDocID_FK{0}", seq), overtime.SignDocID_FK},
					{"@ApplyID_FK", overtime.ApplyID_FK},
					{"@EmployeeID_FK", overtime.EmployeeID_FK},
					{"@DepartmentID_FK", overtime.DepartmentID_FK},
					{"@StartDateTime", overtime.StartDateTime},
					{"@EndDateTime", overtime.EndDateTime},
					{"@SupportDeptID_FK", overtime.SupportDeptID_FK},
					{"@PayTypeKey", overtime.PayTypeKey},
					{"@MealOrderKey", overtime.MealOrderKey},
					{"@AutoInsert", false},
					{"@Note", overtime.Note},
                    {"@TotalHours", overtime.TotalHours },
					{"@Creator",overtime.Creator},
					{"@CreateDate",overtime.CreateDate},
				};
                //新增 OvertimeForm 資料
                strSQL = _dc.ConstructInsertDML("OvertimeForm", dic);
                manipulationConditions.Add(new MultiConditions() { { String.Format(strSQL, seq), dic } });
            }

            try
            {
                if (!_dc.ExecuteMultAndCheck(manipulationConditions)) { throw new Exception("新增加班單失敗!"); }
            }
            catch (Exception ex)
            {
                throw new Exception("新增加班單失敗!" + ex.Message);
            }
            finally
            {
                // log each SQL Pair
                manipulationConditions.ForEach(item =>
                {
                    var sql = item.Single().Key;
                    var valuePair = String.Join("\r\n", item.Single().Value.Select(x => x.Key + " : " + x.Value));
                    _log.Debug(String.Concat("\r\n", "Create Data Method: \r\n", sql, "\r\n", valuePair, "\r\n"));
                });
            }
        }

        //編輯草稿
        public void EditData(List<RinnaiForms> modelList)
        {
            var overtimeList = modelList.Cast<OvertimeViewModel>().ToList();
            var vmModel = overtimeList.First();
            var orgSignMainData = GetWorkflowData(vmModel.SignDocID_FK);

            var procedureData = _rootRepo.QueryForSignProcedureBySignDocID(vmModel.SignDocID_FK);
            var remainder = procedureData != null ? Int32.Parse(procedureData["SignLevel"].ToString()) : -1;
            // 取得簽核人員部門資料
            var deptData = _rootRepo.QueryForDepartmentByDeptID(vmModel.SupportDeptID_FK);
            if (deptData == null) { throw new Exception("查無簽核人員所屬部門資料!"); }

            //取得當前簽核部門資料，即支援單位主管部門
            var upperDeptDate = _pwfRepo.FindUpperDeptData(vmModel.SupportDeptID_FK);
            var chiefDeptID = upperDeptDate[deptData["ChiefID_FK"].ToString()];

            //判斷 FlowRule
            if (!_pwfRepo.IsFollowFlowRule(vmModel.RuleID_FK, chiefDeptID)) { throw new Exception("不符合簽核規則，無法編輯!"); }

            var status = 1;
            //根據支援部門更新簽核資料
            var manipulationConditions = new List<MultiConditions>();
            var strSQL = string.Empty;

            strSQL =
@"UPDATE signform_main 
SET    employeeid_fk = @EmployeeID_FK, 
	   currentsignleveldeptid_fk = @CurrentSignLevelDeptID_FK, 
	   finalstatus = @FinalStatus, 
	   remainder = @remainder,
	   modifier = @Modifier, 
	   modifydate = @ModifyDate 
WHERE  signdocid = @SignDocID ";
            var dic = new Conditions()
			{
				{"@SignDocID", vmModel.SignDocID_FK},
				{"@EmployeeID_FK", vmModel.ApplyID_FK},
				{"@CurrentSignLevelDeptID_FK", chiefDeptID},
				{"@FinalStatus", status},
				{"@remainder", remainder},
				{"@Modifier", vmModel.Modifier},
				{"@ModifyDate", currentDateTime},
			};
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            //列表細項資料設定
            overtimeList.ForEach(overtime =>
            {
                //根據加班員工ID 設定加班單部門ID
                var employeeData = _rootRepo.QueryForEmployeeByEmpID(overtime.EmployeeID_FK);
                overtime.DepartmentID_FK = employeeData != null ? employeeData["DepartmentID_FK"].ToString() : String.Empty;
                overtime.Modifier = vmModel.Modifier;
            });

            dic = new Conditions()
			{
				{"@SignDocID_FK", vmModel.SignDocID_FK},
				{"@FormID_FK", vmModel.FormID_FK},
				{"@EmployeeID_FK", vmModel.ApplyID_FK},
				{"@CurrentSignLevelDeptID_FK", chiefDeptID},
				{"@FinalStatus", status},
				{"@remainder", remainder},
				{"@Modifier_Main", vmModel.Modifier},
				{"@ModifyDate_Main", currentDateTime},
				{"@LogDatetime", DateTime.Now},
			};
            //加入 log
            strSQL = _dc.ConstructInsertDML("signform_log", dic);
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });


            //先刪除資料，再更新資料，再寫入新資料

            int seq = 0;
            // 組成 in 條件，找出需要被刪除的資料
            var snSQL = String.Join(",", overtimeList.Select((s, i) => "@value" + i.ToString()));
            strSQL = String.Format(@"Select * from OvertimeForm Where SN not in ({0}) And SignDocID_FK=@SignDocID_FK", snSQL);
            dic = new Conditions() { { "@SignDocID_FK", vmModel.SignDocID_FK } };
            foreach (var item in overtimeList)
            {
                dic.Add(String.Format(@"value{0}", seq), item.SN.ToString());
                seq++;
            }
            var overtimeDataCount = _dc.QueryForRowsCount(strSQL, dic);
            if (overtimeDataCount > 0)
            {
                // Delete OvertimeForm data
                strSQL = String.Format(@"Delete OvertimeForm Where SN not in ({0}) And SignDocID_FK=@SignDocID_FK", snSQL);
                manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });
            }

            //更新 OvertimeForm 資料
            seq = 0;
            string signDocID = String.Empty;
            strSQL =
@"UPDATE overtimeform 
SET    applydatetime = @ApplyDateTime, 
	   employeeid_fk = @EmployeeID_FK, 
	   departmentid_fk = @DepartmentID_FK, 
	   startdatetime = @StartDateTime, 
	   enddatetime = @EndDateTime, 
	   supportdeptid_fk = @SupportDeptID_FK, 
	   paytypekey = @PayTypeKey, 
	   mealorderkey = @MealOrderKey, 
	   autoInsert = @AutoInsert,
	   note = @Note, 
       totalHours = @TotalHours, 
	   modifier = @Modifier, 
	   modifydate = @ModifyDate 
WHERE  sn=@SN{0} 
AND    signdocid_fk=@SignDocID_FK";
            foreach (var overtime in overtimeList)
            {
                if (overtime.SN == 0) { continue; }
                seq++;
                var sn = String.Format("@SN{0}", seq);
                dic = new Conditions()
				{
					{sn, overtime.SN},
					{"@SignDocID_FK", overtime.SignDocID_FK},
					{"@ApplyDateTime", (string)null},
					{"@EmployeeID_FK", overtime.EmployeeID_FK},
					{"@DepartmentID_FK", overtime.DepartmentID_FK},
					{"@StartDateTime", overtime.StartDateTime},
					{"@EndDateTime", overtime.EndDateTime},
					{"@SupportDeptID_FK", overtime.SupportDeptID_FK},
					{"@PayTypeKey", overtime.PayTypeKey},
					{"@MealOrderKey", overtime.MealOrderKey},
					{"@Note", overtime.Note},
                    {"@TotalHours", overtime.TotalHours },
					{"@Modifier", overtime.Modifier},
					{"@ModifyDate",currentDateTime},
					{"@AutoInsert", false},
				};
                manipulationConditions.Add(new MultiConditions() { { String.Format(strSQL, seq), dic } });
            }

            var orgOvertimeTable = QueryOvertimeFormData(vmModel.SignDocID_FK);
            var orgOvertimeList = new List<int>();
            foreach (DataRow row in orgOvertimeTable.Rows)
            {
                orgOvertimeList.Add(Int32.Parse(row["SN"].ToString()));
            }
            //把原本的加班單與要編輯的加班單作交集 取出 序號 後續判斷哪寫資料要寫入用
            var innerSN = overtimeList.Join(orgOvertimeList, x => x.SN, y => y, (x, y) => y).ToArray();
            var insertFormList = overtimeList.Where(x => !innerSN.Contains(x.SN)).ToList();

            seq = 0;
            foreach (var data in insertFormList)
            {
                seq++;
                dic = new Conditions()
				{
					{"@FormID_FK", data.FormID_FK},
					{String.Format("@SignDocID_FK{0}", seq), data.SignDocID_FK },
					{"@ApplyID_FK", data.ApplyID_FK},
					{"@EmployeeID_FK", data.EmployeeID_FK},
					{"@DepartmentID_FK", data.DepartmentID_FK},
					{"@StartDateTime", data.StartDateTime},
					{"@EndDateTime", data.EndDateTime},
					{"@SupportDeptID_FK", data.SupportDeptID_FK},
					{"@PayTypeKey", data.PayTypeKey},
					{"@MealOrderKey", data.MealOrderKey},
					{"@Note", data.Note},
					{"@Creator",data.Modifier},
					{"@CreateDate",currentDateTime},
                    {"@AutoInsert", false},
				};
                //新增 OvertimeForm 資料
                strSQL = _dc.ConstructInsertDML("overtimeform", dic);
                manipulationConditions.Add(new MultiConditions() { { String.Format(strSQL, seq), dic } });
            }

            try
            {
                if (!_dc.ExecuteMultAndCheck(manipulationConditions)) { throw new Exception("編輯加班單失敗!"); }
            }
            catch (Exception ex)
            {
                throw new Exception("編輯加班單失敗!" + ex.Message);
            }
            finally
            {
                // log each SQL Pair
                manipulationConditions.ForEach(item =>
                {
                    var sql = item.Single().Key;
                    var valuePair = String.Join("\r\n", item.Single().Value.Select(x => x.Key + " : " + x.Value));
                    _log.Debug(String.Concat("\r\n", "Edit Data Method: \r\n", sql, "\r\n", valuePair, "\r\n"));
                });
            }
        }

        public bool IsExistChiefID(string SignDocID_FK, string ChiefID_FK)
        {
            var conditions = new Conditions() { { "@SignDocID_FK", SignDocID_FK },
                                                { "@ChiefID_FK", ChiefID_FK }};
            var sql = @"Select * from SignForm_Detail where SignDocID_FK = @SignDocID_FK and ChiefID_FK=@ChiefID_FK";
            var rows = _dc.QueryForRowsCount(sql, conditions);

            return (rows > 0) ? true : false;
        }

        //送出簽核
        public void SubmitData(ProcessWorkflowViewModel mainModel)
        {
            var procedureData = _rootRepo.QueryForSignProcedureBySignDocID(mainModel.SignDocID);
            var remainder = procedureData != null ? Int32.Parse(procedureData["SignLevel"].ToString()) : -1;

            var chiefDeptData = _pwfRepo.FindUpperDeptData(mainModel.CurrentSignLevelDeptID_FK);
            var chiefDeptID = chiefDeptData.Values.SingleOrDefault();
            if (chiefDeptID == null) { throw new Exception("查尋上層部門發生異常!"); }

            //判斷 FlowRule
            if (!_pwfRepo.IsFollowFlowRule(mainModel.RuleID_FK, chiefDeptID)) { throw new Exception("不符合簽核規則，無法送出簽核!"); }
            mainModel.SendDate = currentDateTime;
            mainModel.FinalStatus = 2;
            mainModel.Remainder = remainder;
            mainModel.ModifyDate = currentDateTime;

            //根據支援部門更新簽核資料
            var manipulationConditions = new List<MultiConditions>();
            var strSQL = string.Empty;

            strSQL =
@"UPDATE signform_main 
SET    senddate = @SendDate, 
	   finalstatus = @FinalStatus, 
	   remainder = @remainder,
	   modifier = @Modifier, 
	   modifydate = @ModifyDate 
WHERE  signdocid = @SignDocID ";
            var dic = new Conditions()
			{
				{"@SignDocID", mainModel.SignDocID},
				{"@SendDate", mainModel.SendDate},
				{"@FinalStatus", mainModel.FinalStatus},
				{"@remainder", mainModel.Remainder},
				{"@Modifier", mainModel.Modifier},
				{"@ModifyDate", mainModel.ModifyDate},
			};
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            dic = new Conditions()
				{
					{"@SignDocID_FK", mainModel.SignDocID},
					{"@FormID_FK", mainModel.FormID_FK},
					{"@EmployeeID_FK", mainModel.EmployeeID_FK},
					{"@SendDate", mainModel.SendDate},
					{"@CurrentSignLevelDeptID_FK", chiefDeptID},
					{"@FinalStatus", mainModel.FinalStatus},
					{"@Remainder", mainModel.Remainder},
					{"@Modifier_Main", mainModel.Modifier},
					{"@ModifyDate_Main", mainModel.ModifyDate},
					{"@LogDatetime", DateTime.Now},

				};
            //加入 log
            strSQL = _dc.ConstructInsertDML("signform_log", dic);
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            int seq = 0;
            //根據所屬單位產生會簽資料
            mainModel.RinnaiForms.Cast<OvertimeViewModel>().All(x =>
            {
                var employeeData = _rootRepo.QueryForEmployeeByEmpID(x.EmployeeID_FK);
                x.DepartmentID_FK = employeeData != null ? employeeData["DepartmentID_FK"].ToString() : String.Empty;
                return true;
            });

            //20170119 修正會簽主管短少問題Start
            var orgOvertimeTable = QueryOvertimeData(mainModel.SignDocID);
            foreach (DataRow row in orgOvertimeTable.Rows)
            {
                //orgOvertimeList.Add(Int32.Parse(row["SN"].ToString()));
                seq++;
                //var deptData = _rootRepo.QueryForDepartmentByDeptID(row["DepartmentID_FK"].ToString());
                var chiefID = row["ChiefID_FK"].ToString();
                dic = new Conditions()
				{
					{String.Format("@SignDocID_FK{0}", seq), mainModel.SignDocID},
					{"@ChiefID_FK", chiefID},
					{"@Status", mainModel.FinalStatus},
					{"@Creator", mainModel.Modifier},
					{"@CreateDate", currentDateTime},
				};

                //新增 SignForm_Detail 資料
                strSQL = _dc.ConstructInsertDML("signform_detail", dic);
                manipulationConditions.Add(new MultiConditions() { { String.Format(strSQL, seq), dic } });

                dic = new Conditions()
				{
					{String.Format("@DetailSignDocID_FK{0}", seq), mainModel.SignDocID},
					{"@ChiefID_FK", chiefID},
					{"@Status", mainModel.FinalStatus},
					{"@Creator_Detail", mainModel.Modifier},
					{"@CreateDate_Detail", currentDateTime},
					{"@LogDatetime", DateTime.Now},
				};
                //加入 log
                strSQL = _dc.ConstructInsertDML("signform_log", dic);
                manipulationConditions.Add(new MultiConditions() { { String.Format(strSQL, seq), dic } });

            }
            //20170119 修正會簽主管短少問題End

            //foreach (var data in mainModel.ChiefIDs.Distinct())
            //{
            //    seq++;
            //    var deptData = _rootRepo.QueryForDepartmentByDeptID(data);
            //    var chiefID = deptData != null ? deptData["ChiefID_FK"].ToString() : String.Empty;
            //    //if (!IsExistChiefID(mainModel.SignDocID, chiefID))//兼任主管不寫入重複signform_detail
            //    //{
            //        dic = new Conditions()
            //        {
            //            {String.Format("@SignDocID_FK{0}", seq), mainModel.SignDocID},
            //            {"@ChiefID_FK", data},
            //            {"@Status", mainModel.FinalStatus},
            //            {"@Creator", mainModel.Modifier},
            //            {"@CreateDate", currentDateTime},
            //        };

            //        //新增 SignForm_Detail 資料
            //        strSQL = _dc.ConstructInsertDML("signform_detail", dic);
            //        manipulationConditions.Add(new MultiConditions() { { String.Format(strSQL, seq), dic } });

            //        dic = new Conditions()
            //        {
            //            {String.Format("@DetailSignDocID_FK{0}", seq), mainModel.SignDocID},
            //            {"@ChiefID_FK", data},
            //            {"@Status", mainModel.FinalStatus},
            //            {"@Creator_Detail", mainModel.Modifier},
            //            {"@CreateDate_Detail", currentDateTime},
            //            {"@LogDatetime", DateTime.Now},
            //        };
            //        //加入 log
            //        strSQL = _dc.ConstructInsertDML("signform_log", dic);
            //        manipulationConditions.Add(new MultiConditions() { { String.Format(strSQL, seq), dic } });
            //    //}
            //};

            //判斷結薪日，當加班日小於結薪日手動入志元，大於結薪日自動入志元
            var limitData = _rootRepo.GetSalaryLimit();
            var limitDate = limitData.Count != 0 ? limitData["LimitDate"].ToString().ToDateTimeNullable() : (DateTime?)null;

            if (limitDate.HasValue)
            {
                var date = mainModel.RinnaiForms.Cast<OvertimeViewModel>().First().StartDateTime;
                if (date.HasValue)
                {
                    //加班時間大於等於結薪日，自動入志元
                    if (DateTime.Compare(limitDate.Value.Date, date.Value.Date) <= 0)
                    {
                        mainModel.RinnaiForms.ForEach(form => form.AutoInsert = true);
                    }
                }

            }

            var overtimeModel = mainModel.RinnaiForms.Cast<OvertimeViewModel>().First();
            //更新送出日期
            strSQL =
            @"UPDATE overtimeform 
            SET    applydatetime = @ApplyDateTime, 
	               autoinsert = @AutoInsert, 
	               isholiday = @IsHoliday
            WHERE  signdocid_fk = @SignDocID_FK ";

            dic = new Conditions()
			{
				{"@SignDocID_FK", mainModel.SignDocID},
				{"@ApplyDateTime", mainModel.SendDate},
				{"@AutoInsert", overtimeModel.AutoInsert},
				{"@IsHoliday", overtimeModel.IsHoliday },
				//{"@TotalHours", overtimeModel.TotalHours },
			};
            manipulationConditions.Add(new MultiConditions() { { String.Format(strSQL, seq), dic } });

            //            var overtimeList = mainModel.RinnaiForms.Cast<OvertimeViewModel>().ToList();
            //            //更新 OvertimeForm 資料
            //            seq = 0;
            //            string signDocID = String.Empty;
            //            strSQL =
            //                @"UPDATE overtimeform 
            //            SET    applydatetime = @ApplyDateTime, 
            //	               autoinsert = @AutoInsert, 
            //	               isholiday = @IsHoliday, 
            //	               totalhours = @TotalHours 
            //            WHERE  sn=@SN{0} 
            //            AND    signdocid_fk=@SignDocID_FK";

            //            foreach (var overtime in overtimeList)
            //            {
            //                if (overtime.SN == 0) { continue; }
            //                seq++;
            //                var sn = String.Format("@SN{0}", seq);
            //                dic = new Conditions()
            //                {
            //                    {sn, overtime.SN},
            //                    {"@SignDocID_FK", mainModel.SignDocID},
            //                    {"@ApplyDateTime", mainModel.SendDate},
            //                    {"@AutoInsert", overtime.AutoInsert},
            //                    {"@IsHoliday", overtime.IsHoliday },
            //                    {"@TotalHours", overtime.TotalHours },
            //                };
            //                manipulationConditions.Add(new MultiConditions() { { String.Format(strSQL, seq), dic } });
            //            }

            //seq = 0;
            //var overtimeModelList = mainModel.RinnaiForms.Cast<OvertimeViewModel>();
            //            overtimeModelList.All(overtime =>
            //            {
            //                seq++;
            //                //更新送出日期
            //                strSQL =
            //                @"UPDATE overtimeform 
            //            SET    applydatetime = @ApplyDateTime, 
            //	               autoinsert = @AutoInsert, 
            //	               isholiday = @IsHoliday, 
            //	               totalhours = @TotalHours 
            //            WHERE  signdocid_fk = @SignDocID_FK ";

            //                dic = new Conditions()
            //                {
            //                    {"@SignDocID_FK", mainModel.SignDocID},
            //                    {"@ApplyDateTime", mainModel.SendDate},
            //                    {"@AutoInsert", overtime.AutoInsert},
            //                    {"@IsHoliday", overtime.IsHoliday },
            //                    {"@TotalHours", overtime.TotalHours },
            //                };
            //                manipulationConditions.Add(new MultiConditions() { { String.Format(strSQL, seq), dic } });
            //                return true;
            //            });



            //var overtimeModel = mainModel.RinnaiForms.Cast<OvertimeViewModel>().First();


            try
            {
                if (!_dc.ExecuteMultAndCheck(manipulationConditions)) { throw new Exception("送出加班單失敗!"); }
            }
            catch (Exception ex)
            {
                throw new Exception("送出加班單失敗!" + ex.Message);
            }

        }

        //取得簽核主表資料
        public ProcessWorkflowViewModel GetWorkflowData(string signDocID)
        {
            ProcessWorkflowViewModel model = null;
            string strSQL = @"Select * from SignForm_Main where SignDocID = @SignDocID";
            var strCondition = new Conditions() { { "@SignDocID", signDocID } };

            var result = _dc.QueryForDataRow(strSQL, strCondition);
            if (result == null) { return null; }

            model = new ProcessWorkflowViewModel()
            {
                SignDocID = result["SignDocID"].ToString(),
                FormID_FK = Int32.Parse(result["FormID_FK"].ToString()),
                EmployeeID_FK = result["EmployeeID_FK"].ToString(),
                SendDate = !result["SendDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["SendDate"].ToString()) : (DateTime?)null,
                CurrentSignLevelDeptID_FK = result["CurrentSignLevelDeptID_FK"].ToString(),
                FinalStatus = Int32.Parse(result["FinalStatus"].ToString()),
                Creator = result["Creator"].ToString(),
                CreateDate = DateTime.Parse(result["CreateDate"].ToString()),
                Modifier = result["Modifier"].ToString(),
                ModifyDate = !result["ModifyDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ModifyDate"].ToString()) : (DateTime?)null,
                WorkflowDetailList = GetWorkflowDataDetail(result["SignDocID"].ToString())
            };

            return model;
        }

        //取得簽核明細資料
        public List<ProcessWorkflowDetailViewModel> GetWorkflowDataDetail(string signDocID_FK)
        {
            List<ProcessWorkflowDetailViewModel> model = null;
            string strSQL = @"Select * from SignForm_Detail where SignDocID_FK = @SignDocID_FK";
            var strCondition = new Conditions() { { "@SignDocID_FK", signDocID_FK } };

            var results = _dc.QueryForDataRows(strSQL, strCondition);

            if (results == null) { return null; }

            model = new List<ProcessWorkflowDetailViewModel>();
            foreach (var result in results)
            {
                model.Add(new ProcessWorkflowDetailViewModel()
                {
                    SN = Int32.Parse(result["SN"].ToString()),
                    ChiefID_FK = result["ChiefID_FK"].ToString(),
                    SignDocID_FK = result["SignDocID_FK"].ToString(),
                    Remark = result["Remark"].ToString(),
                    Status = Int32.Parse(result["Status"].ToString()),
                    Modifier = result["Modifier"].ToString(),
                    ModifyDate = !result["ModifyDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ModifyDate"].ToString()) : (DateTime?)null,
                });
            }

            return model;
        }

        //明細列表
        public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
        {
            string strSQL =
@"SELECT        
				otf.SN                 SN,
				otf.FormID_FK          FormID, 
				otf.SignDocID_FK       SignDocID, 
				otf.ApplyID_FK         ApplyID,
				otf.ApplyDateTime      ApplyDateTime,  
				otf.EmployeeID_FK      EmployeeID, 
				emp.EmployeeName       EmployeeName, 
				dept.DepartmentID      DepartmentID,
				dept.DepartmentName    DepartmentName, 
				otf.StartDateTime      StartDateTime, 
				otf.EndDateTime        EndDateTime, 
				supdept.DepartmentID   SupportDeptID,
				supdept.DepartmentName SupportDeptName, 
				otf.PayTypeKey         PayTypeKey,  
				otf.MealOrderKey       MealOrderKey, 
				otf.Note               Note,  
				emp.NationalType       NationalType
FROM            overtimeform otf 
LEFT OUTER JOIN employee emp 
ON              otf.employeeid_fk = emp.employeeid 
LEFT OUTER JOIN department dept 
ON              otf.departmentid_fk = dept.departmentid 
LEFT OUTER JOIN department supDept 
ON              otf.supportdeptid_fk = supdept.departmentid {0} {1}";


            //組排序SQL
            pParms.OrderField = "CreateDate".Equals(pParms.OrderField) ? "sn" : pParms.OrderField;
            string strOrder = String.Format(" order by {0} {1}", pParms.OrderField, pParms.Descending ? "Desc" : "Asc");

            string strConditions = " where SignDocID_FK = @SignDocID_FK";

            var paginationParms = new PaginationParms()
            {
                QueryString = String.Format(strSQL, strConditions, strOrder),
                QueryConditions = new Conditions() { { "@SignDocID_FK", slParms.SignDocID } },
                PageIndex = pParms.PageIndex,
                PageSize = pParms.PageSize
            };

            //根據 SQL取得　Pagination
            return _dc.QueryForPagination(paginationParms);
        }

        private string getOvertimeFormSQL()
        {
            var strSQL =
            @"select distinct 
AutoInsert,
SN,
type.FormType Type,
EmployeeID_FK EmployeeID,
emp.EmployeeName EmployeeName,
dept.DepartmentID DepartmentID,
dept.DepartmentName DepartmentName,
StartDateTime, 
EndDateTime, 
supDept.DepartmentID SupportDeptID, 
supDept.DepartmentName SupportDeptName, 
PayTypeKey, 
MealOrderKey,
emp.NationalType NationalType,
Note,
CAST(punch.DutyDate AS NVARCHAR(8)) +  CAST(punch.Begintime AS NVARCHAR(6)) + '00'  RealStartDateTime,
CAST(punch.DutyDate AS NVARCHAR(8)) +  CAST(punch.Endtime AS NVARCHAR(6)) + '00'  RealEndDateTime,
CAST(punch.DutyDate AS NVARCHAR(8)) +  CAST(punch.Endtime AS NVARCHAR(6))  RealEndDateTime1,
TotalHours,
ISNULL(DAILYON.totalH,0) totalH
from OvertimeForm overtime
left outer join SignType type on overtime.FormID_FK = type.FormID
left outer join Employee emp on overtime.EmployeeID_FK = emp.EmployeeID
left outer join Department dept on overtime.DepartmentID_FK = dept.DepartmentID
left outer join Department supDept on overtime.SupportDeptID_FK = supDept.DepartmentID
left outer join (select EmployeCD ,DutyDate, Begintime, Endtime from {0}.dbo.DailyOnOff where dutydate = @DutyDate) punch on overtime.EmployeeID_FK = punch.EmployeCD
left outer join (select EMPLOYECD,ISNULL(sum(OVERWORKHOURS),0)-ISNULL(sum(case when H_TYPE='2' and OVERWORKHOURS>=8 then 8 else 0 end),0) as totalH from {0}.dbo.DAILYON where dutydate between CASE WHEN (CONVERT(varchar(12) , @DutyDate, 111 ) > CONVERT(varchar(12) , convert(varchar(4),YEAR(DATEADD(MONTH,-0,@DutyDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@DutyDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@DutyDate)))+'20', 111 )) then convert(varchar(4),YEAR(DATEADD(MONTH,-0,@DutyDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@DutyDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@DutyDate)))+'21' else convert(varchar(4),YEAR(DATEADD(MONTH,-1,@DutyDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-1,@DutyDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-1,@DutyDate)))+'21' end and CASE WHEN (CONVERT(varchar(12) , @DutyDate, 111 ) > CONVERT(varchar(12) , convert(varchar(4),YEAR(DATEADD(MONTH,-0,@DutyDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@DutyDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@DutyDate)))+'20', 111 )) then convert(varchar(4),YEAR(DATEADD(MONTH,+1,@DutyDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,+1,@DutyDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,+1,@DutyDate)))+'20' else convert(varchar(4),YEAR(DATEADD(MONTH,-0,@DutyDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@DutyDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@DutyDate)))+'20' end group by EMPLOYECD) DAILYON on overtime.EmployeeID_FK = DAILYON.EMPLOYECD 
{1} {2}
";
            return strSQL;
        }

        public Pagination GetOvertimeListPagination(SignListParms slParms, PaggerParms pParms)
        {
            var strSQL = getOvertimeFormSQL();

            //組排序SQL
            string strOrder = " order by DepartmentID Asc ";
            string strConditions = " where SignDocID_FK = @SignDocID_FK";

            var orgData = _rootRepo.QueryForOvertimeFormDataBySignDocID(slParms.SignDocID);
            if (orgData == null) { return null; }
            var dutyDate = (DateTime?)orgData.FirstOrDefault()["StartDateTime"];

            var paginationParms = new PaginationParms()
            {
                QueryString = String.Format(strSQL, String.Concat(RepositoryFactory.SmartManConn["DataSource"], ".", RepositoryFactory.SmartManConn["Catelog"]), strConditions, strOrder),
                QueryConditions = new Conditions() { { "@DutyDate", dutyDate.HasValue ? dutyDate.Value.ToString("yyyyMMdd") : (string)null }, { "@SignDocID_FK", slParms.SignDocID } },
                PageIndex = pParms.PageIndex,
                PageSize = 5
            };
            return _dc.QueryForPagination(paginationParms);
        }

        /// <summary>
        /// DataTable轉成模型
        /// </summary>
        /// <param name="dtTable"></param>
        /// <returns></returns>
        public List<OvertimeViewModel> dataMapping(DataTable dtTable)
        {
            if (dtTable == null) { return null; }
            var result = new List<OvertimeViewModel>();
            foreach (DataRow row in dtTable.Rows)
            {
                result.Add(new OvertimeViewModel()
                {
                    AutoInsert = Convert.ToBoolean(row["AutoInsert"]),
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

        /// <summary>
        /// 取出加班單明細
        /// </summary>
        /// <param name="docID"></param>
        /// <returns></returns>
        public DataTable QueryOvertimeFormData(string docID)
        {
            if (String.IsNullOrWhiteSpace(docID)) { return null; }

            var strSQL = String.Format(getOvertimeFormSQL(), String.Concat(RepositoryFactory.SmartManConn["DataSource"], ".", RepositoryFactory.SmartManConn["Catelog"]), "where SignDocID_FK = @SignDocID_FK", "");

            var orgData = _rootRepo.QueryForOvertimeFormDataBySignDocID(docID);
            if (orgData == null) { return null; }
            var dutyDate = (DateTime?)orgData.FirstOrDefault()["StartDateTime"];
            var dic = new Conditions()
			{
				{"@DutyDate", dutyDate.HasValue ? dutyDate.Value.ToString("yyyyMMdd") : (string)null },
				{"@SignDocID_FK", docID}
			};

            var result = _dc.QueryForDataTable(strSQL, dic);

            return result;
        }

        public DataTable QueryOvertimeData(string docID)
        {
            var conditions = new Conditions() { { "@SignDocID_FK", docID } };
            string sql = @"select distinct ChiefID_FK 
                            from OvertimeForm otf
                            left join Department dept on otf.DepartmentID_FK=dept.DepartmentID
                            where SignDocID_FK= @SignDocID_FK";
            var result = _dc.QueryForDataTable(sql, conditions);
            return result ?? null;
        }

        public string GetGeneralAffairsADAccount(string formID)
        {
            if (String.IsNullOrWhiteSpace(formID)) { return null; }
            var signTypeData = _rootRepo.QueryForSignTypeByFormID(formID);
            if (signTypeData == null) { return null; }
            var fillingDeptID = (string)signTypeData["FilingDepartmentID_FK"];
            if (String.IsNullOrWhiteSpace(fillingDeptID)) { return null; }
            var deptData = _rootRepo.QueryForDepartmentByDeptID(fillingDeptID);
            if (deptData == null) { return null; }
            var fillingEmpID = (string)deptData["FilingEmployeeID_FK"];
            if (String.IsNullOrWhiteSpace(fillingEmpID)) { return null; }
            var empData = _rootRepo.QueryForEmployeeByEmpID(fillingEmpID);
            if (empData == null) { return null; }
            var adAccount = (string)empData["ADAccount"];
            if (String.IsNullOrWhiteSpace(adAccount)) { return null; }

            return adAccount;
        }

        //todo
        public MultiConditions DeleteData(string signDocID)
        {
            return new MultiConditions() { { "Delete From overtimeform Where SignDocID_FK = @SignDocID_FK", new Conditions() { { "@SignDocID_FK", signDocID } } } };
        }
    }
}
