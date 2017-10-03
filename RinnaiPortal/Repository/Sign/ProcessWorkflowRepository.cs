using DBTools;
using RinnaiPortal.Extensions;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository.Sign.Forms;
using RinnaiPortal.Repository.SmartMan;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RinnaiPortal.Repository
{
    public class ProcessWorkflowRepository : ISignRepository
    {
        private DB _dc { get; set; }
        private RootRepository _rootRepo { get; set; }

        public ProcessWorkflowRepository(DB dc, RootRepository rootRepo)
        {
            _dc = dc;
            _rootRepo = rootRepo;
        }

        #region #0012 簽核之前確認是否符合班別的時間

        /// <summary>
        /// 簽核之前確認是否符合班別的時間
        /// </summary>
        /// <param name="SignDocID"></param>
        /// <returns></returns>
        public bool CheckFogotPunchTimeHasInRule(string SignDocID)
        {
            bool chkIsInRule = true;
            try
            {
                ForgotPunchViewModel model = null;
                ForgotPunchRepository _forgotRepo = null;
                _forgotRepo = RepositoryFactory.CreateForgotPunchRepo();
                //根據查詢的 簽核代碼 搜尋忘刷單
                model = _forgotRepo.GetForgotPunchForm(SignDocID);
                if (model == null)
                    throw new Exception("查無相關忘刷單資料");
                string empID = model.EmployeeID_FK;

                string workDatStr = string.Empty;

                DateTime nFpDateInTime = new DateTime();
                DateTime nFpDateOutTime = new DateTime();
                string fFirstTime = string.Empty;
                string fLastTime = string.Empty;
                if (model.ForgotPunchInDateTime == null && model.ForgotPunchOutDateTime == null)
                    throw new Exception("無法取得忘間.");

                if (model.ForgotPunchInDateTime != null)
                {
                    nFpDateInTime = (DateTime)model.ForgotPunchInDateTime;
                    fFirstTime = string.Format("{0}{1}", model.ForgotPunchInDateTime.Value.Hour.ToString().PadLeft(2, '0'), model.ForgotPunchInDateTime.Value.Minute.ToString().PadLeft(2, '0'));
                }
                if (model.ForgotPunchOutDateTime != null)
                {
                    nFpDateOutTime = (DateTime)model.ForgotPunchOutDateTime;
                    fLastTime = string.Format("{0}{1}", model.ForgotPunchOutDateTime.Value.Hour.ToString().PadLeft(2, '0'), model.ForgotPunchOutDateTime.Value.Minute.ToString().PadLeft(2, '0'));
                }

                //取得志元班表時間
                var smartDc = ConnectionFactory.GetSmartManDC();

                if (model.ForgotPunchInDateTime != null)
                    workDatStr = nFpDateInTime.ToString("yyyyMMdd");
                if (model.ForgotPunchOutDateTime != null)
                    workDatStr = nFpDateOutTime.ToString("yyyyMMdd");

                string strSQL = @"
                        select w.morningtime, w.offworktime from dutywork d
                        inner join worktime w on w.worktype = d.worktype
                        where d.employecd = @EmployeeCD
                        and d.workdate = @WorkDate
                        ";
                var strCondition = new Conditions() {
                { "@EmployeeCD", empID } ,
                { "@WorkDate", workDatStr } ,
                };

                DataRow result = smartDc.QueryForDataRow(strSQL, strCondition);
                var ruleWorkFirstTime = result["morningtime"];
                var ruleWorkLastTime = result["offworktime"];
                //CompareTo 大於會回傳1 不判斷上班時間 20170824 by 小遇
                //if (!string.IsNullOrEmpty(fFirstTime))
                //    chkIsInRule = fFirstTime.CompareTo(ruleWorkFirstTime.ToString()) > 0 ? false : true;
                if (!string.IsNullOrEmpty(fLastTime))
                    chkIsInRule = fLastTime.CompareTo(ruleWorkLastTime.ToString()) < 0 ? false : true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return chkIsInRule;
        }

        #endregion #0012 簽核之前確認是否符合班別的時間

        //列表
        public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
        {
            string strSQL =
@"select distinct signM.*,emp.EmployeeName EmployeeName, typ.FormType FormType, dept.DepartmentName CurrentSignLevelDeptName, dept.ChiefID_FK, chief.ADAccount chiefADAccount, signD.Status
from SignForm_Main signM
left outer join SignForm_Detail signD on signM.SignDocID = signD.SignDocID_FK
left outer join Employee emp on signM.EmployeeID_FK = emp.EmployeeID
left outer join SignType typ on signM.FormID_FK = typ.FormID
left outer join Department dept on signM.CurrentSignLevelDeptID_FK = dept.DepartmentID
left outer join Employee chief on signD.ChiefID_FK = chief.EmployeeID
 {0}";

            string strConditions = " where chief.ADAccount = @ADAccount and signD.Status = 2 ";
            var conditionsDic = new Conditions()
			{
				{ "@ADAccount", slParms.Member.ADAccount}
			};
            //組成查詢條件 SQL
            if (!String.IsNullOrWhiteSpace(slParms.QueryText))
            {
                conditionsDic.Add("@queryText", String.Format("{0}%", slParms.QueryText.Trim()));
                //20170221 修正查詢條件(避免查詢到未送簽)
                strConditions += String.Format(" and (EmployeeID_FK like @queryText or emp.EmployeeName like @queryText or SignDocID like @queryText)");
            }

            var paginationParms = new PaginationParms()
            {
                QueryString = String.Format(strSQL, strConditions),
                QueryConditions = conditionsDic,
                PageIndex = pParms.PageIndex,
                PageSize = pParms.PageSize
            };

            string orderExpression = String.Format("{0}{1}", pParms.Descending ? "-" : "", pParms.OrderField);

            var allowColumns = new List<string>
			{
				"SignDocID", "FormType", "EmployeeID_FK", "EmployeeName", "SendDate", "CurrentSignLevelDeptID_FK",
				"CurrentSignLevelDeptName", "FinalStatus", "Creator", "CreateDate","Modifier","ModifyDate"
			};
            //根據 SQL取得　Pagination
            return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
        }

        //明細是否存在主管簽核(針對跨部門相同主管)
        public ProcessWorkflowViewModel GetChiefID(string SignDocID_FK, string ChiefID_FK)
        {
            var conditions = new Conditions() { { "@SignDocID_FK", SignDocID_FK },
                                                { "@ChiefID_FK", ChiefID_FK }};
            var sql = @"Select * from SignForm_Detail where SignDocID_FK = @SignDocID_FK and ChiefID_FK=@ChiefID_FK";
            var rows = _dc.QueryForRowsCount(sql, conditions);
            ProcessWorkflowViewModel model = new ProcessWorkflowViewModel()
            {
            };
            return model;
        }

        public ProcessWorkflowViewModel GetWorkflowData(string signDocID, string status = "2")
        {
            string strSQL =
            @"select m.*, t.SignID_FK as SignID_FK from signform_main m
             left outer join signtype t on m.formid_FK = t.formid
             where m.SignDocID = @SignDocID ";
            var strCondition = new Conditions() { { "@SignDocID", signDocID } };

            if (!String.IsNullOrEmpty(status))
            {
                strSQL = String.Concat(strSQL, " and m.FinalStatus = @FinalStatus");
                strCondition.Add("@FinalStatus", status);
            }

            var result = _dc.QueryForDataRow(strSQL, strCondition);
            if (result == null)
            {
                return null;
            }

            ProcessWorkflowViewModel model = new ProcessWorkflowViewModel()
            {
                SignDocID = result["SignDocID"].ToString(),
                FormID_FK = Int32.Parse(result["FormID_FK"].ToString()),
                EmployeeID_FK = result["EmployeeID_FK"].ToString(),
                SendDate = !result["SendDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["SendDate"].ToString()) : (DateTime?)null,
                CurrentSignLevelDeptID_FK = result["CurrentSignLevelDeptID_FK"].ToString(),
                FinalStatus = Int32.Parse(result["FinalStatus"].ToString()),
                Remainder = Int32.Parse(result["Remainder"].ToString()),
                RuleID_FK = result["SignID_FK"].ToString(),
                Creator = result["Creator"].ToString(),
                CreateDate = DateTime.Parse(result["CreateDate"].ToString()),
                Modifier = result["Modifier"].ToString(),
                ModifyDate = !result["ModifyDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ModifyDate"].ToString()) : (DateTime?)null,
                WorkflowDetailList = GetWorkflowDataDetail((string)result["SignDocID"])
            };

            return model;
        }

        public ProcessWorkflowViewModel GetWorkflowDataAndCheck(string signDocID, string adAccount)
        {
            ProcessWorkflowViewModel model = null;
            string strSQL =
@"SELECT *
FROM   signform_main
WHERE  signdocid = @SignDocID AND finalstatus = 2 AND
	EXISTS (SELECT *
			FROM   signform_detail
			WHERE  signdocid_fk = @SignDocID AND chiefid_fk = @ChiefID_FK AND status = 2) ";

            var chiefData = _rootRepo.QueryForEmployeeByADAccount(adAccount);
            var strCondition = new Conditions()
			{
				{"@SignDocID", signDocID},
				{"@ChiefID_FK", chiefData != null ? chiefData["EmployeeID"].ToString() : (string)null}
			};

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

        //是否存在簽核代碼
        public bool IsExistSignDocID(string signDocID)
        {
            var conditions = new Conditions() { { "@SignDocID", signDocID } };
            var sql = @"select * from SignForm_Main where SignDocID = @SignDocID";
            var rows = _dc.QueryForRowsCount(sql, conditions);
            return (rows > 0) ? true : false;
        }

        //強制刪除記錄 main / detail / log / kind of form data /
        public bool DeleteRecordForce(IForms form, string signDocID)
        {
            var mainpulationConditions = new List<MultiConditions>();
            mainpulationConditions.Add(form.DeleteData(signDocID));
            mainpulationConditions.Add(new MultiConditions() { { "Delete From signform_detail Where SignDocID_FK = @SignDocID_FK", new Conditions() { { "@SignDocID_FK", signDocID } } } });
            mainpulationConditions.Add(new MultiConditions() { { "Delete From signform_main Where SignDocID = @SignDocID", new Conditions() { { "@SignDocID", signDocID } } } });

            return _dc.ExecuteMultAndCheck(mainpulationConditions);
        }

        /// <summary>
        /// 檢查是否符合簽核規則
        /// </summary>
        /// <param name="ruleID"></param>
        /// <param name="departmentID"></param>
        /// <param name="actualRemainder"></param>
        /// <returns></returns>
        public bool IsFollowFlowRule(string ruleID, string departmentID, int actualRemainder = Int32.MaxValue)
        {
            var ruleData = _rootRepo.QueryForSignProcedureByRuleID(ruleID);
            var deptData = _rootRepo.QueryForDepartmentByDeptID(departmentID);
            //查無簽核規則或是部門資料 => 不符合規則
            if (ruleData == null || deptData == null) { return false; }
            //MaxLevel 最高簽核層級
            var maxLevel = Int32.Parse(ruleData["MaxLevel"].ToString());
            var currentLevel = Int32.Parse(deptData["DepartmentLevel"].ToString());

            //簽核部門層級 小於 最大簽核層級 => 不符合規則
            if (currentLevel < maxLevel) { return false; }

            // 剩餘層數不為預設值 且 小於等於 0 => 不符合規則
            if (actualRemainder != Int32.MaxValue && actualRemainder <= 0) { return false; }

            return true;
        }

        //尋找上層主管
        public Dictionary<string, string> FindUpperDeptData(string departmentID, string employeeID = "0000")
        {
            var deptData = _rootRepo.QueryForDepartmentByDeptID(departmentID);
            if (!employeeID.Equals(deptData["ChiefID_FK"]))
            {
                return new Dictionary<string, string>() { { deptData["ChiefID_FK"].ToString(), deptData["UpperDepartmentID"].ToString() } };
            }
            else if (Int32.Parse(deptData["DepartmentLevel"].ToString()) < 0)
            {
                return null;
            }
            return FindUpperDeptData(deptData["UpperDepartmentID"].ToString(), deptData["ChiefID_FK"].ToString());
        }

        /// <summary>
        /// 本次簽核結果SQL語句
        /// </summary>
        /// <param name="detailModel"></param>
        /// <returns></returns>
        public List<MultiConditions> GetSignOffDML(ProcessWorkflowDetailViewModel detailModel)
        {
            var result = new List<MultiConditions>();
            detailModel.ModifyDate = DateTime.Now;

            #region 產生更新簽核明細SQL語句

            string signDetailStrSQL = @"
                            Update SignForm_Detail Set
                            Status = @Status,
                            Remark = @Remark,
                            Modifier = @Modifier,
                            ModifyDate = @ModifyDate
                            Where SignDocID_FK = @SignDocID_FK and ChiefID_FK = @ChiefID_FK
                            ";

            Conditions signDetailConditions = new Conditions()
            {
            		{ "@SignDocID_FK", detailModel.SignDocID_FK },
					{ "@Status", detailModel.Status },
					{ "@ChiefID_FK", detailModel.ChiefID_FK },
					{ "@Remark", detailModel.Remark },
					{ "@Modifier", detailModel.Modifier},
					{ "@ModifyDate", detailModel.ModifyDate}
            };

            MultiConditions signDetail = new MultiConditions() { { signDetailStrSQL, signDetailConditions } };

            #endregion 產生更新簽核明細SQL語句

            result.Add(signDetail);

            detailModel.Creator = (string)null;
            detailModel.CreateDate = new DateTime();

            #region 產生新增SignLog新增SQL語句

            Conditions conditions = new Conditions()
			{
				{ "@DetailSignDocID_FK", detailModel != null ? detailModel.SignDocID_FK : (string)null},
				{ "@ChiefID_FK", detailModel != null ? detailModel.ChiefID_FK : (string)null},
				{ "@Remark", detailModel != null ? detailModel.Remark : (string)null},
				{ "@Status", detailModel != null ? detailModel.Status.ToString() : (string)null},
				{ "@Creator_Detail", detailModel != null ? detailModel.Creator : (string)null},
				{ "@CreateDate_Detail", detailModel != null && detailModel.CreateDate != DateTime.MinValue ? detailModel.CreateDate.FormatDatetime() : (string)null},
				{ "@Modifier_Detail", detailModel != null ? detailModel.Modifier : (string)null},
				{ "@ModifyDate_Detail", detailModel != null && detailModel.ModifyDate != DateTime.MinValue ? detailModel.ModifyDate.Value.FormatDatetime(): (string)null},
				{ "@LogDatetime", DateTime.Now},
			};

            // ConstructInsertDML => 產生新增語句
            string logDML = _dc.ConstructInsertDML("SignForm_Log", conditions);

            #endregion 產生新增SignLog新增SQL語句

            result.Add(new MultiConditions() { { logDML, conditions } });

            return result;
        }

        //駁回SQL
        public List<MultiConditions> GetRejectSQLDML(ProcessWorkflowViewModel mainModel, string orgSupervisorID)
        {
            var result = new List<MultiConditions>();

            //取得原始簽核規則
            var procedureData = _rootRepo.QueryForSignProcedureBySignDocID(mainModel.SignDocID);
            mainModel.Remainder = Int32.Parse(procedureData["SignLevel"].ToString());
            mainModel.ModifyDate = DateTime.Now;
            mainModel.Modifier = _rootRepo.QueryForEmployeeByEmpID(orgSupervisorID)["ADAccount"].ToString();

            //駁回後即送回原部門 改Main 為最初設定 狀態為駁回
            var sql =
                    @"UPDATE signform_main
                    SET    finalstatus = @FinalStatus,
                    currentsignleveldeptid_fk = @CurrentSignLevelDeptID_FK,
                    remainder = @remainder,
                    modifier = @Modifier,
                    modifydate = @ModifyDate
                    WHERE  signdocid = @SignDocID ";
            var conditions = new Conditions();
            conditions.Add("@FinalStatus", mainModel.FinalStatus);
            conditions.Add("@CurrentSignLevelDeptID_FK", mainModel.CurrentSignLevelDeptID_FK);
            conditions.Add("@Remainder", mainModel.Remainder);
            conditions.Add("@Modifier", mainModel.Modifier);
            conditions.Add("@ModifyDate", mainModel.ModifyDate);
            conditions.Add("@SignDocID", mainModel.SignDocID);
            result.Add(new MultiConditions() { { sql, conditions } });

            //刪除Detail資料
            sql =
@"Delete SignForm_Detail Where SignDocID_FK = @SignDocID ";
            conditions = new Conditions();
            conditions.Add("@SignDocID", mainModel.SignDocID);
            result.Add(new MultiConditions() { { sql, conditions } });

            mainModel.Creator = (string)null;
            mainModel.CreateDate = new DateTime();

            //log
            conditions = new Conditions()
			{
				{ "@SignDocID_FK", mainModel !=null ? mainModel.SignDocID : (string)null},
				{ "@FormID_FK", mainModel !=null ? mainModel.FormID_FK.ToString() : (string)null},
				{ "@EmployeeID_FK", mainModel !=null ? mainModel.EmployeeID_FK : (string)null},
				{ "@SendDate", mainModel !=null ? mainModel.SendDate.Value.FormatDatetime() :(string)null},
				{ "@CurrentSignLevelDeptID_FK", mainModel !=null ? mainModel.CurrentSignLevelDeptID_FK : (string)null},
				{ "@FinalStatus", mainModel !=null ? mainModel.FinalStatus.ToString() : (string)null},
				{ "@Remainder", mainModel !=null ? mainModel.Remainder.ToString() :(string)null},
				{ "@Creator_Main", mainModel !=null ? mainModel.Creator : (string)null},
				{ "@CreateDate_Main", mainModel !=null && mainModel.CreateDate != DateTime.MinValue ? mainModel.CreateDate.FormatDatetime() : (string)null},
				{ "@Modifier_Main", mainModel !=null ? mainModel.Modifier : (string)null},
				{ "@ModifyDate_Main", mainModel !=null && mainModel.ModifyDate != DateTime.MinValue ? mainModel.ModifyDate.Value.FormatDatetime() :(string)null},
				{ "@LogDatetime", DateTime.Now},
			};

            var log = _dc.ConstructInsertDML("SignForm_Log", conditions);
            result.Add(new MultiConditions() { { log, conditions } });

            return result;
        }

        /// <summary>
        /// 上呈簽核SQL(判斷是否還有上層需要簽核)
        /// </summary>
        /// <param name="mainModel"></param>
        /// <param name="currentSignerID"></param>
        /// <returns></returns>
        public List<MultiConditions> GetNextSQLDML(ProcessWorkflowViewModel mainModel, string currentSignerID)
        {
            var result = new List<MultiConditions>();
            //取出指定明細
            //var detailModel = mainModel.WorkflowDetailList.Single(row => currentSignerID.Equals(row.ChiefID_FK));

            #region 0020 會有多筆資料 不該用Single

            var detailModel = mainModel.WorkflowDetailList.FirstOrDefault(row => currentSignerID.Equals(row.ChiefID_FK));

            #endregion 0020 會有多筆資料 不該用Single

            //更新時間
            mainModel.ModifyDate = DateTime.Now;
            detailModel.ModifyDate = mainModel.ModifyDate;
            mainModel.Creator = null;
            mainModel.CreateDate = new DateTime();

            //更新修改人員
            mainModel.Modifier = detailModel.Modifier;

            //找尋上層簽核部門資料
            var upperDeptData = FindUpperDeptData(mainModel.CurrentSignLevelDeptID_FK, currentSignerID);
            var isFollowRule = IsFollowFlowRule(mainModel.RuleID_FK, upperDeptData.Values.Single(), mainModel.Remainder);

            var sql = String.Empty;
            Conditions conditions = null;
            //確認當前部門是否符合上呈簽核規則
            if (isFollowRule)
            {
                //符合上呈簽核規則 卻沒有上層簽核 => 發生Exception
                if (upperDeptData == null || upperDeptData.Count == 0)
                {
                    throw new MissingMemberException(String.Format("簽核發生符合規則卻找不到上層簽核部門的例外!"));
                }

                //符合上呈簽核規則 且 有上層簽核

                //確認是否有額外的會簽人員
                var otherChiefCount = GetWorkflowDataDetail(mainModel.SignDocID).Count;
                if (otherChiefCount > 1)
                {
                    //刪除不是當前簽核主管的會簽人員
                    sql =
                    @"Delete SignForm_Detail Where SignDocID_FK = @SignDocID_FK and ChiefID_FK <> @OrgSupervisorID ";
                    conditions = new Conditions();
                    conditions.Add("@SignDocID_FK", detailModel.SignDocID_FK);
                    conditions.Add("@OrgSupervisorID", currentSignerID);
                    result.Add(new MultiConditions() { { sql, conditions } });
                }

                //更改主表簽核部門
                mainModel.CurrentSignLevelDeptID_FK = upperDeptData.Values.Single();
                sql =
                @"UPDATE signform_main
                SET currentsignleveldeptid_fk = @CurrentSignLevelDeptID_FK, remainder = @Remainder, modifier = @Modifier, modifydate = @ModifyDate WHERE  signdocid = @SignDocID ";
                conditions = new Conditions();
                conditions.Add("@CurrentSignLevelDeptID_FK", mainModel.CurrentSignLevelDeptID_FK);
                conditions.Add("@Remainder", --mainModel.Remainder);
                conditions.Add("@Modifier", mainModel.Modifier);
                conditions.Add("@ModifyDate", mainModel.ModifyDate);
                conditions.Add("@SignDocID", mainModel.SignDocID);
                result.Add(new MultiConditions() { { sql, conditions } });

                //更改子表簽核人員
                detailModel.ChiefID_FK = upperDeptData.Keys.Single();
                detailModel.Status = 2;
                detailModel.Remark = String.Empty;
                sql =
                @"UPDATE signform_detail
                SET chiefid_fk = @ChiefID_FK, status = @Status, remark = @Remark,modifier = @Modifier, modifydate = @ModifyDate WHERE  signdocid_fk = @SignDocID AND chiefid_fk = @OrgSupervisorID ";
                conditions = new Conditions();
                conditions.Add("@ChiefID_FK", detailModel.ChiefID_FK);
                conditions.Add("@Status", detailModel.Status);
                conditions.Add("@Remark", detailModel.Remark);
                conditions.Add("@SignDocID", detailModel.SignDocID_FK);
                conditions.Add("@OrgSupervisorID", currentSignerID);
                conditions.Add("@Modifier", detailModel.Modifier);
                conditions.Add("@ModifyDate", detailModel.ModifyDate);
                result.Add(new MultiConditions() { { sql, conditions } });
            }
            else
            {
                //不符合規則，表示已經到終點

                //指派當前簽核人員
                detailModel.ChiefID_FK = currentSignerID;

                //更改主表狀態 為結案
                sql =
                @"Update SignForm_Main Set FinalStatus = '6', remainder = @Remainder, modifier = @Modifier, modifydate = @ModifyDate Where SignDocID = @SignDocID ";
                mainModel.FinalStatus = 6;
                conditions = new Conditions();
                conditions.Add("@Remainder", --mainModel.Remainder);
                conditions.Add("@SignDocID", mainModel.SignDocID);
                conditions.Add("@Modifier", mainModel.Modifier);
                conditions.Add("@ModifyDate", mainModel.ModifyDate);
                result.Add(new MultiConditions() { { sql, conditions } });

                //更改子表狀態
                sql =
                @"Update SignForm_Detail Set Status = @Status, modifier = @Modifier, modifydate = @ModifyDate Where SignDocID_FK = @SignDocID_FK ";
                conditions = new Conditions();
                detailModel.Status = 6;
                detailModel.Remark = String.Empty;
                conditions.Add("@Status", detailModel.Status);
                conditions.Add("@SignDocID_FK", detailModel.SignDocID_FK);
                conditions.Add("@Modifier", detailModel.Modifier);
                conditions.Add("@ModifyDate", detailModel.ModifyDate);
                result.Add(new MultiConditions() { { sql, conditions } });

                //轉交 AutoInsertHandler

                #region 決定將寫入志元與否

                AutoInsertHandler autoInsert = RepositoryFactory.CreateAutoInsert(mainModel.SignDocID);

                #endregion 決定將寫入志元與否

                var autoInsertDML = autoInsert.GetDML();
                if (autoInsertDML != null)
                {
                    //windows server2003 DTC 設定 交易管理通訊雙方必須設定為 不需驗證
                    result.Add(autoInsert.GetXACTABORTON());
                    result.AddRange(autoInsertDML);
                }
            }

            //log
            conditions = new Conditions()
			{
				{ "@SignDocID_FK", mainModel !=null ? mainModel.SignDocID : (string)null},
				{ "@FormID_FK", mainModel !=null ? mainModel.FormID_FK.ToString() : (string)null},
				{ "@EmployeeID_FK", mainModel !=null ? mainModel.EmployeeID_FK : (string)null},
				{ "@SendDate", mainModel !=null ? mainModel.SendDate.Value.FormatDatetime() :(string)null},
				{ "@CurrentSignLevelDeptID_FK", mainModel !=null ? mainModel.CurrentSignLevelDeptID_FK : (string)null},
				{ "@FinalStatus", mainModel !=null ? mainModel.FinalStatus.ToString() : (string)null},
				{ "@Remainder", mainModel !=null ? mainModel.Remainder.ToString() :(string)null},
				{ "@Creator_Main", mainModel !=null ? mainModel.Creator : (string)null},
				{ "@CreateDate_Main", mainModel !=null && mainModel.CreateDate != DateTime.MinValue ? mainModel.CreateDate.FormatDatetime() : (string)null},
				{ "@Modifier_Main", mainModel !=null ? mainModel.Modifier : (string)null},
				{ "@ModifyDate_Main", mainModel !=null && mainModel.ModifyDate != DateTime.MinValue ? mainModel.ModifyDate.Value.FormatDatetime() :(string)null},
				{ "@DetailSignDocID_FK", detailModel != null ? detailModel.SignDocID_FK : (string)null},
				{ "@ChiefID_FK", detailModel != null ? detailModel.ChiefID_FK : (string)null},
				{ "@Remark", detailModel != null ? detailModel.Remark : (string)null},
				{ "@Status", detailModel != null ? detailModel.Status.ToString() : (string)null},
				{ "@Creator_Detail", detailModel != null ? detailModel.Creator : (string)null},
				{ "@CreateDate_Detail", detailModel != null && detailModel.CreateDate != DateTime.MinValue ? detailModel.CreateDate.FormatDatetime() : (string)null},
				{ "@Modifier_Detail", detailModel != null ? detailModel.Modifier : (string)null},
				{ "@ModifyDate_Detail", detailModel != null && detailModel.ModifyDate != DateTime.MinValue ? detailModel.ModifyDate.Value.FormatDatetime(): (string)null},
				{ "@LogDatetime", DateTime.Now},
			};

            var log = _dc.ConstructInsertDML("SignForm_Log", conditions);
            result.Add(new MultiConditions() { { log, conditions } });

            return result;
        }

        //執行流程判斷後的SQL
        public void ExecuteSQL(List<MultiConditions> sqlDML)
        {
            _dc.ExecuteMultAndCheck(sqlDML);
        }
    }
}