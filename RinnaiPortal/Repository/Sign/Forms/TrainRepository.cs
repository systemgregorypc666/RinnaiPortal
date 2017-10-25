using DBTools;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository.SmartMan;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RinnaiPortal.Repository.Sign.Forms
{
    public class TrainRepository : ISignRepository
    {
        public bool IsConnect { get; set; }
        private DB _dc { get; set; }
        private RootRepository _rootRepo { get; set; }
        private ProcessWorkflowRepository _pwfRepo { get; set; }
        private AutoInsertHandler _handler { get; set; }

        public TrainRepository(DB dc)
        {
            _dc = dc;
            _rootRepo = RepositoryFactory.CreateRootRepo();
            _pwfRepo = RepositoryFactory.CreateProcessWorkflowRepo();
            _handler = RepositoryFactory.CreateAutoInsert();
            IsConnect = _dc.TestConnection();
        }

        public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
        {
            //string strCatalog = RepositoryFactory.PotalConn["Catalog"];
            string strSQL = "";
            switch (slParms.TABLE_ID)
            {
                case "01":
                    strSQL = @"SELECT distinct
		                        class.clid       CLID,
		                        class.clname     CLNAME,
		                        class.start_date STARTDATE,
		                        class.hours      HOURS,
		                        stu.sid          SID,
		                        stu.sname        SNAME,
		                        class.unitid     UNITID,
		                        class.unitname   UNITNAME,
		                        [sign].signdocid SIGNDOCID
                        FROM   rtclass class
	                            JOIN students stu
		                            ON class.clid = stu.clid
	                            LEFT OUTER JOIN (select distinct table_id from FORM_FORMAT)as F on 1=1
							    LEFT OUTER JOIN (select CLID,SID,TABLE_ID,count(*) as A_COUNT from(
							    select CLID, SID, TABLE_ID, QNO, SERIAL_NO, DESCRIPTION ANS, EDIT_DATE, EDIT_PERSON from CHARACTER_ANSWER
							    union
							    select CLID, SID, TABLE_ID, QNO, SERIAL_NO,CONVERT(varchar, ANS) ANS, EDIT_DATE, EDIT_PERSON from NUMERIC_ANSWER) A
							    group by CLID,SID,TABLE_ID) as count_ans on count_ans.CLID=class.CLID and count_ans.SID=stu.SID and count_ans.TABLE_ID=f.TABLE_ID
	                            LEFT OUTER JOIN form_sign [sign]
					                        ON [sign].clid = stu.clid and  [sign].TABLE_ID=F.table_id and stu.sid=[sign].SID
                        WHERE  class.CLID NOT LIKE '9999%' and ((F.TABLE_ID=@TABLE_ID AND [sign].issigned = 'False' ) or (F.TABLE_ID=@TABLE_ID AND [sign].issigned is null )) and ISNULL(count_ans.A_COUNT,0)=0
	                    {0}";
                    break;

                case "02":
                    string strCate = RepositoryFactory.PotalConn["Catelog"];
                    strSQL = String.Format(@"SELECT distinct
		                        class.clid       CLID,
		                        class.clname     CLNAME,
		                        class.start_date STARTDATE,
		                        class.hours      HOURS,
		                        stu.sid          SID,
		                        stu.sname        SNAME,
		                        class.unitid     UNITID,
		                        class.unitname   UNITNAME,
		                        [sign].signdocid SIGNDOCID
                        FROM   rtclass class
	                            JOIN students stu
		                            ON class.clid = stu.clid
	                            LEFT OUTER JOIN (select distinct table_id from FORM_FORMAT)as F on 1=1
								LEFT OUTER JOIN (select CLID,SID,TABLE_ID,count(*) as A_COUNT from(
							    select CLID, SID, TABLE_ID, QNO, SERIAL_NO, DESCRIPTION ANS, EDIT_DATE, EDIT_PERSON from CHARACTER_ANSWER
							    union
							    select CLID, SID, TABLE_ID, QNO, SERIAL_NO,CONVERT(varchar, ANS) ANS, EDIT_DATE, EDIT_PERSON from NUMERIC_ANSWER) A
							    group by CLID,SID,TABLE_ID) as count_ans on count_ans.CLID=class.CLID and count_ans.SID=stu.SID and count_ans.TABLE_ID=f.TABLE_ID
	                            LEFT OUTER JOIN form_sign [sign]
					                        ON [sign].clid = stu.clid and  [sign].TABLE_ID=F.table_id and stu.sid=[sign].SID
                                left OUTER join {0}..SignForm_Main Sign_M ON Sign_M.SignDocID=[sign].SignDocID
                        WHERE  class.CLID NOT LIKE '9999%' and ((F.TABLE_ID=@TABLE_ID AND [sign].issigned = 'False' ) or (F.TABLE_ID=@TABLE_ID AND [sign].issigned is null )) and ISNULL(Sign_M.FinalStatus,0) not in (2,3,4,6) and ISNULL(count_ans.A_COUNT,0)=0", strCate);
                    strSQL = strSQL + "{0}";
                    break;

                case "03":
                    string strCatelog = RepositoryFactory.PotalConn["Catelog"];
                    strSQL = String.Format(@"SELECT distinct
		                                    class.clid       CLID,
		                                    class.clname     CLNAME,
		                                    class.start_date STARTDATE,
		                                    class.hours      HOURS,
		                                    stu.sid          SID,
		                                    stu.sname        SNAME,
		                                    class.unitid     UNITID,
		                                    class.unitname   UNITNAME,
		                                    [sign].signdocid SIGNDOCID
                                    FROM   rtclass class
	                                        JOIN students stu
		                                        ON class.clid = stu.clid
		                                    JOIN {0}..Employee Emp
		                                        ON stu.sid = Emp.EmployeeID
		                                    JOIN {0}..Department Dept
			                                    ON Emp.DepartmentID_FK = Dept.DepartmentID
	                                        LEFT OUTER JOIN (select distinct table_id from FORM_FORMAT)as F on 1=1
                                            LEFT OUTER JOIN (select CLID,SID,TABLE_ID,count(*) as A_COUNT from(
							                select CLID, SID, TABLE_ID, QNO, SERIAL_NO, DESCRIPTION ANS, EDIT_DATE, EDIT_PERSON from CHARACTER_ANSWER
							                union
							                select CLID, SID, TABLE_ID, QNO, SERIAL_NO,CONVERT(varchar, ANS) ANS, EDIT_DATE, EDIT_PERSON from NUMERIC_ANSWER) A
							                group by CLID,SID,TABLE_ID) as count_ans on count_ans.CLID=class.CLID and count_ans.SID=stu.SID and count_ans.TABLE_ID=f.TABLE_ID
	                                        LEFT OUTER JOIN form_sign [sign]
					                                    ON [sign].clid = stu.clid and  [sign].TABLE_ID=F.table_id and stu.sid=[sign].SID
                                    WHERE  class.CLID NOT LIKE '9999%' and ((F.TABLE_ID=@TABLE_ID AND [sign].issigned = 'False' ) or (F.TABLE_ID=@TABLE_ID AND [sign].issigned is null )) and ISNULL(count_ans.A_COUNT,0)=0", strCatelog);
                    strSQL = strSQL + "{0}";
                    break;

                default:
                    throw new Exception("無此問卷代號!");
            }

            #region 備份SQL

            //            if (slParms.TABLE_ID=="02")
            //            {
            //                strSQL = @"SELECT distinct
            //		                class.clid       CLID,
            //		                class.clname     CLNAME,
            //		                class.start_date STARTDATE,
            //		                class.hours      HOURS,
            //		                stu.sid          SID,
            //		                stu.sname        SNAME,
            //		                class.unitid     UNITID,
            //		                class.unitname   UNITNAME,
            //		                [sign].signdocid SIGNDOCID
            //                FROM   rtclass class
            //	                    JOIN students stu
            //		                    ON class.clid = stu.clid
            //	                    LEFT OUTER JOIN (select distinct table_id from FORM_FORMAT)as F on 1=1
            //	                    LEFT OUTER JOIN form_sign [sign]
            //					                ON [sign].clid = stu.clid and  [sign].TABLE_ID=F.table_id and stu.sid=[sign].SID
            //                        left OUTER join RinnaiPortal..SignForm_Main Sign_M ON Sign_M.SignDocID=[sign].SignDocID
            //                WHERE  class.CLID NOT LIKE '9999%' and ((F.TABLE_ID=@TABLE_ID AND [sign].issigned = 'False' ) or (F.TABLE_ID=@TABLE_ID AND [sign].issigned is null )) and ISNULL(Sign_M.FinalStatus,0) not in (2,3)
            //	                {0}";
            //            }
            //            else if (slParms.TABLE_ID == "03")
            //            {
            //                string strCatelog = RepositoryFactory.PotalConn["Catelog"];
            //                strSQL = String.Format(@"SELECT distinct
            //		                                class.clid       CLID,
            //		                                class.clname     CLNAME,
            //		                                class.start_date STARTDATE,
            //		                                class.hours      HOURS,
            //		                                stu.sid          SID,
            //		                                stu.sname        SNAME,
            //		                                class.unitid     UNITID,
            //		                                class.unitname   UNITNAME,
            //		                                [sign].signdocid SIGNDOCID
            //                                FROM   rtclass class
            //	                                    JOIN students stu
            //		                                    ON class.clid = stu.clid
            //		                                JOIN {0}..Employee Emp
            //		                                    ON stu.sid = Emp.EmployeeID
            //		                                JOIN {0}..Department Dept
            //			                                ON Emp.DepartmentID_FK = Dept.DepartmentID
            //	                                    LEFT OUTER JOIN (select distinct table_id from FORM_FORMAT)as F on 1=1
            //	                                    LEFT OUTER JOIN form_sign [sign]
            //					                                ON [sign].clid = stu.clid and  [sign].TABLE_ID=F.table_id and stu.sid=[sign].SID
            //                                WHERE  class.CLID NOT LIKE '9999%' and ((F.TABLE_ID=@TABLE_ID AND [sign].issigned = 'False' ) or (F.TABLE_ID=@TABLE_ID AND [sign].issigned is null ))", strCatelog);
            //                strSQL = strSQL + "{0}";
            //            }
            //            else
            //            {
            //                strSQL = @"SELECT distinct
            //		                class.clid       CLID,
            //		                class.clname     CLNAME,
            //		                class.start_date STARTDATE,
            //		                class.hours      HOURS,
            //		                stu.sid          SID,
            //		                stu.sname        SNAME,
            //		                class.unitid     UNITID,
            //		                class.unitname   UNITNAME,
            //		                [sign].signdocid SIGNDOCID
            //                FROM   rtclass class
            //	                    JOIN students stu
            //		                    ON class.clid = stu.clid
            //	                    LEFT OUTER JOIN (select distinct table_id from FORM_FORMAT)as F on 1=1
            //	                    LEFT OUTER JOIN form_sign [sign]
            //					                ON [sign].clid = stu.clid and  [sign].TABLE_ID=F.table_id and stu.sid=[sign].SID
            //                WHERE  class.CLID NOT LIKE '9999%' and ((F.TABLE_ID=@TABLE_ID AND [sign].issigned = 'False' ) or (F.TABLE_ID=@TABLE_ID AND [sign].issigned is null ))
            //	                {0}";
            //            }

            #endregion 備份SQL

            string strConditions = string.Empty;
            Conditions conditionsDic = new Conditions()
            {
                //{ "@SID", slParms.Member.EmployeeID },
                { "@TABLE_ID", slParms.TABLE_ID }
            };
            //組成查詢條件 SQL

            if (slParms.TABLE_ID == "03")
            {
                if (!String.IsNullOrWhiteSpace(slParms.QueryText))
                {
                    conditionsDic.Add("@queryText", String.Format("{0}", slParms.QueryText.Trim()));
                    strConditions = String.Format("AND (class.CLID=@queryText OR stu.sid = @queryText)");

                    //conditionsDic.Add("@ChiefID", String.Format("{0}", slParms.Member.EmployeeID.Trim()));
                    //strConditions += String.Format("AND Dept.ChiefID_FK = @ChiefID");
                }
                //else
                //{
                conditionsDic.Add("@ChiefID", String.Format("{0}", slParms.Member.EmployeeID.Trim()));
                strConditions += String.Format("AND Dept.ChiefID_FK = @ChiefID");
                //}
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(slParms.QueryText))
                {
                    conditionsDic.Add("@queryText", String.Format("{0}", slParms.QueryText.Trim()));
                    strConditions = String.Format("AND (class.CLID=@queryText OR stu.sid = @queryText)");
                }
                else
                {
                    conditionsDic.Add("@SID", String.Format("{0}", slParms.Member.EmployeeID.Trim()));
                    strConditions = String.Format("AND stu.sid = @SID");
                }
            }
            //#0001 2017-07-19 by 俊晨 開課日期 <  2017/7/1的資料因不簽核故不顯示(經理指示)。
            //#0019 受訓心得報告不顯示B10，C01，A09 的課程代碼
            //#0023 主管成效評核追縱維護新增時不顯示B10 ，C01，A09 的課程代碼 by 淑娟 原slParms.TABLE_ID == "02"增加slParms.TABLE_ID == "03"
            strSQL += " and class.START_DATE >= '2017-07-01' ";
            if (slParms.TABLE_ID == "02" || slParms.TABLE_ID == "03")
                strSQL += " and class.CTID not in ('B10','C01','A09') ";

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
				"CLID", "CLNAME","STARTDATE", "HOURS", "SID", "SNAME", "UNITID", "UNITNAME", "SIGNDOCID",
			};

            Pagination resultPagination = _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);

            #region #0010 增加判斷評核表不該出現受訓心得未填寫之學員

            //增加判斷未填寫受訓心得不該出現在評核列表上
            if (slParms.TABLE_ID == "03")
            {
                Dictionary<string, string> delDataIdentify = new Dictionary<string, string>();
                int beforePageSize = paginationParms.PageSize;
                paginationParms.PageIndex = 1;
                paginationParms.PageSize = 99999;
                Pagination getResult = _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);

                if (resultPagination.Data != null)
                {
                    var filterData = getResult.Data.Rows.Cast<DataRow>().ToList();
                    string queryHasFeedbackSQL = @"
                                        SELECT distinct
                                        class.clid       CLID,
                                        class.clname     CLNAME,
                                        class.start_date STARTDATE,
                                        class.hours      HOURS,
                                        stu.sid          SID,
                                        stu.sname        SNAME,
                                        class.unitid     UNITID,
                                        class.unitname   UNITNAME,
                                        [sign].signdocid SIGNDOCID
                                        FROM   rtclass class
                                        JOIN students stu
                                        ON class.clid = stu.clid
                                        LEFT OUTER JOIN (select distinct table_id from FORM_FORMAT)as F on 1=1
                                        LEFT OUTER JOIN (select CLID,SID,TABLE_ID,count(*) as A_COUNT from(
                                        select CLID, SID, TABLE_ID, QNO, SERIAL_NO, DESCRIPTION ANS, EDIT_DATE, EDIT_PERSON from CHARACTER_ANSWER
                                        union
                                        select CLID, SID, TABLE_ID, QNO, SERIAL_NO,CONVERT(varchar, ANS) ANS, EDIT_DATE, EDIT_PERSON from NUMERIC_ANSWER) A
                                        group by CLID,SID,TABLE_ID) as count_ans on count_ans.CLID=class.CLID and count_ans.SID=stu.SID and count_ans.TABLE_ID=f.TABLE_ID
                                        LEFT OUTER JOIN form_sign [sign]
                                        ON [sign].clid = stu.clid and  [sign].TABLE_ID=F.table_id and stu.sid=[sign].SID
                                        left OUTER join RinnaiPortal_Formal..SignForm_Main Sign_M ON Sign_M.SignDocID=[sign].SignDocID
                                        WHERE  class.CLID NOT LIKE '9999%' and ((F.TABLE_ID=@TABLE_ID AND [sign].issigned = 'False' ) or (F.TABLE_ID=@TABLE_ID AND [sign].issigned is null )) and ISNULL(Sign_M.FinalStatus,0) not in (2,3,4,6) and ISNULL(count_ans.A_COUNT,0)=0AND stu.sid = @SID and class.START_DATE >= '2017-07-01'
                                        AND class.CLID = @CLID
                                            ";
                    foreach (DataRow data in filterData)
                    {
                        string studentID = data["SID"].ToString();
                        string classID = data["CLID"].ToString();

                        Conditions conditionsDicForQueryHasAns = new Conditions();
                        conditionsDicForQueryHasAns.Add("@TABLE_ID", "02");
                        conditionsDicForQueryHasAns.Add("@SID", String.Format("{0}", studentID));
                        conditionsDicForQueryHasAns.Add("@CLID", String.Format("{0}", classID));

                        //var paginationParmsForQueryHasAns = new PaginationParms()
                        //{
                        //    QueryString = StudyOpinionFeedbackSQL,
                        //    QueryConditions = conditionsDicForQueryHasAns,
                        //    PageIndex = pParms.PageIndex,
                        //    PageSize = pParms.PageSize
                        //};

                        DataRow result = _dc.QueryForDataRow(queryHasFeedbackSQL, conditionsDicForQueryHasAns);
                        //!= null為受訓心得尚未填寫 不可呈現於列表
                        if (result != null)
                            delDataIdentify[classID] = studentID;
                    }

                    //刪除未填寫受訓心得的條件
                    foreach (var di in delDataIdentify)
                    {
                        try
                        {
                            var query = getResult.Data.AsEnumerable().Where(r => r.Field<string>("CLID") == di.Key && r.Field<string>("SID") == di.Value);
                            foreach (var row in query.ToList())
                                getResult.Data.Rows.Remove(row);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    if (getResult.Data.Rows.Count > 0)
                    {
                        int startRow = (pParms.PageIndex - 1) * beforePageSize;
                        Pagination pagination = new Pagination(getResult.Data, getResult.Data.Rows.Count, pParms.PageIndex);
                        pagination.Data = pagination.Data.Rows.Cast<DataRow>().Skip(startRow).Take(beforePageSize).CopyToDataTable();
                        resultPagination = pagination;
                    }
                    else
                        resultPagination.Data = null;
                }
            }

            #endregion #0010 增加判斷評核表不該出現受訓心得未填寫之學員

            //根據 SQL取得　Pagination
            return resultPagination;
        }

        public void SubmitData(TrainViewModel model)
        {
            //Create
            if (String.IsNullOrWhiteSpace(model.SignDocID_FK))
            {
                createData(model);
            }
            else//Edit
            {
            }
        }

        private void createData(RinnaiForms trainForm)
        {
            var model = trainForm as TrainViewModel;
            if (model == null) { throw new Exception("新增錯誤!"); }

            model.SignDocID_FK = GetSeqIDUtils.GetSignDocID(model.Creator, "TR");
            if (String.IsNullOrWhiteSpace(model.SignDocID_FK))
            {
                throw new Exception("系統忙碌中請稍候再試!");
            }

            var status = "2";
            var createDateTime = DateTime.Now;
            var procedureData = _rootRepo.QueryForSignProcedureByRuleID(model.RuleID_FK);
            var remainder = procedureData != null ? Int32.Parse(procedureData["SignLevel"].ToString()) : -1;
            //判斷 FlowRule
            if (!_pwfRepo.IsFollowFlowRule(model.RuleID_FK, model.DepartmentID_FK))
            {
                throw new Exception("不符合簽核規則，無法新增!");
            }

            // 取得簽核人員部門資料
            var deptData = _rootRepo.QueryForDepartmentByDeptID(model.DepartmentID_FK);
            if (deptData == null)
            {
                throw new Exception("查無簽核人員所屬部門資料!");
            }

            // Cross DB，置換 _dc 為 Portal
            _dc = ConnectionFactory.GetPortalDC();

            var upperDeptData = _pwfRepo.FindUpperDeptData(model.DepartmentID_FK, model.ApplyID_FK);
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

            // Use Distributed Transaction Coordinator Service
            // Need Set XACT_ABORT ON
            manipulationConditions.Add(_handler.GetXACTABORTON());

            // 回寫 Trainning System
            // 需要設定 link server
            dic = new Conditions()
			{
				{"@SIGNDOCID", model.SignDocID_FK},
				{"@CLID", model.CLID},
				{"@SID", model.ApplyID_FK},
			};
            strSQL = String.Format(
            @"UPDATE {0}.{1}.dbo.form_records
			SET    signdocid = @SIGNDOCID, IsSign = 'True'
			WHERE  clid = @CLID
				   AND table_id = '02'
				   AND sid = @SID ", RepositoryFactory.TrainConn["DataSource"], RepositoryFactory.TrainConn["Catalog"]);
            //@"Select * from soyal.training.dbo.form_records";
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            var result = _dc.ExecuteMultAndCheck(manipulationConditions);
            if (!result) { throw new Exception("發生DML異動0筆資料的異常!"); }
        }
    }
}