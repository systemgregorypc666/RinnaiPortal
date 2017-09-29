using DBTools;
using RinnaiPortal.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository.Sign;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.ViewModel.Sign.Forms;

namespace RinnaiPortal.Tools
{
    public class RootRepository
    {
        DataTable temp = new DataTable();
        private DB _dc { get; set; }
        public RootRepository(DB dc)
        {
            _dc = dc;
        }

        //取得員工資料
        public Dictionary<string, string> GetEmployee()
        {
            var dataSource = _dc.QueryForDictionary("Select EmployeeID, EmployeeID + ' ' + EmployeeName from Employee Where Disabled = 'False'", null);
            return ViewUtils.ConstructDropDownList(dataSource);
        }
        //取得部門員工資料
        public Dictionary<string, string> GetDeptEmployee(string DeptID)
        {
            var dataSource = _dc.QueryForDictionary("Select EmployeeID, EmployeeID + ' ' + EmployeeName from Employee Where Disabled = 'False' and CostDepartmentID='" + DeptID + "'", null);
            return ViewUtils.ConstructDropDownList(dataSource);
        }

        //取得部門資料
        public Dictionary<string, string> GetDepartment()
        {
            var dataSource = _dc.QueryForDictionary("Select DepartmentID, DepartmentName from Department Where Disabled = 'False'", null);
            return ViewUtils.ConstructDropDownList(dataSource);
        }

        //20161117 新增
        //取得成本部門資料
        public Dictionary<string, string> GetCostDepartment()
        {
            var dataSource = _dc.QueryForDictionary("Select DepartmentID, DepartmentName from Department Where Disabled = 'False' and Virtual='N'", null);
            return ViewUtils.ConstructDropDownList(dataSource);
        }

        //取得忘刷類型
        public Dictionary<string, string> GetPeriodType()
        {
            var dataSource = _dc.QueryForDictionary("Select PunchID, PunchName from PeriodType", null);
            return ViewUtils.ConstructDropDownList(dataSource);
        }

        //取得簽核規則
        public Dictionary<string, string> GetSignProcedure()
        {
            var dataSource = _dc.QueryForDictionary("Select SignID, SignID from SignProcedure Where Disabled = 'False'", null);
            return ViewUtils.ConstructDropDownList(dataSource);
        }

        //取得群組
        public Dictionary<string, string> GetGroup()
        {
            var dataSource = _dc.QueryForDictionary("Select GroupType, GroupName from SysGroup ", null);
            return dataSource ?? null;
        }

        //取得存取資源
        public Dictionary<string, string> GetSystemArchitecture()
        {
            var dataSource = _dc.QueryForDictionary("Select ResourceAlias, NodeTitle  from SystemArchitecture Where ResourceAlias is not null and Active = 'True' order by ParentID", null);
            return dataSource ?? null;
        }


        /// <summary>
        /// 取得結薪日資料
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetSalaryLimit()
        {
            var dataSource = _dc.QueryForDictionary("Select 'LimitDate', LimitDate from SalaryLimit", null);
            return dataSource ?? null;
        }

        //查詢部門資料 根據 DocID
        public DataRow QueryForDepartmentByDocID(string docID)
        {
            var conditions = new Conditions() { { "@SignDocID", docID } };
            string sql =
@"SELECT * 
FROM   department 
WHERE  disabled = 'False' 
	   AND departmentid = (SELECT DISTINCT departmentid_fk 
						   FROM   employee 
						   WHERE  employeeid = (SELECT DISTINCT employeeid_fk 
												FROM   signform_main 
												WHERE  signdocid = @SignDocID)) ";
            var result = _dc.QueryForDataRow(sql, conditions);
            return result ?? null;
        }

        //查詢部門資料 根據 部門ID
        public DataRow QueryForDepartmentByDeptID(string deptID)
        {
            var strSQL =
@"Select * From Department Where Disabled = 'False' and DepartmentID = @DepartmentID";
            var dic = new Conditions() { { "@DepartmentID", deptID } };
            var result = _dc.QueryForDataRow(strSQL, dic);
            return result ?? null;
        }

        //查詢部門資料 根據 AD帳號
        public DataRow QueryForDepartmentByADAccount(string adAccount)
        {
            string strSQL =
@"SELECT * 
FROM   department 
WHERE  disabled = 'False' 
	   AND departmentid = (SELECT departmentid_fk 
						   FROM   employee 
						   WHERE  adaccount = @ADAccount) ";
            var conditions = new Conditions() { { "@ADAccount", adAccount } };
            var result = _dc.QueryForDataRow(strSQL, conditions);
            return result ?? null;
        }

        //查詢部門資料 根據 員工編號
        public DataRow QueryForDepartmentByEmpID(string employeeID)
        {
            string strSQL =
@"SELECT * 
FROM   department 
WHERE  disabled = 'False' 
	   AND departmentid = (SELECT departmentid_fk 
						   FROM   employee 
						   WHERE  employeeID = @EmployeeID) ";
            var conditions = new Conditions() { { "@EmployeeID", employeeID } };
            var result = _dc.QueryForDataRow(strSQL, conditions);
            return result ?? null;
        }

        //查詢部門資料 根據 單據編號
        public DataRow QueryForDepartmentByFormID(int formID)
        {
            string strSQL =
@"SELECT * 
FROM Department
WHERE DepartmentID = (SELECT distinct FilingDepartmentID_FK 
					  FROM SignType 
					  WHERE FormID = @FormID)";
            var conditions = new Conditions() { { "@FormID", formID } };
            var result = _dc.QueryForDataRow(strSQL, conditions);
            return result ?? null;
        }

        //查詢員工資料 根據 DocID
        public DataRow QueryForEmployeeByDocID(string docID)
        {
            var conditions = new Conditions() { { "@SignDocID", docID } };
            string sql = @"
                    SELECT * 
                    FROM   employee 
                    WHERE  disabled = 'False' 
                    AND employeeid = (SELECT DISTINCT employeeid_fk 
                    FROM   signform_main 
                    WHERE  signdocid = @SignDocID) ";
            var result = _dc.QueryForDataRow(sql, conditions);
            return result ?? null;
        }


        /// <summary>
        /// 查詢員工資料 根據 員工ID
        /// </summary>
        /// <param name="empID"></param>
        /// <returns></returns>
        public DataRow QueryForEmployeeByEmpID(string empID)
        {
            var conditions = new Conditions() { { "@EmployeeID", empID } };
            string sql = sql = @"
            Select * from Employee Where Disabled = 'False' and employeeID = @EmployeeID";
            var result = _dc.QueryForDataRow(sql, conditions);
            return result ?? null;
        }

        /// <summary>
        /// 查詢員工資料 根據 AD帳號
        /// </summary>
        /// <param name="adAccount"></param>
        /// <returns></returns>
        public DataRow QueryForEmployeeByADAccount(string adAccount)
        {
            var conditions = new Conditions() { { "@ADAccount", adAccount } };
            string sql = sql =
            @"Select * from Employee Where Disabled = 'False' and ADAccount = @ADAccount";
            var result = _dc.QueryForDataRow(sql, conditions);
            return result ?? null;
        }

        /// <summary>
        /// 查詢全部員工資料
        /// </summary>
        /// <returns></returns>
        public DataTable QueryForEmployee()
        {
            string sql =
            @"Select * from Employee Where Disabled = 'False'";
            var result = _dc.QueryForDataTable(sql, null);
            return result ?? null;
        }

        /// <summary>
        /// 查詢簽核類型 根據 DocID
        /// </summary>
        /// <param name="docID"></param>
        /// <returns></returns>
        public DataRow QueryForSignTypeBySignDocID(string docID)
        {
            var conditions = new Conditions() { { "@SignDocID", docID } };
            string sql = @"
            SELECT * 
            FROM   signtype 
            WHERE  formid = (SELECT DISTINCT formid_fk 
            FROM   signform_main 
            WHERE  signdocid = @SignDocID) ";
            var result = _dc.QueryForDataRow(sql, conditions);
            return result ?? null;
        }

        /// <summary>
        /// 查詢簽核類型 根據 FormID
        /// </summary>
        /// <param name="formTypeID"></param>
        /// <returns></returns>
        public DataRow QueryForSignTypeByFormID(string formTypeID)
        {
            var conditions = new Conditions() { { "@FormID", formTypeID } };
            string sql =
            @"select * from SignType where FormID = @FormID";
            var result = _dc.QueryForDataRow(sql, conditions);
            return result ?? null;
        }


        /// <summary>
        /// 查詢群組 根據 存取權限
        /// </summary>
        /// <param name="groupType"></param>
        /// <returns></returns>
        public DataRow QueryForGroupDataByGroupType(string groupType)
        {
            var conditions = new Conditions() { { "@GroupType", groupType } };
            string sql = @"
            SELECT * 
            FROM   sysgroup 
            WHERE  groupType = @GroupType ";
            var result = _dc.QueryForDataRow(sql, conditions);
            return result ?? null;
        }


        /// <summary>
        /// 查詢權限 根據 AD帳號
        /// </summary>
        /// <param name="adAccount"></param>
        /// <returns></returns>
        public DataRow QueryForAccessTypeByADAccount(string adAccount)
        {
            var conditions = new Conditions() { { "@ADAccount", adAccount } };
            string sql = @"
                SELECT DISTINCT AccessType 
                FROM   employee 
                WHERE  adaccount = @ADAccount ";
            var result = _dc.QueryForDataRow(sql, conditions);
            return result ?? null;
        }


        /// <summary>
        /// 查詢忘刷單資料 根據 SignDocID
        /// </summary>
        /// <param name="signDocID"></param>
        /// <returns></returns>
        public List<DataRow> QueryForForgotPunchDataBySignDocID(string signDocID)
        {
            var conditions = new Conditions() { { "@SignDocID_FK", signDocID } };
            string sql =
            @"Select * From ForgotPunchForm Where SignDocID_FK = @SignDocID_FK";
            var result = _dc.QueryForDataRows(sql, conditions);
            return result ?? null;
        }

        /// <summary>
        /// 查詢受訓單資料 根據 SignDocID
        /// </summary>
        /// <param name="signDocID"></param>
        /// <returns></returns>
        public List<DataRow> QueryForTrainDataBySignDocID(string signDocID)
        {
            string strCatelog = RepositoryFactory.TrainConn["Catelog"];
            var conditions = new Conditions() { { "@SignDocID", signDocID } };
            string sql = String.Format(@"Select * From {0}..FORM_SIGN Where SignDocID = @SignDocID", strCatelog);
            var result = _dc.QueryForDataRows(sql, conditions);
            return result ?? null;
        }


        /// <summary>
        /// 查詢加班單資料 根據 SignDocID
        /// </summary>
        /// <param name="signDocID"></param>
        /// <returns></returns>
        public List<DataRow> QueryForOvertimeFormDataBySignDocID(string signDocID)
        {
            var conditions = new Conditions() { { "@SignDocID_FK", signDocID } };
            string sql = @"
            Select * From OvertimeForm Where SignDocID_FK = @SignDocID_FK";
            var result = _dc.QueryForDataRows(sql, conditions);
            return result ?? null;
        }


        /// <summary>
        /// 查詢加班單資料 根據日期
        /// </summary>
        /// <param name="date"></param>
        /// <param name="status"></param>
        /// <param name="status2"></param>
        /// <returns></returns>
        public List<DataRow> QueryForOvertimeFormDataByDate(DateTime date, string status = "1", string status2 = "5")
        {
            var conditions = new Conditions() { { "@date", date.Date } };
            string sql = @"
            select distinct o.* from OvertimeForm o
            left outer join SignForm_Main m
            on o.SignDocID_FK = m.SignDocID 
            Where convert(varchar, startDateTime, 111) = convert(varchar, @date, 111) ";
            if (!String.IsNullOrWhiteSpace(status))
            {
                conditions.Add("@status", status);
                sql += " and m.FinalStatus != @status";
            }
            if (!String.IsNullOrWhiteSpace(status2))
            {
                conditions.Add("@status2", status2);
                sql += " and m.FinalStatus != @status2";
            }
            var result = _dc.QueryForDataRows(sql, conditions);
            return result ?? null;
        }

        /// <summary>
        /// 查詢簽核單據類別資料
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public DataRow QueryForSignTypeDataBySeries(string series)
        {
            var conditions = new Conditions() { { "@FormSeries", series } };
            string sql =
            @"SELECT * FROM SignType Where FormSeries = @FormSeries ";
            var result = _dc.QueryForDataRow(sql, conditions);
            return result ?? null;
        }

        /// <summary>
        /// 紀錄簽核流程(代碼組織階層)
        /// </summary>
        /// <param name="ruleID"></param>
        /// <returns></returns>
        public DataRow QueryForSignProcedureByRuleID(string ruleID)
        {
            var conditions = new Conditions() { { "@SignID", ruleID } };
            string sql = @"
                    SELECT * 
                    FROM   SignProcedure 
                    WHERE  disabled = 'False' 
                    AND SignID = @SignID  ";
            var result = _dc.QueryForDataRow(sql, conditions);
            return result ?? null;
        }

        /// <summary>
        /// 查詢簽核規則 根據 SignDocID
        /// </summary>
        /// <param name="signDocID"></param>
        /// <returns></returns>
        public DataRow QueryForSignProcedureBySignDocID(string signDocID)
        {
            var conditions = new Conditions() { { "@SignDocID", signDocID } };
            string sql = @"
                SELECT * 
                FROM   signprocedure 
                WHERE  signid = (SELECT DISTINCT signid_fk 
                FROM   signtype 
                WHERE  formid = (SELECT DISTINCT formid_fk 
                FROM   signform_main 
                WHERE  signdocid = @SignDocID)) ";
            var result = _dc.QueryForDataRow(sql, conditions);
            return result ?? null;
        }



        //查詢簽核規則 根據 RuleID(SignID)
        public DataRow QueryForSignForm_MainBySignDocID(string signDocID)
        {
            var conditions = new Conditions() { { "@SignDocID", signDocID } };
            var sql =
@"select * from SignForm_Main where SignDocID = @SignDocID";
            var result = _dc.QueryForDataRow(sql, conditions);
            return result ?? null;
        }

        //查詢支援部門階層 根據 SupportDeptID_FK
        public DataRow QueryDeptLevel(string SupportDeptID_FK)
        {
            var conditions = new Conditions() { { "@SupportDeptID_FK", SupportDeptID_FK } };
            var sql = @"select DepartmentLevel from Department where DepartmentID = @SupportDeptID_FK";
            var result = _dc.QueryForDataRow(sql, conditions);
            return result ?? null;
        }

        //20161128刪除草稿
        public void DelData(string DocID)
        {
            var mainpulationConditions = new Conditions()
		    {
			    {"@SignDocID", DocID },
                {"@ModifyDate", DateTime.Now }
		    };

            try
            {
                _dc.ExecuteAndCheck(@"Update SignForm_Main Set FinalStatus = '5', modifydate = @ModifyDate Where SignDocID = @SignDocID ", mainpulationConditions);

            }
            catch (Exception ex)
            {
                throw new Exception("刪除失敗!" + ex.Message);
            }
        }

        //20161201 抽單(狀態同駁回)
        public void PumpData(string DocID, string FormID, string employeeID, string UserName, string CurrentSignLevelDeptID, string Remainder)
        {
            //var empData = _rootRepo.QueryForEmployeeByADAccount(User.Identity.Name);
            var mainpulationConditions = new Conditions()
		    {
			    {"@SignDocID", DocID },
                {"@Modifier", employeeID },
                {"@ModifyDate", DateTime.Now }
		    };
            var detailpulationConditions = new Conditions()
		    {
			    {"@SignDocID_FK", DocID }
		    };
            var logpulationConditions = new Conditions()
			{
				{ "@SignDocID_FK", DocID !=null ? DocID : (string)null},
				{ "@FormID_FK", FormID !=null ? FormID : (string)null},
				{ "@EmployeeID_FK", employeeID !=null ? employeeID : (string)null},
				{ "@SendDate", DateTime.Now},
				{ "@CurrentSignLevelDeptID_FK", CurrentSignLevelDeptID !=null ? CurrentSignLevelDeptID : (string)null},
				{ "@FinalStatus", "4"},
                { "@Remark", "自行抽單"},
				{ "@Remainder", Remainder !=null ? Remainder : (string)null},
				{ "@Creator_Main", null},
				{ "@CreateDate_Main", null},
				{ "@Modifier_Main", UserName !=null ? UserName : (string)null},
				{ "@ModifyDate_Main", DateTime.Now},
				{ "@LogDatetime", DateTime.Now}
			};
            try
            {
                //1.駁回後即送回原部門 改Main 為最初設定 狀態為駁回
                _dc.ExecuteAndCheck(@"Update SignForm_Main Set FinalStatus = '4',modifier=@Modifier,modifydate = @ModifyDate Where SignDocID = @SignDocID ", mainpulationConditions);
                //2.刪除Detail資料
                _dc.ExecuteAndCheck(@"Delete SignForm_Detail Where SignDocID_FK = @SignDocID_FK ", detailpulationConditions);
                //3.寫入歷程log
                _dc.ExecuteAndCheck(@"Insert into SignForm_Log (SignDocID_FK, FormID_FK, EmployeeID_FK, SendDate, CurrentSignLevelDeptID_FK,FinalStatus,Remark,Remainder,Creator_Main,CreateDate_Main,Modifier_Main,ModifyDate_Main,LogDatetime) 
                                    Values (@SignDocID_FK, @FormID_FK, @EmployeeID_FK, @SendDate, @CurrentSignLevelDeptID_FK, @FinalStatus, @Remark, @Remainder, @Creator_Main, @CreateDate_Main, @Modifier_Main, @ModifyDate_Main, @LogDatetime)", logpulationConditions);
            }
            catch (Exception ex)
            {
                throw new Exception("抽單失敗!" + ex.Message);
            }
        }

        public string Find_CreatDate(string SignDocID)
        {
            string CreatDate = string.Empty;
            string strSQL = @"Select * From OvertimeForm Where SignDocID_FK = '" + SignDocID + "'";
            temp = DBClass.Create_Table(strSQL, "GET");
            if (temp.Rows.Count > 0)
            {
                CreatDate = temp.Rows[0]["CreateDate"].ToString();
            }
            return CreatDate;
        }

        /// <summary>
        /// 判斷是否有申請過加班，並判斷請領方式(同一天 請領方式須相同)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string CheckHasOverTimeThenPayTypeSame(OvertimeViewModel model)
        {
            string beforePayTypeValue = string.Empty;
            var stDate = string.Format("{0:yyyyMMdd}", model.StartDateTime);
            var enDate = string.Format("{0:yyyyMMdd}", model.EndDateTime);
            string strSQL = @"
                            select * 
                            from overtimeform o
                            inner join SignForm_Main s on o.SignDocID_FK = s.SignDocID
                            where s.FinalStatus <> '5' 
                            and o.EmployeeID_FK = @EmployeCD
                            and PayTypeKey != @OvertimePay
                            and (CONVERT(varchar(12) ,StartDateTime, 112 ) = @StartDate 
                            or CONVERT(varchar(12) ,EndDateTime, 112 ) = @EndDate)
                            ";
            var conditions = new Conditions()
			{
				{ "@StartDate", stDate},
				{ "@EndDate", enDate},
				{ "@EmployeCD",  model.EmployeeID_FK},
				{ "@OvertimePay",  model.PayTypeKey}
			};
            var result = _dc.QueryForDataRow(strSQL, conditions);
            if (result != null)
            {
                string typeKey = result["PayTypeKey"].ToString();
                switch (typeKey)
                {
                    case "overtimeLeave":
                        beforePayTypeValue = "換休";
                        break;
                    case "overtimePay":
                        beforePayTypeValue = "加班費";
                        break;
                    default:
                        break;
                }
            }
            return beforePayTypeValue;
        }


        /// <summary>
        /// 取得員工每月加班總時數(簽核中)(不含國定假日前8H)   
        /// </summary>
        /// <param name="date"></param>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public DataRow QueryOverWork(string date, string employeeID)
        {
            if (String.IsNullOrWhiteSpace(date) || String.IsNullOrWhiteSpace(employeeID)) { return null; }
            string strSQL = @" SELECT ISNULL(sum(overtime.totalHours),0)-ISNULL(sum(case when H_TYPE='2' and overtime.totalHours>=8 then 8 else 0 end),0) total	   	   
                                FROM  overtimeform overtime 
	   
	                                   LEFT OUTER JOIN signform_main main 
					                                ON overtime.signdocid_fk  = main.signdocid
	                                   LEFT OUTER JOIN employee emp 
					                                ON overtime.employeeid_fk = emp.employeeid 
	                                   Left OUTER JOIN ITEIP.hris.dbo.DUTYWORK dtw
					                                ON overtime.employeeid_fk = dtw.EMPLOYECD and CONVERT(varchar(12) , overtime.startdatetime, 112 )=dtw.WORKDATE
                                WHERE EMPLOYECD=@EmployeCD and CONVERT(varchar(12) , overtime.startdatetime, 112 ) between CASE WHEN (CONVERT(varchar(12) , @WorkDate, 111 ) > CONVERT(varchar(12) , convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'20', 111 )) then convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'21' else convert(varchar(4),YEAR(DATEADD(MONTH,-1,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-1,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-1,@WorkDate)))+'21' end and CASE WHEN (CONVERT(varchar(12) , @WorkDate, 111 ) > CONVERT(varchar(12) , convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'20', 111 )) then convert(varchar(4),YEAR(DATEADD(MONTH,+1,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,+1,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,+1,@WorkDate)))+'20' else convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'20' end 
                                And finalStatus ='2' ";
            var conditions = new Conditions()
			{
				{ "@WorkDate", date},
				{"@EmployeCD",  employeeID},
			};

            var result = _dc.QueryForDataRow(strSQL, conditions);
            if (result == null) { return null; }

            return result;
        }

        /// <summary>
        /// 20170310 add取得員工每月加班總時數(簽核中)(含國定假日前8H)     
        /// </summary>
        /// <param name="date"></param>
        /// <param name="employeeID"></param>
        /// <returns></returns>
        public DataRow QueryAllOverWork(string date, string employeeID)
        {
            if (String.IsNullOrWhiteSpace(date) || String.IsNullOrWhiteSpace(employeeID)) { return null; }
            string strSQL = @" SELECT ISNULL(sum(overtime.totalHours),0) total	   	   
                                FROM  overtimeform overtime 
	   
	                                   LEFT OUTER JOIN signform_main main 
					                                ON overtime.signdocid_fk  = main.signdocid
	                                   LEFT OUTER JOIN employee emp 
					                                ON overtime.employeeid_fk = emp.employeeid 
	                                   Left OUTER JOIN ITEIP.hris.dbo.DUTYWORK dtw
					                                ON overtime.employeeid_fk = dtw.EMPLOYECD and CONVERT(varchar(12) , overtime.startdatetime, 112 )=dtw.WORKDATE
                                WHERE EMPLOYECD=@EmployeCD and CONVERT(varchar(12) , overtime.startdatetime, 112 ) between CASE WHEN (CONVERT(varchar(12) , @WorkDate, 111 ) > CONVERT(varchar(12) , convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'20', 111 )) then convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'21' else convert(varchar(4),YEAR(DATEADD(MONTH,-1,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-1,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-1,@WorkDate)))+'21' end and CASE WHEN (CONVERT(varchar(12) , @WorkDate, 111 ) > CONVERT(varchar(12) , convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'20', 111 )) then convert(varchar(4),YEAR(DATEADD(MONTH,+1,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,+1,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,+1,@WorkDate)))+'20' else convert(varchar(4),YEAR(DATEADD(MONTH,-0,@WorkDate)))+replicate('0', (2-len(convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate))))))+convert(varchar(4),MONTH(DATEADD(MONTH,-0,@WorkDate)))+'20' end 
                                And finalStatus ='2' ";
            var conditions = new Conditions()
			{
				{ "@WorkDate", date},
				{"@EmployeCD",  employeeID},
			};

            var result = _dc.QueryForDataRow(strSQL, conditions);
            if (result == null) { return null; }

            return result;
        }

        //查詢換休
        public string Find_ADDOFFHOURS(string USERUD, string T_DAY)
        {
            string Find_ADDOFFHOURS = string.Empty;
            string strSQL = @"exec SP_RUN_ADDHOURS '" + USERUD + "','" + T_DAY + "'";
            temp = DBClass.Create_Table_S(strSQL, "GET");
            if (temp.Rows.Count > 0)
            {
                Find_ADDOFFHOURS = temp.Rows[0]["RETURNHOURS"].ToString();
            }
            return Find_ADDOFFHOURS;
        }
    }
}