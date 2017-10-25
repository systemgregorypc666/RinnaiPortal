using DBTools;
using RinnaiPortal.Extensions;
using RinnaiPortal.Interface;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RinnaiPortal.Repository.Sign.Forms
{
    public class ForgotPunchRepository : IForms
    {
        private DB _dc { get; set; }
        private RootRepository _rootRepo { get; set; }
        private ProcessWorkflowRepository _pwfRepo { get; set; }

        public ForgotPunchRepository(DB dc, RootRepository rootRepo, ProcessWorkflowRepository pwfRepo)
        {
            _dc = dc;
            _rootRepo = rootRepo;
            _pwfRepo = pwfRepo;
        }

        //是否為主管
        protected bool IsChief(string employeeID, string departmentID)
        {
            var deptData = _rootRepo.QueryForDepartmentByDeptID(departmentID);
            return employeeID.Equals(deptData["ChiefID_FK"]);
        }

        /// <summary>
        /// 找尋送簽人的主管
        /// </summary>
        /// <param name="signDocID">忘刷單簽核單號</param>
        /// <returns></returns>
        public string FindChiefID(string signDocID, string empName)
        {
            string strSQL = @"
            select ADAccount from Employee where EmployeeID =(
            select ChiefID_FK from Department where  DepartmentID = 
            (select DepartmentID_FK from ForgotPunchForm where SignDocID_FK = @SignDocID_FK)) ";
            var strCondition = new Conditions() { { "@SignDocID_FK", signDocID } };

            DataRow result = _dc.QueryForDataRow(strSQL, strCondition);
            if (result == null)
            {
                strSQL = @"
            select ADAccount from Employee where EmployeeID =(
            select ChiefID_FK from Department where  DepartmentID = 
            (select DepartmentID_FK from Employee where ADAccount = @empID)) ";
                strCondition = new Conditions() { { "@empID", empName } };
                result = _dc.QueryForDataRow(strSQL, strCondition);
                if (result == null)
                return "";
            }
            return result["ADAccount"].ToString();
        }

        //簽核作業頁面 > 取得表單資料
        public ForgotPunchViewModel GetForgotPunchForm(string signDocID)
        {
            ForgotPunchViewModel model = null;
            string strSQL =
@"Select per.PunchName , forgot.*, apply.EmployeeName ApplayName, emp.EmployeeName, dep.DepartmentName from ForgotPunchForm forgot
left outer join Employee apply on forgot.ApplyID_FK = apply.EmployeeID
left outer join Employee emp on forgot.EmployeeID_FK = emp.EmployeeID
left outer join Department dep on forgot.DepartmentID_FK = dep.DepartmentID
left outer join PeriodType per on forgot.PeriodType = per.PunchID
Where SignDocID_FK = @SignDocID_FK";

            var strCondition = new Conditions() { { "@SignDocID_FK", signDocID } };

            DataRow result = _dc.QueryForDataRow(strSQL, strCondition);
            if (result == null) { return null; }

            model = new ForgotPunchViewModel()
            {
                SN = Int32.Parse(result["SN"].ToString()),
                FormID_FK = Int32.Parse(result["FormID_FK"].ToString()),
                SignDocID_FK = result["SignDocID_FK"].ToString(),
                ApplyID_FK = result["ApplyID_FK"].ToString(),
                ApplyName = result["ApplayName"].ToString(),
                ApplyDateTime = DateTime.Parse(result["ApplyDateTime"].ToString()),
                EmployeeID_FK = result["EmployeeID_FK"].ToString(),
                EmployeeName = result["EmployeeName"].ToString(),
                DepartmentID_FK = result["DepartmentID_FK"].ToString(),
                DepartmentName = result["DepartmentName"].ToString(),
                PeriodType = Int32.Parse(result["PeriodType"].ToString()),
                PunchName = result["PunchName"].ToString(),
                ForgotPunchInDateTime = !result["ForgotPunchInDateTime"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ForgotPunchInDateTime"].ToString()) : (DateTime?)null,
                ForgotPunchOutDateTime = !result["ForgotPunchOutDateTime"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ForgotPunchOutDateTime"].ToString()) : (DateTime?)null,

                Note = result["Note"].ToString()
            };

            return model;
        }

        /// <summary>
        /// 新增忘刷單 送出簽核 主要函式
        /// </summary>
        /// <param name="modelList"></param>
        public void CreateData(List<RinnaiForms> modelList)
        {
            var fogotList = modelList.Cast<ForgotPunchViewModel>();
            var model = fogotList.First();

            model.SignDocID_FK = GetSeqIDUtils.GetSignDocID(model.Creator, "FP");
            if (String.IsNullOrWhiteSpace(model.SignDocID_FK)) { throw new Exception("系統忙碌中請稍候再試!"); }

            var status = "2";
            var createDateTime = DateTime.Now;
            var procedureData = _rootRepo.QueryForSignProcedureByRuleID(model.RuleID_FK);
            var remainder = procedureData != null ? Int32.Parse(procedureData["SignLevel"].ToString()) : -1;
            //判斷 FlowRule
            if (!_pwfRepo.IsFollowFlowRule(model.RuleID_FK, model.DepartmentID_FK)) { throw new Exception("不符合簽核規則，無法新增!"); }

            // 取得簽核人員部門資料
            var deptData = _rootRepo.QueryForDepartmentByDeptID(model.DepartmentID_FK);
            if (deptData == null) { throw new Exception("查無簽核人員所屬部門資料!"); }

            var upperDeptData = _pwfRepo.FindUpperDeptData(model.DepartmentID_FK, model.EmployeeID_FK);
            var chiefID = upperDeptData.Keys.Single();
            var currentSignLevelDeptID = upperDeptData.Values.Single();

            // create SignForm_Main data
            var manipulationConditions = new List<MultiConditions>();

            var dic = new Conditions()
			{
				{"@SignDocID", model.SignDocID_FK},
				{"@FormID_FK", model.FormID_FK},
				{"@EmployeeID_FK", model.ApplyID_FK},
				{"@CurrentSignLevelDeptID_FK", currentSignLevelDeptID},
				{"@FinalStatus", status},
				{"@remainder", remainder},
				{"@Creator", model.Creator},
				{"@CreateDate", createDateTime},
				{"@SendDate", createDateTime},
			};
            var strSQL = _dc.ConstructInsertDML("signform_main", dic);
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            // create SignForm_Detail data
            dic = new Conditions()
			{
				{"@SignDocID_FK", model.SignDocID_FK},
				{"@ChiefID_FK", chiefID},
				{"@Status", status},
				{"@Creator", model.Creator},
				{"@CreateDate", createDateTime},
			};
            strSQL = _dc.ConstructInsertDML("signform_detail", dic);
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            // add log
            dic = new Conditions()
			{
				{"@SignDocID_FK", model.SignDocID_FK},
				{"@FormID_FK", model.FormID_FK},
				{"@EmployeeID_FK", model.ApplyID_FK},
				{"@SendDate", createDateTime},
				{"@CurrentSignLevelDeptID_FK", currentSignLevelDeptID},
				{"@FinalStatus", status},
				{"@remainder", remainder},
				{"@Creator_Main", model.Creator},
				{"@CreateDate_Main", createDateTime},
				{"@DetailSignDocID_FK", model.SignDocID_FK},
				{"@ChiefID_FK", chiefID},
				{"@Status", status},
				{"@Creator_Detail", model.Creator},
				{"@CreateDate_Detail", createDateTime},
				{"@LogDatetime", DateTime.Now},
			};
            strSQL = _dc.ConstructInsertDML("signform_log", dic);
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            // create ForgotPunch data
            dic = new Conditions()
			{
				{"@FormID_FK", model.FormID_FK},
				{"@SignDocID_FK", model.SignDocID_FK},
				{"@ApplyID_FK", model.ApplyID_FK},
				{"@ApplyDateTime", model.ApplyDateTime},
				{"@EmployeeID_FK", model.EmployeeID_FK},
				{"@DepartmentID_FK", model.DepartmentID_FK},
				{"@PeriodType", model.PeriodType},
				{"@ForgotPunchInDateTime", model.ForgotPunchInDateTime},
				{"@ForgotPunchOutDateTime",model.ForgotPunchOutDateTime},
				{"@Note", model.Note},
				{"@Creator",model.Creator},
				{"@CreateDate",model.CreateDate},
			};
            strSQL = _dc.ConstructInsertDML("forgotpunchform", dic);
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            try
            {
                if (!_dc.ExecuteMultAndCheck(manipulationConditions)) { throw new Exception("新增忘刷單失敗!"); }
            }
            catch (Exception ex)
            {
                throw new Exception("新增忘刷單失敗!" + ex.Message);
            }
        }

        //編輯忘刷單 / 送出簽核
        public void EditData(List<RinnaiForms> modelList)
        {
            var fogotList = modelList.Cast<ForgotPunchViewModel>().ToList();
            var model = fogotList.First();

            // create SignForm_Main data
            var manipulationConditions = new List<MultiConditions>();
            var strSQL = string.Empty;
            Conditions dic = null;

            var status = "2";
            var currentDateTime = DateTime.Now.FormatDatetime();
            var procedureData = _rootRepo.QueryForSignProcedureByRuleID(model.RuleID_FK);
            var remainder = procedureData != null ? Int32.Parse(procedureData["SignLevel"].ToString()) : -1;

            //判斷 FlowRule
            if (!_pwfRepo.IsFollowFlowRule(model.RuleID_FK, model.DepartmentID_FK))
            {
                throw new Exception("不符合簽核規則，無法編輯!");
            }

            var upperDeptData = _pwfRepo.FindUpperDeptData(model.DepartmentID_FK, model.EmployeeID_FK);
            var chiefID = upperDeptData.Keys.Single();
            var currentSignLevelDeptID = upperDeptData.Values.Single();

            // Update SignForm_Main
            strSQL =
@"UPDATE signform_main
SET    employeeid_fk = @EmployeeID_FK,
	   senddate = @SendDate,
	   currentsignleveldeptid_fk = @CurrentSignLevelDeptID_FK,
	   finalstatus = @FinalStatus,
	   remainder = @remainder,
	   modifier = @Modifier,
	   modifydate = @ModifyDate
WHERE  signdocid = @SignDocID ";
            dic = new Conditions()
			{
				{"@SignDocID", model.SignDocID_FK},
				{"@EmployeeID_FK", model.ApplyID_FK},
				{"@SendDate", currentDateTime},
				{"@CurrentSignLevelDeptID_FK", currentSignLevelDeptID},
				{"@FinalStatus", status},
				{"@remainder", remainder},
				{"@Modifier", model.Modifier},
				{"@ModifyDate", currentDateTime},
			};
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            // create SignForm_Detail data
            strSQL =
@"INSERT INTO signform_detail
			(signdocid_fk,
			 chiefid_fk,
			 status,
			 creator,
			 createdate)
VALUES      (@SignDocID_FK,
			 @ChiefID_FK,
			 @Status,
			 @Creator,
			 @CreateDate) ";
            dic = new Conditions()
			{
				{"@SignDocID_FK", model.SignDocID_FK},
				{"@ChiefID_FK", chiefID},
				{"@Status", status},
				{"@Creator", model.Modifier},
				{"@CreateDate", currentDateTime},
			};
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            // add log
            strSQL =
@"INSERT INTO SIGNFORM_LOG
			(signdocid_fk,
			 formid_fk,
			 employeeid_fk,
			 senddate,
			 currentsignleveldeptid_fk,
			 finalstatus,
			 remainder,
			 modifier_main,
			 modifydate_main,
			 detailsigndocid_fk,
			 chiefid_fk,
			 status,
			 creator_detail,
			 createdate_detail,
			 logdatetime)
VALUES      (@SignDocID_FK,
			 @FormID_FK,
			 @EmployeeID_FK,
			 @SendDate,
			 @CurrentSignLevelDeptID_FK,
			 @FinalStatus,
			 @Remainder,
			 @Modifier_Main,
			 @ModifyDate_Main,
			 @DetailSignDocID_FK,
			 @ChiefID_FK,
			 @Status,
			 @Creator_Detail,
			 @CreateDate_Detail,
			 @LogDatetime ) ";
            dic = new Conditions()
			{
				{"@SignDocID_FK", model.SignDocID_FK},
				{"@FormID_FK", model.FormID_FK},
				{"@EmployeeID_FK", model.ApplyID_FK},
				{"@SendDate", currentDateTime},
				{"@CurrentSignLevelDeptID_FK", currentSignLevelDeptID},
				{"@FinalStatus", status},
				{"@Remainder", remainder},
				{"@Modifier_Main", model.Modifier},
				{"@ModifyDate_Main", currentDateTime},
				{"@DetailSignDocID_FK", model.SignDocID_FK},
				{"@ChiefID_FK", chiefID},
				{"@Status", status},
				{"@Creator_Detail", model.Modifier},
				{"@CreateDate_Detail", currentDateTime},
				{"@LogDatetime", DateTime.Now},
			};
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            // Update ForgotPunch data
            strSQL =
@"UPDATE forgotpunchform
SET    applydatetime = @ApplyDateTime,
	   employeeid_fk = @EmployeeID_FK,
	   departmentid_fk = @DepartmentID_FK,
	   periodtype = @PeriodType,
	   forgotpunchindatetime = @ForgotPunchInDateTime,
	   forgotpunchoutdatetime = @ForgotPunchOutDateTime,
	   note = @Note,
	   modifier = @Modifier,
	   modifydate = @ModifyDate
WHERE  signdocid_fk = @SignDocID_FK ";
            dic = new Conditions()
			{
				{"@SignDocID_FK", model.SignDocID_FK},
				{"@ApplyDateTime", model.ApplyDateTime},
				{"@EmployeeID_FK", model.EmployeeID_FK},
				{"@DepartmentID_FK", model.DepartmentID_FK},
				{"@PeriodType", model.PeriodType},
				{"@ForgotPunchInDateTime", model.ForgotPunchInDateTime},
				{"@ForgotPunchOutDateTime",model.ForgotPunchOutDateTime},
				{"@Note", model.Note},
				{"@Modifier", model.Modifier},
				{"@ModifyDate ", model.ModifyDate},
			};
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            try
            {
                if (!_dc.ExecuteMultAndCheck(manipulationConditions)) { throw new Exception("編輯忘刷單失敗!"); }
            }
            catch (Exception ex)
            {
                throw new Exception("編輯忘刷單失敗!" + ex.Message);
            }
        }

        //todo
        public MultiConditions DeleteData(string signDocID)
        {
            return new MultiConditions() { { "Delete From forgotpunchform Where SignDocID_FK = @SignDocID_FK", new Conditions() { { "@SignDocID_FK", signDocID } } } };
        }
    }
}