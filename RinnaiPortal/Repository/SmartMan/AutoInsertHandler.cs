using DBTools;
using RinnaiPortal.Extensions;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace RinnaiPortal.Repository.SmartMan
{
    public class AutoInsertHandler
    {
        private DB _dc { get; set; }
        private string signDocID { get; set; }
        private RootRepository _rootRepo = RepositoryFactory.CreateRootRepo();
        private SmartManRepository _smartRepo = RepositoryFactory.CreateSmartManRepo();

        public AutoInsertHandler(DB dc, string docID)
        {
            signDocID = docID;
            _dc = dc;
        }

        public AutoInsertHandler(DB dc)
            : this(dc, null)
        {
        }

        public List<MultiConditions> GetDML()
        {
            var result = new List<MultiConditions>();
            if (String.IsNullOrWhiteSpace(signDocID)) { throw new Exception("簽核編號為空值!"); }

            MultiConditions multiDML = null;
            switch (signDocID.Substring(0, 2).ToUpper())
            {
                case "FP":
                    var forgotpunchList = _rootRepo.QueryForForgotPunchDataBySignDocID(signDocID);
                    forgotpunchList.All(row =>
                    {
                        var prototypeModel = WebUtils.ViewModelMapping<ForgotPunchViewModel>(row);
                        var dml = getForgotPunchDML(prototypeModel);
                        result.Add(dml);
                        return true;
                    });

                    break;

                case "OT":
                    var overtimeList = _rootRepo.QueryForOvertimeFormDataBySignDocID(signDocID);

                    // AutoInsert 設定為 True 則自動寫入 SmartMan, 反之不自動寫入
                    if (overtimeList.Any(form => "False".Equals(form["AutoInsert"].ToString(), StringComparison.OrdinalIgnoreCase))) { return null; }

                    var sampleRow = overtimeList.First();

                    var payRangeData = _smartRepo.QueryForPayRange(sampleRow["StartDateTime"].ToDateTimeFormateString("yyyyMMdd"), sampleRow["EndDateTime"].ToDateTimeFormateString("yyyyMMdd"));
                    if (payRangeData == null) { throw new Exception(String.Format("查無{0}薪資歸入月份設定!", sampleRow["StartDateTime"].ToDateTimeFormateString("yyyyMMdd"))); }

                    //表單歸入月份
                    var payRange = ParsePayRange(payRangeData["PayYYYYMM"].ToString());

                    overtimeList.All(row =>
                    {
                        //該員工該月是否已經結算薪資
                        if (IsSettledAccounts(row["EmployeeID_FK"].ToString(), payRange))
                        {
                            //已結算薪資，將此筆歸入下月
                            payRange = payRange.AddMonths(1);
                            //確認是否有下月薪資歸入月份設定
                            if (_smartRepo.QueryForPayRange(payRange.ToString("yyyyMMdd"), payRange.ToString("yyyyMMdd")) == null) { throw new Exception(String.Format("查無{0}薪資歸入月份設定!", payRange.ToString("yyyyMMdd"))); }
                        }

                        var prototypeModel = WebUtils.ViewModelMapping<OvertimeViewModel>(row);
                        var dailyonoffData = _smartRepo.QueryForDailyOnOff(prototypeModel.StartDateTime.Value.ToString("yyyyMMdd"), prototypeModel.EmployeeID_FK);
                        prototypeModel.WorkType = dailyonoffData["WorkType"].ToString();
                        prototypeModel.BeginTime = dailyonoffData["BEGINTIME"].ToString();
                        prototypeModel.EndTime = dailyonoffData["ENDTIME"].ToString();
                        //20161101修改取得員工即時人事成本部門
                        //var employeeData = _rootRepo.QueryForEmployeeByEmpID(prototypeModel.EmployeeID_FK);
                        var employeeData = _smartRepo.QueryCostDept(prototypeModel.EmployeeID_FK);
                        prototypeModel.CostDepartmentID = employeeData["UNITCD"].ToString();
                        var dml = GetOvertimeDML(prototypeModel, payRange);
                        if (dml == null) { return true; } //skip
                        result.Add(dml);
                        return true;
                    });

                    break;

                case "TR":
                    //var TrainList = _rootRepo.QueryForTrainDataBySignDocID(signDocID);
                    //TrainList.All(row =>
                    //{
                    //    var dml = getTrainDML();
                    //    result.Add(dml);
                    //    return true;
                    //});
                    multiDML = getTrainDML(signDocID);
                    result.Add(multiDML);
                    break;
            }

            return result;
        }

        private MultiConditions getForgotPunchDML(ForgotPunchViewModel forgotPunch)
        {
            if (forgotPunch == null || !forgotPunch.AutoInsert) { throw new ArgumentNullException("忘刷資料為空!", new Exception("自動寫入值異常!")); }

            var result = new MultiConditions();
            string tableName = RepositoryFactory.SmartManConn["DataSource"] + "." + RepositoryFactory.SmartManConn["Catelog"] + ".dbo.LostCard";
            string strSQL = null;
            Conditions conditions = new Conditions()
			{
				{ "@CARDNO", forgotPunch.EmployeeID_FK},
                //20161104 志元改版，LostCard無EMPLOYECD欄位，新增Sequenceid紀錄電子簽核單號
				//{ "@EMPLOYECD", forgotPunch.EmployeeID_FK},
                { "@Sequenceid", forgotPunch.SignDocID_FK},
                //20161114 不特地寫入Flag狀態，志元預設為0，才會自動比對刷卡資料
                //20170411 因以直接寫入刷卡資料，故直接給1，至元不比對刷卡資料
                //{ "@FLAG", "1"},
			};

            switch (forgotPunch.PeriodType)
            {
                // 上班
                case 1:
                    conditions.Add("@CARDDATE", forgotPunch.ForgotPunchInDateTime.HasValue ? forgotPunch.ForgotPunchInDateTime.Value.ToString("yyyyMMdd") : (string)null);
                    conditions.Add("@CARDTIME", forgotPunch.ForgotPunchInDateTime.HasValue ? forgotPunch.ForgotPunchInDateTime.Value.ToString("HHmm") : (string)null);
                    conditions.Add("@CARDTYPE", "01");
                    strSQL = _dc.ConstructInsertDML(tableName, conditions);
                    result.Add(strSQL, conditions);
                    break;
                // 下班
                case 2:
                    conditions.Add("@CARDDATE", forgotPunch.ForgotPunchOutDateTime.HasValue ? forgotPunch.ForgotPunchOutDateTime.Value.ToString("yyyyMMdd") : (string)null);
                    conditions.Add("@CARDTIME", forgotPunch.ForgotPunchOutDateTime.HasValue ? forgotPunch.ForgotPunchOutDateTime.Value.ToString("HHmm") : (string)null);
                    conditions.Add("@CARDTYPE", "02");
                    strSQL = _dc.ConstructInsertDML(tableName, conditions);
                    result.Add(strSQL, conditions);
                    break;
                // 全天
                case 3:
                    for (int i = 1; i < 3; i++)
                    {
                        conditions = new Conditions()
						{
							{ "@CARDNO", forgotPunch.EmployeeID_FK},
                            //20161104 志元改版，LostCard無EMPLOYECD欄位，新增Sequenceid紀錄電子簽核單號
							//{ "@EMPLOYECD", forgotPunch.EmployeeID_FK},
                            { "@Sequenceid", forgotPunch.SignDocID_FK},
                            //{ "@FLAG", "1"},
							{ String.Format("@CARDTYPE{0}", i), "0" + i.ToString()}
						};
                        if (i == 1)
                        {
                            conditions.Add("@CARDDATE", forgotPunch.ForgotPunchInDateTime.HasValue ? forgotPunch.ForgotPunchInDateTime.Value.ToString("yyyyMMdd") : (string)null);
                            conditions.Add("@CARDTIME", forgotPunch.ForgotPunchInDateTime.HasValue ? forgotPunch.ForgotPunchInDateTime.Value.ToString("HHmm") : (string)null);
                        }
                        else
                        {
                            conditions.Add("@CARDDATE", forgotPunch.ForgotPunchOutDateTime.HasValue ? forgotPunch.ForgotPunchOutDateTime.Value.ToString("yyyyMMdd") : (string)null);
                            conditions.Add("@CARDTIME", forgotPunch.ForgotPunchOutDateTime.HasValue ? forgotPunch.ForgotPunchOutDateTime.Value.ToString("HHmm") : (string)null);
                        }
                        strSQL = _dc.ConstructInsertDML(tableName, conditions);
                        result.Add(strSQL, conditions);
                    }

                    break;

                default:
                    throw new Exception("無此忘刷型別!");
            }

            //----------start----------

            var ForgotDate = new DateTime();
            if (forgotPunch.ForgotPunchInDateTime != null)
            {
                ForgotDate = forgotPunch.ForgotPunchInDateTime.Value;
            }
            else if (forgotPunch.ForgotPunchOutDateTime != null)
            {
                ForgotDate = forgotPunch.ForgotPunchOutDateTime.Value;
            }

            //抓班表資料
            var dutyWorkData = _smartRepo.QueryForDutyWork(ForgotDate.ToDateTimeFormateString("yyyyMMdd"), forgotPunch.EmployeeID_FK);
            //抓員工刷卡資料
            var EmpdailyOnOff = _smartRepo.QueryForDailyOnOff(ForgotDate.ToString("yyyyMMdd"), forgotPunch.EmployeeID_FK);
            //20170302 add新增dailyOnOff時寫入計薪年月
            var payRange = _smartRepo.QueryForPayRange(ForgotDate.ToDateTimeFormateString("yyyyMMdd"), ForgotDate.ToDateTimeFormateString("yyyyMMdd"));
            //var payYYYYMM = ParsePayRange(payRange["PayYYYYMM"].ToString());
            var result_dailyOnOff = new MultiConditions();
            string strSQL_dailyOnOff = null;
            var dateTimeNow = DateTime.Now;
            Conditions conditions_dailyOnOff = new Conditions()
                {
                    { "@SYSTEMDATE", dateTimeNow.ToString("yyyyMMdd")},
                    { "@SYSTEMTIME", dateTimeNow.ToString("HHmm")},
                    { "@COMPANYCD", "A"},
                    { "@EMPLOYECD", forgotPunch.EmployeeID_FK},
                    { "@LOGINUSER", "System"},
                    { "@APPROVEUSER", "System"},
                    { "@APPROVEYN", "Y"},
                    { "@DUTYDATE", ForgotDate.ToDateTimeFormateString("yyyyMMdd")},
                    //{ "@BEGINTIME", "Y"},
                    //{ "@ENDTIME", "Y"},
                    { "@SEQNOX", forgotPunch.SignDocID_FK},
                    { "@PAYYYYYMM", payRange["PayYYYYMM"].ToString()},
                    { "@WORKTYPE", dutyWorkData["WORKTYPE"].ToString()},
                    { "@HOLIDAY", dutyWorkData["HOLIDAY"].ToString()},
                    { "@H_TYPE", dutyWorkData["H_TYPE"]},
                    { "@overwork1", 0},
                    { "@overwork2", 0},
                    { "@overwork3", 0},
                    { "@overwork4", 0},
                    { "@overwork5", 0},
                    { "@recreatedays", 0},
                    { "@workhours", 0},
                    { "@workdays", 0},
                    { "@offwork1", 0},
                    { "@offwork2", 0},
                    { "@offwork3", 0},
                    { "@offwork4", 0},
                    { "@offwork5", 0},
                    { "@offwork6", 0},
                    { "@offwork7", 0},
                    { "@offwork8", 0},
                    { "@offwork9", 0},
                    { "@offwork10", 0},
                    { "@offwork5m", 0},
                    { "@offwork6m", 0},
                    { "@offhours", 0},
                    { "@addhours", 0},
                    { "@overworkhours", 0},
                    { "@mealdelay", 0},
                    { "@offwork11", 0},
                    { "@offwork12", 0},
                    { "@offworkhours", 0},
                    { "@taxhours", 0},
                    //{ "@losttimes", 0},
                    { "@FixedPay18", 0},
                    { "@FixedPay19", 0},
                    { "@FixedPay20", 0},
                    { "@FixedPay21", 0},
                    { "@FixedPay22", 0},
                    { "@RealOnDate", ForgotDate.ToDateTimeFormateString("yyyyMMdd")},
                    { "@RealOffDate", ForgotDate.ToDateTimeFormateString("yyyyMMdd")},
                    { "@OffWork14", 0},
                    { "@OffWork15", 0},
                };

            string tableName_dailyOnOff = RepositoryFactory.SmartManConn["DataSource"] + "." + RepositoryFactory.SmartManConn["Catelog"] + ".dbo.DailyOnOff";
            if (dutyWorkData["HOLIDAY"].ToString() == "Y")
            {
                //假日忘刷
                if (EmpdailyOnOff == null)
                {
                    // insert dailyOnOff
                    conditions_dailyOnOff.Add("@BEGINTIME", forgotPunch.ForgotPunchInDateTime.HasValue ? forgotPunch.ForgotPunchInDateTime.Value.ToString("HHmm") : (string)null);
                    conditions_dailyOnOff.Add("@ENDTIME", forgotPunch.ForgotPunchOutDateTime.HasValue ? forgotPunch.ForgotPunchOutDateTime.Value.ToString("HHmm") : (string)null);
                    conditions_dailyOnOff.Add("@losttimes", 0);

                    strSQL_dailyOnOff = String.Format(@"INSERT INTO {0}
                                                                    (SYSTEMDATE,
                                                                    SYSTEMTIME,
                                                                    COMPANYCD,
                                                                    EMPLOYECD,
                                                                    LOGINUSER,
                                                                    APPROVEUSER,
                                                                    APPROVEYN,
                                                                    DUTYDATE,
                                                                    SEQNOX,
                                                                    PAYYYYYMM,
                                                                    WORKTYPE,
                                                                    HOLIDAY,
                                                                    H_TYPE,
                                                                    overwork1,
                                                                    overwork2,
                                                                    overwork3,
                                                                    overwork4,
                                                                    overwork5,
                                                                    recreatedays,
                                                                    workhours,
                                                                    workdays,
                                                                    offwork1,
                                                                    offwork2,
                                                                    offwork3,
                                                                    offwork4,
                                                                    offwork5,
                                                                    offwork6,
                                                                    offwork7,
                                                                    offwork8,
                                                                    offwork9,
                                                                    offwork10,
                                                                    offwork5m,
                                                                    offwork6m,
                                                                    offhours,
                                                                    addhours,
                                                                    overworkhours,
                                                                    mealdelay,
                                                                    offwork11,
                                                                    offwork12,
                                                                    offworkhours,
                                                                    taxhours,
                                                                    FixedPay18,
                                                                    FixedPay19,
                                                                    FixedPay20,
                                                                    FixedPay21,
                                                                    FixedPay22,
                                                                    RealOnDate,
                                                                    RealOffDate,
                                                                    OffWork14,
                                                                    OffWork15,
                                                                    BEGINTIME,
                                                                    ENDTIME,
                                                                    losttimes)
                                                        VALUES
                                                                   (@SYSTEMDATE,
                                                                    @SYSTEMTIME,
                                                                    @COMPANYCD,
                                                                    @EMPLOYECD,
                                                                    @LOGINUSER,
                                                                    @APPROVEUSER,
                                                                    @APPROVEYN,
                                                                    @DUTYDATE,
                                                                    @SEQNOX,
                                                                    @PAYYYYYMM,
                                                                    @WORKTYPE,
                                                                    @HOLIDAY,
                                                                    @H_TYPE,
                                                                    @overwork1,
                                                                    @overwork2,
                                                                    @overwork3,
                                                                    @overwork4,
                                                                    @overwork5,
                                                                    @recreatedays,
                                                                    @workhours,
                                                                    @workdays,
                                                                    @offwork1,
                                                                    @offwork2,
                                                                    @offwork3,
                                                                    @offwork4,
                                                                    @offwork5,
                                                                    @offwork6,
                                                                    @offwork7,
                                                                    @offwork8,
                                                                    @offwork9,
                                                                    @offwork10,
                                                                    @offwork5m,
                                                                    @offwork6m,
                                                                    @offhours,
                                                                    @addhours,
                                                                    @overworkhours,
                                                                    @mealdelay,
                                                                    @offwork11,
                                                                    @offwork12,
                                                                    @offworkhours,
                                                                    @taxhours,
                                                                    @FixedPay18,
                                                                    @FixedPay19,
                                                                    @FixedPay20,
                                                                    @FixedPay21,
                                                                    @FixedPay22,
                                                                    @RealOnDate,
                                                                    @RealOffDate,
                                                                    @OffWork14,
                                                                    @OffWork15,
                                                                    @BEGINTIME,
                                                                    @ENDTIME,
                                                                    @losttimes)", tableName_dailyOnOff);
                    //strSQL_dailyOnOff = _dc.ConstructInsertDML(tableName_dailyOnOff, conditions_dailyOnOff);
                    result.Add(strSQL_dailyOnOff, conditions_dailyOnOff);
                }
                else
                {
                    //update dailyOnOff
                    switch (forgotPunch.PeriodType)
                    {
                        // 上班
                        case 1:
                            if (EmpdailyOnOff["BEGINTIME"].ToString() == "")
                            {
                                strSQL_dailyOnOff = String.Format(
                                                        @"Update {0} Set
                                                        Begintime = '{1}',
                                                        LOSTTIMES = 0,
                                                        SEQNOX  = '{2}'
                                                        Where employecd = @employecd and dutydate=@dutydate", tableName_dailyOnOff, forgotPunch.ForgotPunchInDateTime.HasValue ? forgotPunch.ForgotPunchInDateTime.Value.ToString("HHmm") : (string)null, forgotPunch.SignDocID_FK);
                                result.Add(strSQL_dailyOnOff, conditions_dailyOnOff);
                            }
                            //20170411 增加判斷事後補單(HR已先手動作業)，只更新單號
                            else if ((EmpdailyOnOff["BEGINTIME"].ToString() != "") && (EmpdailyOnOff["ENDTIME"].ToString() != ""))
                            {
                                strSQL_dailyOnOff = String.Format(
                                                        @"Update {0} Set
                                                        LOSTTIMES= ( CASE WORKTYPE WHEN 'A' THEN 0 ELSE LOSTTIMES END) ,
                                                        REMARKOFF='',
                                                        OFFWORK3=0,
                                                        SEQNOX  ='{1}'
                                                        Where employecd = @employecd and dutydate=@dutydate", tableName_dailyOnOff, forgotPunch.SignDocID_FK);
                                result.Add(strSQL_dailyOnOff, conditions_dailyOnOff);
                            }
                            else
                            {
                                strSQL_dailyOnOff = String.Format(
                                                        @"Update {0} Set
                                                        Begintime = '{1}',
                                                        ENDTIME = '{2}',
                                                        LOSTTIMES = 0,
                                                        SEQNOX  = '{3}'
                                                        Where employecd = @employecd and dutydate=@dutydate", tableName_dailyOnOff, forgotPunch.ForgotPunchInDateTime.HasValue ? forgotPunch.ForgotPunchInDateTime.Value.ToString("HHmm") : (string)null, EmpdailyOnOff["BEGINTIME"].ToString(), forgotPunch.SignDocID_FK);
                                result.Add(strSQL_dailyOnOff, conditions_dailyOnOff);
                                //conditions.Add("@CARDTIME", forgotPunch.ForgotPunchInDateTime.HasValue ? forgotPunch.ForgotPunchInDateTime.Value.ToString("HHmm") : (string)null);
                            }

                            break;
                        // 下班
                        case 2:
                            strSQL_dailyOnOff = String.Format(
                                                        @"Update {0} Set
                                                        ENDTIME = '{1}',
                                                        LOSTTIMES = 0,
                                                        SEQNOX  = '{2}'
                                                        Where employecd = @employecd and dutydate=@dutydate", tableName_dailyOnOff, forgotPunch.ForgotPunchOutDateTime.HasValue ? forgotPunch.ForgotPunchOutDateTime.Value.ToString("HHmm") : (string)null, forgotPunch.SignDocID_FK);
                            result.Add(strSQL_dailyOnOff, conditions_dailyOnOff);
                            break;
                        // 全天
                        case 3:
                            strSQL_dailyOnOff = String.Format(
                                                        @"Update {0} Set
                                                        Begintime = '{1}',
                                                        ENDTIME = '{2}',
                                                        LOSTTIMES = 0,
                                                        SEQNOX = '{3}'
                                                        Where employecd = @employecd and dutydate=@dutydate", tableName_dailyOnOff, forgotPunch.ForgotPunchInDateTime.HasValue ? forgotPunch.ForgotPunchInDateTime.Value.ToString("HHmm") : (string)null, forgotPunch.ForgotPunchOutDateTime.HasValue ? forgotPunch.ForgotPunchOutDateTime.Value.ToString("HHmm") : (string)null, forgotPunch.SignDocID_FK);
                            result.Add(strSQL_dailyOnOff, conditions_dailyOnOff);
                            break;

                        default:
                            throw new Exception("無此忘刷型別!");
                    }
                }
            }
            else
            {
                //上班日忘刷
                switch (forgotPunch.PeriodType)
                {
                    // 上班

                    case 1:
                        if ((EmpdailyOnOff["BEGINTIME"].ToString() == ""))
                        {
                            strSQL_dailyOnOff = String.Format(
                                                    @"Update {0} Set
                                                        Begintime = '{1}',
                                                        LOSTTIMES= ( CASE WORKTYPE WHEN 'A' THEN 0 ELSE LOSTTIMES END) ,
                                                        REMARKOFF='',
                                                        OFFWORK3=0,
                                                        SEQNOX  ='{2}'
                                                        Where employecd = @employecd and dutydate=@dutydate", tableName_dailyOnOff, forgotPunch.ForgotPunchInDateTime.HasValue ? forgotPunch.ForgotPunchInDateTime.Value.ToString("HHmm") : (string)null, forgotPunch.SignDocID_FK);
                            result.Add(strSQL_dailyOnOff, conditions_dailyOnOff);
                        }
                        //20170411 增加判斷事後補單(HR已先手動作業)，只更新單號
                        else if ((EmpdailyOnOff["BEGINTIME"].ToString() != "") && (EmpdailyOnOff["ENDTIME"].ToString() != ""))
                        {
                            strSQL_dailyOnOff = String.Format(
                                                    @"Update {0} Set
                                                        LOSTTIMES= ( CASE WORKTYPE WHEN 'A' THEN 0 ELSE LOSTTIMES END) ,
                                                        REMARKOFF='',
                                                        OFFWORK3=0,
                                                        SEQNOX  ='{1}'
                                                        Where employecd = @employecd and dutydate=@dutydate", tableName_dailyOnOff, forgotPunch.SignDocID_FK);
                            result.Add(strSQL_dailyOnOff, conditions_dailyOnOff);
                        }
                        else
                        {
                            strSQL_dailyOnOff = String.Format(
                                                    @"Update {0} Set
                                                        Begintime = '{1}',
                                                        ENDTIME = '{2}',
                                                        LOSTTIMES= ( CASE WORKTYPE WHEN 'A' THEN 0 ELSE LOSTTIMES END) ,
                                                        REMARKOFF='',
                                                        OFFWORK3=0,
                                                        SEQNOX  = '{3}'
                                                        Where employecd = @employecd and dutydate=@dutydate", tableName_dailyOnOff, forgotPunch.ForgotPunchInDateTime.HasValue ? forgotPunch.ForgotPunchInDateTime.Value.ToString("HHmm") : (string)null, EmpdailyOnOff["BEGINTIME"].ToString(), forgotPunch.SignDocID_FK);
                            result.Add(strSQL_dailyOnOff, conditions_dailyOnOff);
                            //conditions.Add("@CARDTIME", forgotPunch.ForgotPunchInDateTime.HasValue ? forgotPunch.ForgotPunchInDateTime.Value.ToString("HHmm") : (string)null);
                        }

                        break;
                    // 下班
                    case 2:
                        strSQL_dailyOnOff = String.Format(
                                                    @"Update {0} Set
                                                        ENDTIME = '{1}',
                                                        LOSTTIMES= ( CASE WORKTYPE WHEN 'A' THEN 0 ELSE LOSTTIMES END) ,
                                                        REMARKOFF='',
                                                        OFFWORK3=0,
                                                        SEQNOX  = '{2}'
                                                        Where employecd = @employecd and dutydate=@dutydate", tableName_dailyOnOff, forgotPunch.ForgotPunchOutDateTime.HasValue ? forgotPunch.ForgotPunchOutDateTime.Value.ToString("HHmm") : (string)null, forgotPunch.SignDocID_FK);
                        result.Add(strSQL_dailyOnOff, conditions_dailyOnOff);
                        break;
                    // 全天
                    case 3:
                        strSQL_dailyOnOff = String.Format(
                                                    @"Update {0} Set
                                                        Begintime = '{1}',
                                                        ENDTIME = '{2}',
                                                        LOSTTIMES= ( CASE WORKTYPE WHEN 'A' THEN 0 ELSE LOSTTIMES END) ,
                                                        REMARKOFF='',
                                                        OFFWORK3=0,
                                                        SEQNOX  = '{3}'
                                                        Where employecd = @employecd and dutydate=@dutydate", tableName_dailyOnOff, forgotPunch.ForgotPunchInDateTime.HasValue ? forgotPunch.ForgotPunchInDateTime.Value.ToString("HHmm") : (string)null, forgotPunch.ForgotPunchOutDateTime.HasValue ? forgotPunch.ForgotPunchOutDateTime.Value.ToString("HHmm") : (string)null, forgotPunch.SignDocID_FK);
                        result.Add(strSQL_dailyOnOff, conditions_dailyOnOff);
                        break;

                    default:
                        throw new Exception("無此忘刷型別!");
                }
            }

            //---------end---------

            return result;
        }

        public MultiConditions GetOvertimeDML(OvertimeViewModel overtime, DateTime payRange)
        {
            _rootRepo = RepositoryFactory.CreateRootRepo();
            var DeptDataLevel = _rootRepo.QueryForDepartmentByDeptID(overtime.SupportDeptID_FK);
            var dutyWorkData = _smartRepo.QueryForDutyWork(overtime.StartDateTime.Value.ToDateTimeFormateString("yyyyMMdd"), overtime.EmployeeID_FK);
            overtime.IsHoliday = "Y".Equals(dutyWorkData["HOLIDAY"].ToString());
            var HolidayType = dutyWorkData["H_TYPE"].ToString();
            string Level = DeptDataLevel != null ? DeptDataLevel["DepartmentLevel"].ToString() : String.Empty;
            string TrueSupportDept = "";
            if (DeptDataLevel["Virtual"].ToString() == "N")
            {
                TrueSupportDept = DeptDataLevel["DepartmentID"].ToString();
            }
            else
            {
                string UpDept = "";
                for (int i = Int32.Parse(Level) - 1; i >= 1; i--)
                {
                    if (UpDept == "")
                    {
                        var DeptData = _rootRepo.QueryForDepartmentByDeptID(DeptDataLevel["UpperDepartmentID"].ToString());
                        if (DeptData["Virtual"].ToString() == "N")
                        {
                            TrueSupportDept = DeptData["DepartmentID"].ToString();
                            break;
                        }
                        else
                        {
                            UpDept = DeptData["UpperDepartmentID"].ToString();
                        }
                    }
                    else
                    {
                        var DeptData = _rootRepo.QueryForDepartmentByDeptID(UpDept);
                        if (DeptData["Virtual"].ToString() == "N")
                        {
                            TrueSupportDept = DeptData["DepartmentID"].ToString();
                            break;
                        }
                        else
                        {
                            UpDept = DeptData["UpperDepartmentID"].ToString();
                        }
                    }
                }//迴圈END
            }
            string remark = "";
            if (overtime.Note.Length > 30)
            {
                remark = overtime.Note.Substring(0, 30);
            }
            else
            {
                remark = overtime.Note;
            }

            var result = new MultiConditions();
            string strSQL = null;
            var dateTimeNow = DateTime.Now;
            Conditions conditions = new Conditions()
				{
					{ "@SYSTEMDATE", dateTimeNow.ToString("yyyyMMdd")},
					{ "@SYSTEMTIME", dateTimeNow.ToString("HHmm")},
					{ "@LOGINUSER", "System"},
					{ "@APPROVEUSER", "System"},
					{ "@APPROVEYN", "Y"},
					{ "@COMPANYCD", "A"},
					{ "@EMPLOYECD", overtime.EmployeeID_FK},
					{ "@PAYYYYYMM", payRange.ToString("yyyyMM")},
					{ "@DUTYDATE", overtime.StartDateTime.Value.ToString("yyyyMMdd")},
					{ "@BEGINTIME", overtime.BeginTime},
					{ "@ENDTIME", overtime.EndTime},
					{ "@OVERONWORK", overtime.StartDateTime.Value.ToString("HHmm")},
					{ "@OVEROFFWORK", overtime.EndDateTime.Value.ToString("HHmm")},
					{ "@OVERWORKHOURS", overtime.TotalHours},
                    //20170109 加班說明寫入志元只可30個字
					//{ "@REMARK", overtime.Note.Substring(0,30)},
                    { "@REMARK", remark},
                    //20161104 修正單號紀錄
					//{ "@SEQNOX", String.Concat("OT",dateTimeNow.ToString("yyyyMMdd"), dateTimeNow.ToString("HHmmss"))},
                    { "@SEQNOX", overtime.SignDocID_FK},
					//{ "@HOLIDAY", overtime.IsHoliday },
                    { "@HOLIDAY", overtime.IsHoliday ? "Y" : "N"},
                    //20161028 修正寫入志元table部門
					//{ "@UNITCD", overtime.DepartmentID_FK},
                    { "@UNITCD", overtime.CostDepartmentID},
					{ "@COSTCD",  TrueSupportDept},
					{ "@WORKTYPE", overtime.WorkType},
                    { "@H_TYPE", HolidayType},
				};

            string tableName = RepositoryFactory.SmartManConn["DataSource"] + "." + RepositoryFactory.SmartManConn["Catelog"] + ".dbo.DailyOn";
            setConditions(overtime, payRange, ref conditions);

            var dailyOnData = QueryForOvertimeRecord(overtime, tableName);

            if (dailyOnData == null)
            {
                // insert
                strSQL = _dc.ConstructInsertDML(tableName, conditions);
            }
            else
            {
                // update
                var distinglishType = Double.Parse(dailyOnData["ADDHOURS"].ToString()) > 0 ? "OVERTIMELEAVE" : "OVERTIMEPAY";
                var overonworkArray = new string[] { overtime.StartDateTime.Value.ToString("HHmm"), (string)dailyOnData["OVERONWORK"] };
                var overoffworkArray = new string[] { overtime.EndDateTime.Value.ToString("HHmm"), (string)dailyOnData["OVEROFFWORK"] };

                //替換加班申請單 開始與結束的時間
                conditions["@OVERONWORK"] = overonworkArray.Min();
                conditions["@OVEROFFWORK"] = overoffworkArray.Max();

                switch (overtime.PayTypeKey.ToUpper() + distinglishType)
                {
                    case "OVERTIMEPAYOVERTIMEPAY":
                        strSQL = String.Format(
@"Update {0} Set
SYSTEMDATE = @SYSTEMDATE,
SYSTEMTIME = @SYSTEMTIME,
OVERONWORK  = @OVERONWORK,
OVEROFFWORK = @OVEROFFWORK,
OVERWORKone = OVERWORKone + @OVERWORKone,
OVERWORKtwo = OVERWORKtwo + @OVERWORKtwo,
OVERWORKfour = OVERWORKfour + @OVERWORKfour,
OVERWORKHOURS = OVERWORKHOURS + @OVERWORKHOURS,
REMARK = REMARK +@REMARK
Where EMPLOYECD = @EMPLOYECD and DUTYDATE=@DUTYDATE", tableName);

                        break;

                    case "OVERTIMELEAVEOVERTIMELEAVE":

                        strSQL = String.Format(
@"Update {0} Set
SYSTEMDATE = @SYSTEMDATE,
SYSTEMTIME = @SYSTEMTIME,
OVERONWORK  = @OVERONWORK,
OVEROFFWORK = @OVEROFFWORK,
OVERWORKHOURS = OVERWORKHOURS + @OVERWORKHOURS,
ADDHOURS = ADDHOURS + @ADDHOURS,
REMARK = REMARK +@REMARK
Where EMPLOYECD = @EMPLOYECD and DUTYDATE=@DUTYDATE ", tableName);

                        break;

                    default:
                        throw new Exception("報酬型別選擇錯誤!");
                }
            }

            if (conditions.ContainsKey("@OVERWORKone"))//1.34
            {
                strSQL = strSQL.Replace("OVERWORKone", "OVERWORK1");
                conditions.Add("@OVERWORK1", conditions["@OVERWORKone"]);
                conditions.Remove("@OVERWORKone");
            }
            else { strSQL = Regex.Replace(strSQL, @"(?m)^OVERWORKone.*?,", ""); }

            if (conditions.ContainsKey("@OVERWORKtwo"))//1.67
            {
                strSQL = strSQL.Replace("OVERWORKtwo", "OVERWORK2");
                conditions.Add("@OVERWORK2", conditions["@OVERWORKtwo"]);
                conditions.Remove("@OVERWORKtwo");
            }
            else { strSQL = Regex.Replace(strSQL, @"(?m)^OVERWORKtwo.*?,", ""); }

            if (conditions.ContainsKey("@OVERWORKthree"))//2.67
            {
                strSQL = strSQL.Replace("OVERWORKthree", "OVERWORK3");
                conditions.Add("@OVERWORK3", conditions["@OVERWORKthree"]);
                conditions.Remove("@OVERWORKthree");
            }
            else { strSQL = Regex.Replace(strSQL, @"(?m)^OVERWORKthree.*?,", ""); }

            if (conditions.ContainsKey("@OVERWORKfour"))
            {
                strSQL = strSQL.Replace("OVERWORKfour", "OVERWORK4");
                conditions.Add("@OVERWORK4", conditions["@OVERWORKfour"]);
                conditions.Remove("@OVERWORKfour");
            }
            else { strSQL = Regex.Replace(strSQL, @"(?m)^OVERWORKfour.*?,", ""); }

            result.Add(strSQL, conditions);

            return result;
        }

        // 受訓單結案時作的事情寫在這
        private MultiConditions getTrainDML(string signDocID)
        {
            var trainData = _rootRepo.QueryForTrainDataBySignDocID(signDocID);
            var result = new MultiConditions();
            string strSQL = null;
            string tableName = RepositoryFactory.TrainConn["Catelog"] + "..FORM_SIGN";
            Conditions conditions = new Conditions()
			{
				{ "@SignDocID", signDocID},
                { "@Edit_Person", "Portal"}
            };
            if (trainData != null)
            {
                // update 簽核狀態
                strSQL = String.Format(
                                        @"Update {0} Set
                                        IsSigned = 'True',
                                        Edit_Person=@Edit_Person,
                                        Edit_Date=getdate()
                                        Where SignDocID = @SignDocID ", tableName);
                result.Add(strSQL, conditions);
            }

            return result;
        }

        public MultiConditions GetXACTABORTON()
        {
            return new MultiConditions() { { @"SET XACT_ABORT ON", null } };
        }

        public DateTime ParsePayRange(string payYYYYMM)
        {
            var payRange = DateTime.MinValue;
            var rangeDate = String.Concat(payYYYYMM, "01000000").TryParseDateTimeExact(ref payRange);
            if (payRange == DateTime.MinValue) { throw new Exception("薪資歸入月份設定轉換格式失敗!"); }

            return payRange;
        }

        public bool IsSettledAccounts(string empID, DateTime payRange)
        {
            //該員工該月是否已經結算薪資
            var payData = _smartRepo.QueryForEmployeePay(empID, payRange.Date.ToString("yyyyMM"));
            if (payData != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public DataRow QueryForOvertimeRecord(OvertimeViewModel overtime, string tableName)
        {
            if (overtime == null) { throw new Exception("無加班單資料"); }
            var conditions = new Conditions()
			{
				{ "@EMPLOYECD", overtime.EmployeeID_FK},
				{ "@DUTYDATE", overtime.StartDateTime.Value.ToString("yyyyMMdd")},
			};
            var strSQL = String.Format("Select * from {0} where Employecd = @Employecd and dutyDate = @dutydate", tableName);
            var data = _dc.QueryForDataRow(strSQL, conditions);
            if (data == null) { return null; }

            return data;
        }

        private void setConditions(OvertimeViewModel overtime, DateTime payRange, ref Conditions conditions)
        {
            switch (overtime.PayTypeKey.ToUpper())
            {
                case "OVERTIMEPAY":
                    conditions.Add("@RESTYN", 'N');
                    var dutyWorkData = _smartRepo.QueryForDutyWork(overtime.StartDateTime.Value.ToDateTimeFormateString("yyyyMMdd"), overtime.EmployeeID_FK);
                    var OverWorkData = _smartRepo.QueryOverWorkByDate(overtime.StartDateTime.Value.ToDateTimeFormateString("yyyyMMdd"), overtime.EmployeeID_FK);
                    overtime.IsHoliday = "Y".Equals(dutyWorkData["HOLIDAY"].ToString());

                    //var OVERWORK2 = OverWorkData["OVERWORK2"].ToString();
                    if (!overtime.IsHoliday)
                    {
                        var totalHR = overtime.TotalHours;
                        if (OverWorkData == null)//當日未申報過加班
                        {
                            var start2 = 2;
                            //1.33
                            conditions.Add("@OVERWORKone", (totalHR - 2 >= 0) ? start2 : totalHR);
                            //1.66
                            conditions.Add("@OVERWORKtwo", (totalHR - start2 > 0) ? totalHR - start2 : 0);
                        }
                        else
                        {
                            var OVERWORK1 = OverWorkData["OVERWORK1"].ToString();
                            if (OVERWORK1 == "1.00")//當日申報過加班
                            {
                                //1.33
                                conditions.Add("@OVERWORKone", 1);
                                //1.66
                                conditions.Add("@OVERWORKtwo", (totalHR - 1 > 0) ? totalHR - 1 : 0);
                            }
                            else if (OVERWORK1 == "2.00")//當日申報過加班
                            {
                                //1.66
                                conditions.Add("@OVERWORKtwo", totalHR);
                            }
                            else if (OVERWORK1 == "0.50")
                            {
                                var start2 = 1.5;
                                //1.33
                                conditions.Add("@OVERWORKone", (totalHR - 1.5 >= 0) ? start2 : totalHR);
                                //conditions.Add("@OVERWORKone", 0.5);
                                //1.66
                                conditions.Add("@OVERWORKtwo", (totalHR - start2 > 0) ? totalHR - start2 : 0);
                            }
                        }
                    }
                    else
                    {
                        var totalHR = overtime.TotalHours;
                        //20161223 一例一休-國定假日加班時數寫入志元假日加班欄位前8H 1倍，超過8H 前2H 1.34 剩餘1.67 (20161223實施)
                        //20161223 一例一休-休息日加班時數寫入志元前2H1.33、剩餘1.66欄位(20161223實施)
                        //var dutyWorkData = _smartRepo.QueryForDutyWork(overtime.StartDateTime.Value.ToDateTimeFormateString("yyyyMMdd"), overtime.EmployeeID_FK);
                        var HolidayType = dutyWorkData["H_TYPE"].ToString();

                        if (HolidayType == "2")//20161223 一例一休-國定假日加班時數寫入志元假日加班欄位 8H 1倍
                        {
                            if (totalHR - 8 >= 0)
                            {
                                //1倍
                                conditions.Add("@OVERWORKfour", 8);
                            }
                            totalHR -= 8;
                        }
                        else if (HolidayType == "1")//休息日8H後以2.67計算
                        {
                            var start2 = 2;
                            //1.33
                            conditions.Add("@OVERWORKone", (totalHR - 2 >= 0) ? start2 : totalHR);
                            //1.66
                            conditions.Add("@OVERWORKtwo", (totalHR - 8 > 0) ? totalHR - start2 - 4 : totalHR - start2);

                            totalHR -= 8;

                            if (totalHR > 0)
                            {
                                conditions.Add("@OVERWORKthree", totalHR);
                            }
                        }
                        else
                        {
                            if (totalHR > 0)
                            {
                                var start2 = 2;
                                //1.33
                                conditions.Add("@OVERWORKone", (totalHR - 2 >= 0) ? start2 : totalHR);
                                //1.66
                                conditions.Add("@OVERWORKtwo", (totalHR - start2 > 0) ? totalHR - start2 : 0);
                            }
                        }
                        ////if (HolidayType == "1")
                        ////{
                        //if (totalHR > 0)
                        //{
                        //    var start2 = 2;
                        //    //1.33
                        //    conditions.Add("@OVERWORKone", (totalHR - 2 >= 0) ? start2 : totalHR);
                        //    //1.66
                        //    conditions.Add("@OVERWORKtwo", (totalHR - start2 > 0) ? totalHR - start2 : 0);
                        //}
                        ////}
                    }
                    break;

                case "OVERTIMELEAVE":

                    conditions.Add("@RESTYN", "Y");
                    conditions.Add("@ADDHOURS", overtime.TotalHours);

                    #region #0009 補修有效日期 6個月後的20日

                    string limitDateStr = payRange.AddMonths(6).ToString("yyyyMM20");

                    #endregion #0009 補修有效日期 6個月後的20日

                    // 結薪月後推6個月
                    conditions.Add("@CHANGEDATE", limitDateStr);

                    break;

                default:
                    throw new Exception("無此報酬型別!");
            }
        }
    }
}