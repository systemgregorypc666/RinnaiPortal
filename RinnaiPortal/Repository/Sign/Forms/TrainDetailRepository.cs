using DBTools;
using RinnaiPortal.Enums;
using RinnaiPortal.Extensions;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.ViewModel.Sign.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RinnaiPortal.Repository.Sign.Forms
{
    public class TrainDetailRepository : ISignRepository
    {
        private DataTable temp = new DataTable();
        private RootRepository _rootRepo { get; set; }
        private ProcessWorkflowRepository _pwfRepo { get; set; }
        private DB _dc { get; set; }

        public TrainDetailRepository(DB dc, RootRepository rootRepo, ProcessWorkflowRepository pwfRepo)
        {
            _dc = dc;
            _rootRepo = rootRepo;
            _pwfRepo = pwfRepo;
            var result = _dc.TestConnection();
        }

        //列表
        public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
        {
            string strSQL =
@"SELECT
		UU.qno qno,
		UU.codename codename,
		C.description description
FROM   (SELECT R.clid,
			   FS.sid,
			   R.table_id,
			   FS.signdocid,
			   F.serial_no,
			   F.qno,
			   Q.anstype,
			   Q.codename
		FROM   form_records R
			   JOIN form_format F
				 ON ( R.table_id = F.table_id )
					AND ( R.edition_id = F.edition_id )
			   JOIN questions Q
				 ON F.qno = Q.qno AND Q.qno <> 44
			   JOIN form_sign FS
				 ON ( R.table_id = FS.table_id )
		WHERE  R.table_id = '02') UU
	   JOIN character_answer C
		 ON UU.clid = C.clid
			AND UU.sid = C.sid
			AND UU.qno = C.qno
WHERE  1 = 1 {0}";

            var paginationParms = new PaginationParms()
            {
                QueryString = strSQL,
                PageIndex = pParms.PageIndex,
                PageSize = pParms.PageSize
            };
            if (!String.IsNullOrWhiteSpace(slParms.EmployeeID_FK))
            {
                paginationParms.QueryConditions.Add("@SID", slParms.EmployeeID_FK);
                paginationParms.QueryString = String.Format(paginationParms.QueryString, "AND C.sid = @SID {0}");
            }
            if (!String.IsNullOrWhiteSpace(slParms.CLID))
            {
                paginationParms.QueryConditions.Add("@CLID", slParms.CLID);
                paginationParms.QueryString = String.Format(paginationParms.QueryString, "AND UU.clid = @CLID {0}");
            }
            if (!String.IsNullOrWhiteSpace(slParms.SignDocID))
            {
                paginationParms.QueryConditions.Add("@SignDocID", slParms.SignDocID);
                paginationParms.QueryString = String.Format(paginationParms.QueryString, "AND UU.SignDocID = @SignDocID");
            }

            paginationParms.QueryString = String.Format(paginationParms.QueryString, "");
            pParms.OrderField = "qno".Equals(pParms.OrderField) ? "qno" : pParms.OrderField;
            string orderExpression = String.Format("{0}{1}", pParms.Descending ? "-" : "", pParms.OrderField);

            var allowColumns = new List<string> { "qno", "codename", "description", };

            //根據 SQL取得　Pagination
            return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
        }

        //簽核作業頁面 > 取得學員課程明細資料
        public TrainViewModel GetData(string SignDocID)
        {
            TrainViewModel model = null;
            string strCatelog = RepositoryFactory.PotalConn["Catelog"];
            string strSQL = String.Format(@"select
	                            a.CLID CLID,
	                            class.CLNAME CLNAME,
	                            class.HOURS,
	                            class.START_DATE STARTDATE,
	                            a.SID SID,
	                            emp.EmployeeName SNAME,
	                            a.SignDocID SignDocID,
	                            dep.DepartmentID DepartmentID_FK,
	                            dep.DepartmentName DepartmentName,
	                            a.TABLE_ID TABLE_ID,
	                            Sig_M.FormID_FK FormID_FK,
	                            Sig_M.EmployeeID_FK ApplyID_FK,
	                            apply.EmployeeName ApplayName,
	                            Sig_M.SendDate ApplyDateTime
                            from FORM_SIGN a
                            left join {0}..SignForm_Main Sig_M on Sig_M.SignDocID=a.SignDocID
                            left join {0}..Employee apply on apply.EmployeeID=Sig_M.EmployeeID_FK
                            left join {0}..Employee emp on emp.EmployeeID=a.SID
                            left join {0}..Department dep on dep.DepartmentID=emp.DepartmentID_FK
                            left join RTCLASS class on class.CLID=a.CLID
                            where a.SignDocID=@SignDocID", strCatelog);

            var strCondition = new Conditions() { { "@SignDocID", SignDocID } };

            var result = _dc.QueryForDataRow(strSQL, strCondition);
            if (result == null) { return null; }

            model = new TrainViewModel()
            {
                FormID_FK = Int32.Parse(result["FormID_FK"].ToString()),
                SignDocID_FK = result["SignDocID"].ToString(),
                ApplyID_FK = result["ApplyID_FK"].ToString(),
                ApplyName = result["ApplayName"].ToString(),
                ApplyDateTime = DateTime.Parse(result["ApplyDateTime"].ToString()),
                CLID = result["CLID"].ToString(),
                SID = result["SID"].ToString(),
                CLNAME = result["CLNAME"].ToString(),
                Start_Date = !result["STARTDATE"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["STARTDATE"].ToString()) : (DateTime?)null,
                HOURS = result["HOURS"].ToString(),
                SNAME = result["SNAME"].ToString(),
                DepartmentID_FK = result["DepartmentID_FK"].ToString()
            };

            return model;
        }

        public TrainViewModel GetDetail(string CLID, string SID, string Dept)
        {
            TrainViewModel model = null;
            string strSQL = @"SELECT
	                                   class.clid       CLID,
                                       class.clname     CLNAME,
                                       class.start_date STARTDATE,
                                       class.hours      HOURS,
                                       stu.sid          SID,
                                       stu.sname        SNAME,
                                       class.unitid     UNITID,
                                       class.unitname   UNITNAME
                                FROM   rtclass class
                                       RIGHT JOIN students stu  ON class.clid = stu.clid
                                WHERE  stu.SID = @SID and class.clid=@CLID ";

            var strCondition = new Conditions() { { "@CLID", CLID }, { "@SID", SID } };

            var result = _dc.QueryForDataRow(strSQL, strCondition);
            if (result == null) { return null; }

            model = new TrainViewModel()
            {
                CLID = result["CLID"].ToString(),
                SID = result["SID"].ToString(),
                CLNAME = result["CLNAME"].ToString(),
                Start_Date = !result["STARTDATE"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["STARTDATE"].ToString()) : (DateTime?)null,
                HOURS = result["HOURS"].ToString(),
                SNAME = result["SNAME"].ToString(),
                DepartmentID_FK = Dept
            };

            return model;
        }

        /// <summary>
        /// 根據Table查詢對應的所有題目
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public List<DataRow> QueryQuestionDataByTableID(string tableID)
        {
            //題庫dbo.QUESTIONS ANSTYPE NULL	NVARCHAR	5	答案型態	N:數字/C:文字
            var conditions = new Conditions() { { "@TableID", tableID } };
            string sql = @"select *
                            from FORM_FORMAT a
                            left join QUESTIONS b on a.QNO=b.QNO
                            where table_id=@TableID and CHIEF<>'Y' and EDITION_ID=(select max(EDITION_ID) from FORM_FORMAT where table_id=@TableID)
                            order by serial_no";

            var result = _dc.QueryForDataRows(sql, conditions);
            return result ?? null;
        }

        //查詢題目 根據 TableID
        public List<DataRow> QueryQuestionDataByChief(string TableID)
        {
            var conditions = new Conditions() { { "@TableID", TableID } };
            string sql = @"select *
                            from FORM_FORMAT a
                            left join QUESTIONS b on a.QNO=b.QNO
                            where table_id=@TableID and CHIEF='Y' and EDITION_ID=(select max(EDITION_ID) from FORM_FORMAT where table_id=@TableID)
                            order by serial_no";

            var result = _dc.QueryForDataRows(sql, conditions);
            return result ?? null;
        }


        /// <summary>
        /// 查詢學員答案 根據 SignDocID
        /// </summary>
        /// <param name="SignDocID"></param>
        /// <returns></returns>
        public List<DataRow> QueryQuestionDataBySignDocID(string SignDocID)
        {
            var conditions = new Conditions() { { "@SignDocID", SignDocID } };

            #region #0014 現場主管林美員欲簽核許逸庭受訓心得報告時發生一起載入其他學員之報告 sql未mapping 學員ID(SID)

            //Sql未放where a.table_id = '02'  因為只有02需要簽核 而join FORM_SIGN這個table需要有簽核代碼，已就是只會select 有簽核代碼的資料 by 俊晨 20170816
            string sql = @"
                        select * from (
                        select a.SignDocID,a.CLID,a.SID,a.TABLE_ID,CANS.QNO,CANS.SERIAL_NO,CANS.DESCRIPTION ANS,QUE.CODENAME,QUE.ANSTYPE,QUE.CHIEF
                        from FORM_SIGN a
                        left join CHARACTER_ANSWER CANS on CANS.CLID=a.CLID and CANS.TABLE_ID=a.TABLE_ID  and CANS.SID = a.SID
                        left join QUESTIONS QUE on QUE.QNO=CANS.QNO

                        union
                        select a.SignDocID,a.CLID,a.SID,a.TABLE_ID,NANS.QNO,NANS.SERIAL_NO,CONVERT(varchar, NANS.ANS) ANS,QUE.CODENAME,QUE.ANSTYPE,QUE.CHIEF
                        from FORM_SIGN a
                        left join NUMERIC_ANSWER NANS on NANS.CLID=a.CLID and NANS.TABLE_ID=a.TABLE_ID and NANS.SID = a.SID
                        left join QUESTIONS QUE on QUE.QNO=NANS.QNO
                        )data
                        where SignDocID=@SignDocID";

            #endregion #0014 現場主管林美員欲簽核許逸庭受訓心得報告時發生一起載入其他學員之報告 sql未mapping 學員ID(SID)

            //            string sql = @"select * from (
            //                            select a.SignDocID,a.CLID,a.SID,a.TABLE_ID,CANS.QNO,CANS.SERIAL_NO,CANS.DESCRIPTION ANS,QUE.CODENAME,QUE.ANSTYPE,QUE.CHIEF
            //                            from FORM_SIGN a
            //                            left join CHARACTER_ANSWER CANS on CANS.CLID=a.CLID and CANS.TABLE_ID=a.TABLE_ID
            //                            left join QUESTIONS QUE on QUE.QNO=CANS.QNO
            //                            union
            //                            select a.SignDocID,a.CLID,a.SID,a.TABLE_ID,NANS.QNO,NANS.SERIAL_NO,CONVERT(varchar, NANS.ANS) ANS,QUE.CODENAME,QUE.ANSTYPE,QUE.CHIEF
            //                            from FORM_SIGN a
            //                            left join NUMERIC_ANSWER NANS on NANS.CLID=a.CLID and NANS.TABLE_ID=a.TABLE_ID
            //                            left join QUESTIONS QUE on QUE.QNO=NANS.QNO
            //                            )data
            //                            where SignDocID=@SignDocID";

            var result = _dc.QueryForDataRows(sql, conditions);
            return result ?? null;
        }

        /// <summary>
        /// 查詢主管意見 根據 SignDocID
        /// </summary>
        /// <param name="SignDocID"></param>
        /// <returns></returns>
        public List<DataRow> QueryChiefDataBySignDocID(string SignDocID)
        {
            var conditions = new Conditions() { { "@SignDocID", SignDocID } };

            #region #0014 現場主管林美員欲簽核許逸庭受訓心得報告時發生一起載入其他學員之報告 sql 未mapping sid (and d.SID = c.SID)

            string sql = @"
                select c.SignDocID,c.CLID,c.SID,a.TABLE_ID,a.SERIAL_NO,a.TABLE_NAME,b.ANSTYPE,b.QNO,b.CODECD,b.CODENAME,b.CHIEF,d.ANS
                from FORM_FORMAT a
                left join QUESTIONS b on a.QNO=b.QNO
                left join FORM_SIGN c on c.TABLE_ID=a.TABLE_ID

                left join (
                select CANS.TABLE_ID,CANS.CLID,CANS.SID,QNO,CANS.SERIAL_NO,CANS.DESCRIPTION ANS
                from CHARACTER_ANSWER CANS
                union
                select NANS.TABLE_ID,NANS.CLID,NANS.SID,NANS.QNO,NANS.SERIAL_NO,CONVERT(varchar, NANS.ANS) ANS
                from NUMERIC_ANSWER NANS
                ) d on a.QNO=d.QNO   and c.TABLE_ID=d.TABLE_ID and c.CLID=d.CLID
                and d.SID = c.SID

                where SignDocID=@SignDocID and CHIEF='Y'
                ";

            #endregion #0014 現場主管林美員欲簽核許逸庭受訓心得報告時發生一起載入其他學員之報告 sql 未mapping sid (and d.SID = c.SID)

            //            string sql = @"select c.SignDocID,c.CLID,c.SID,a.TABLE_ID,a.SERIAL_NO,a.TABLE_NAME,b.ANSTYPE,b.QNO,b.CODECD,b.CODENAME,b.CHIEF,d.ANS from FORM_FORMAT a
            //                            left join QUESTIONS b on a.QNO=b.QNO
            //                            left join FORM_SIGN c on c.TABLE_ID=a.TABLE_ID
            //                            left join (select CANS.TABLE_ID,CANS.CLID,CANS.SID,QNO,CANS.SERIAL_NO,CANS.DESCRIPTION ANS
            //                            from CHARACTER_ANSWER CANS
            //                            union
            //                            select NANS.TABLE_ID,NANS.CLID,NANS.SID,NANS.QNO,NANS.SERIAL_NO,CONVERT(varchar, NANS.ANS) ANS
            //                            from NUMERIC_ANSWER NANS) d on a.QNO=d.QNO and c.TABLE_ID=d.TABLE_ID and c.CLID=d.CLID
            //                            where c.SignDocID=@SignDocID and b.CHIEF='Y'";

            //            string sql = @"select * from (
            //                            select a.SignDocID,a.CLID,a.SID,a.TABLE_ID,CANS.QNO,CANS.SERIAL_NO,CANS.DESCRIPTION ANS,QUE.CODENAME,QUE.ANSTYPE,QUE.CHIEF
            //                            from FORM_SIGN a
            //                            left join CHARACTER_ANSWER CANS on CANS.CLID=a.CLID and CANS.TABLE_ID=a.TABLE_ID
            //                            left join QUESTIONS QUE on QUE.QNO=CANS.QNO
            //                            union
            //                            select a.SignDocID,a.CLID,a.SID,a.TABLE_ID,NANS.QNO,NANS.SERIAL_NO,CONVERT(varchar, NANS.ANS) ANS,QUE.CODENAME,QUE.ANSTYPE,QUE.CHIEF
            //                            from FORM_SIGN a
            //                            left join NUMERIC_ANSWER NANS on NANS.CLID=a.CLID and NANS.TABLE_ID=a.TABLE_ID
            //                            left join QUESTIONS QUE on QUE.QNO=NANS.QNO
            //                            )data
            //                            where SignDocID=@SignDocID and CHIEF='Y'";

            var result = _dc.QueryForDataRows(sql, conditions);
            return result ?? null;
        }

        /// <summary>
        /// 詢題目最新版次 根據 TableID
        /// </summary>
        /// <param name="TableID"></param>
        /// <returns></returns>
        public string Find_EDITIONID(string TableID)
        {
            string File_Photo = string.Empty;
            string strSQL = @"select max(EDITION_ID) as EDITION_ID from FORM_FORMAT where table_id='" + TableID + "'";
            temp = DBClass.Create_Table_T(strSQL, "GET");
            if (temp.Rows.Count > 0)
            {
                File_Photo = temp.Rows[0]["EDITION_ID"].ToString();
            }
            return File_Photo;
        }

        /// <summary>
        /// 查詢是否已開空白簽核記錄檔
        /// </summary>
        /// <param name="CLID"></param>
        /// <param name="TableID"></param>
        /// <param name="EDITIONID"></param>
        /// <returns></returns>
        public bool CheckHasSignFormRecords(string CLID, string TableID, string EDITIONID)
        {
            bool hasEmptySignFrom = false;
            string File_Photo = string.Empty;
            string strSQL = @"SELECT COUNT(*) as RECORDS_COUNT FROM FORM_RECORDS WHERE CLID='" + CLID + "' and  TABLE_ID='" + TableID + "' AND EDITION_ID='" + EDITIONID + "'";
            temp = DBClass.Create_Table_T(strSQL, "GET");
            if (temp.Rows.Count > 0)
            {
                File_Photo = temp.Rows[0]["RECORDS_COUNT"].ToString();
                if (Convert.ToInt32(File_Photo) > 0)
                    hasEmptySignFrom = true;
            }
            else
                hasEmptySignFrom = false;
            return hasEmptySignFrom;
        }

        /// <summary>
        /// 根據傳來的Table名稱刪除指定的資料
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="CLID"></param>
        /// <param name="SID"></param>
        /// <param name="TABLE_ID"></param>
        public void FromTableNameDeleteValue(string tablename, string CLID, string SID, string TABLE_ID)
        {
            var mainpulationConditions = new Conditions()
		    {
			    {"@CLID", CLID },
                {"@SID", SID },
                {"@TABLE_ID", TABLE_ID }
		    };

            try
            {
                _dc.ExecuteAndCheck(@"delete from " + tablename + " where CLID=@CLID and SID=@SID and  TABLE_ID=@TABLE_ID;  ", mainpulationConditions);
            }
            catch (Exception ex)
            {
                throw new Exception("刪除失敗!" + ex.Message);
            }
        }

        //刪除主管答案
        public void DelChiefAnswer(string tablename, string CLID, string SID, string TABLE_ID)
        {
            var mainpulationConditions = new Conditions()
		    {
			    {"@CLID", CLID },
                {"@SID", SID },
                {"@TABLE_ID", TABLE_ID }
		    };

            try
            {
                string strSQL = @"delete from " + tablename + " from " + tablename + " A,QUESTIONS QUE where QUE.QNO=A.QNO and QUE.CHIEF='Y' and CLID=@CLID and SID=@SID and  TABLE_ID=@TABLE_ID";
                _dc.ExecuteAndCheck(strSQL, mainpulationConditions);
            }
            catch (Exception ex)
            {
                throw new Exception("[刪除失敗] system exception" + ex.Message);
            }
        }

        //新增答案(數字類型)
        public void AddAnswer_N(string CLID, string SID, string TABLE_ID, string QNO, string SERIAL_NO, string ANS, string EDIT_PERSON)
        {
            var mainpulationConditions = new Conditions()
		    {
			    {"@CLID", CLID },
                {"@SID", SID },
                {"@TABLE_ID", TABLE_ID },
                {"@QNO", Int32.Parse(QNO) },
                {"@SERIAL_NO", Int32.Parse(SERIAL_NO) },
                {"@ANS", float.Parse(ANS) },
                //{"@EDIT_DATE", DateTime.Now },
                {"@EDIT_PERSON", EDIT_PERSON }
		    };

            try
            {
                string strSQL = @"INSERT into NUMERIC_ANSWER (CLID,SID,TABLE_ID,QNO,SERIAL_NO,ANS,EDIT_DATE,EDIT_PERSON)
                                    VALUES (@CLID,@SID,@TABLE_ID,@QNO,@SERIAL_NO,@ANS,getdate(),@EDIT_PERSON) ";
                _dc.ExecuteAndCheck(strSQL, mainpulationConditions);
            }
            catch (Exception ex)
            {
                throw new Exception("[新增失敗] system exception" + ex.Message);
            }
        }

        /// <summary>
        /// 新增答案 文字類型
        /// </summary>
        /// <param name="CLID"></param>
        /// <param name="SID"></param>
        /// <param name="TABLE_ID"></param>
        /// <param name="QNO"></param>
        /// <param name="SERIAL_NO"></param>
        /// <param name="DESCRIPTION"></param>
        /// <param name="EDIT_PERSON"></param>
        public void AddAnswer_C(string CLID, string SID, string TABLE_ID, string QNO, string SERIAL_NO, string DESCRIPTION, string EDIT_PERSON)
        {
            var mainpulationConditions = new Conditions()
		    {
			    {"@CLID", CLID },
                {"@SID", SID },
                {"@TABLE_ID", TABLE_ID },
                {"@QNO", Int32.Parse(QNO) },
                {"@SERIAL_NO", Int32.Parse(SERIAL_NO) },
                {"@DESCRIPTION", DESCRIPTION },
                //{"@EDIT_DATE", DateTime.Now },
                {"@EDIT_PERSON", EDIT_PERSON }
		    };

            try
            {
                _dc.ExecuteAndCheck(
                                    @"INSERT into CHARACTER_ANSWER (CLID,SID,TABLE_ID,QNO,SERIAL_NO,DESCRIPTION,EDIT_DATE,EDIT_PERSON)
                                    VALUES (@CLID,@SID,@TABLE_ID,@QNO,@SERIAL_NO,@DESCRIPTION,getdate(),@EDIT_PERSON) ", mainpulationConditions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //新增空白記錄
        public void AddRECORDS(string CLID, string TABLE_ID, string EDITION_ID)
        {
            var mainpulationConditions = new Conditions()
		    {
			    {"@CLID", CLID },
                {"@TABLE_ID", TABLE_ID },
                {"@EDITION_ID", EDITION_ID },
                {"@EDIT_PERSON", "Protal" }
		    };

            try
            {
                _dc.ExecuteAndCheck(
                                    @"INSERT INTO FORM_RECORDS ( CLID,TABLE_ID,EDITION_ID,EDIT_DATE,EDIT_PERSON)
                                    VALUES (@CLID,@TABLE_ID,@EDITION_ID,getdate(),@EDIT_PERSON) ", mainpulationConditions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //新增調查表簽核狀態
        public void AddFormSign(string CLID, string SID, string TABLE_ID, bool IsSigned, List<RinnaiForms> modelList)
        {
            var trainList = modelList.Cast<TrainViewModel>();
            var model = trainList.First();
            string DocID = "";
            if (model != null)
            {
                DocID = model.SignDocID_FK;
            }
            else
            {
                DocID = "";
            }
            var mainpulationConditions = new Conditions()
		    {
			    {"@CLID", CLID },
                {"@SID", SID },
                {"@TABLE_ID", TABLE_ID },
                {"@SignDocID", DocID },
                {"@IsSigned", IsSigned },
                {"@EDIT_PERSON", "Protal" }
		    };

            try
            {
                _dc.ExecuteAndCheck(
                                    @"INSERT INTO FORM_SIGN (CLID,SID,TABLE_ID,SignDocID,IsSigned,Edit_Date,Edit_Person)
                                    VALUES (@CLID,@SID,@TABLE_ID,@SignDocID,@IsSigned,getdate(),@Edit_Person) ", mainpulationConditions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //刪除調查表簽核狀態
        public MultiConditions DeleteData(string CLID, string SID, string TABLE_ID)
        {
            return new MultiConditions() { { "Delete From FORM_SIGN Where CLID = @CLID and SID=@SID and TABLE_ID=@TABLE_ID", new Conditions() { { "@CLID", CLID }, { "@SID", SID }, { "@TABLE_ID", TABLE_ID } } } };
        }

        /// <summary>
        /// 建立簽核檔以及明細歷程
        /// </summary>
        /// <param name="modelList"></param>
        public void CreateSignData(List<RinnaiForms> modelList)
        {
            //建立簽核ID

            string newSignID = string.Empty;

            #region 取出view傳來的model

            TrainViewModel model = modelList.Cast<TrainViewModel>().First();
            newSignID = GetSeqIDUtils.GetSignDocID(model.Creator, "TR");
            DataRow employeeData = _rootRepo.QueryForEmployeeByEmpID(model.SID);
            if (String.IsNullOrWhiteSpace(newSignID))
                throw new Exception("無法建立簽核ID，或系統忙碌中。請聯絡系統管理員!");
            model.SignDocID_FK = newSignID;
            model.DepartmentID_FK = employeeData != null ? employeeData["DepartmentID_FK"].ToString() : String.Empty;
            //已經帶入登入者的ID
            //model.ApplyID_FK = employeeData != null ? employeeData["EmployeeID"].ToString() : String.Empty;
            //基底model成員

            #endregion 取出view傳來的model

            //欲寫入的簽核狀態
            string status = ((int)SignTypeEnum.READYSIGN).ToString();
            var createDateTime = DateTime.UtcNow.AddHours(8);

            #region 取得簽屬程序

            var procedureData = _rootRepo.QueryForSignProcedureByRuleID(model.RuleID_FK);

            #endregion 取得簽屬程序

            //SignLevel =>需簽核層數
            var remainder = procedureData != null ? Int32.Parse(procedureData["SignLevel"].ToString()) : -1;
            //判斷 FlowRule
            if (!_pwfRepo.IsFollowFlowRule(model.RuleID_FK, model.DepartmentID_FK)) { throw new Exception("不符合簽核規則，無法新增!"); }

            #region 取得簽核人員部門資料

            var deptData = _rootRepo.QueryForDepartmentByDeptID(model.DepartmentID_FK);
            if (deptData == null) { throw new Exception("查無簽核人員所屬部門資料!"); }
            var upperDeptData = _pwfRepo.FindUpperDeptData(model.DepartmentID_FK, model.SID);
            var chiefID = upperDeptData.Keys.Single();
            var currentSignLevelDeptID = upperDeptData.Values.Single();

            #endregion 取得簽核人員部門資料

            //建立多筆SQL Conditons 語句
            var manipulationConditions = new List<MultiConditions>();

            #region 建立簽核主檔SQL語句

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
            // 利用DB Method建立Insert語句 ， 並利用靜態字典 兜出資料庫與Table名稱  RinnaiPortal..signform_main
            var strSQL = _dc.ConstructInsertDML(RepositoryFactory.PotalConn["Catelog"] + "..signform_main", dic);
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            #endregion 建立簽核主檔SQL語句

            #region 建立簽核明細檔SQL語句

            dic = new Conditions()
            {
                {"@SignDocID_FK", model.SignDocID_FK},
                {"@ChiefID_FK", chiefID},
                {"@Status", status},
                {"@Creator", model.Creator},
                {"@CreateDate", createDateTime},
            };
            strSQL = _dc.ConstructInsertDML(RepositoryFactory.PotalConn["Catelog"] + "..signform_detail", dic);
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            #endregion 建立簽核明細檔SQL語句

            #region 建立簽核歷程檔SQL語句

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
            strSQL = _dc.ConstructInsertDML(RepositoryFactory.PotalConn["Catelog"] + "..signform_log", dic);
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            #endregion 建立簽核歷程檔SQL語句

            try
            {
                if (!_dc.ExecuteMultAndCheck(manipulationConditions))
                    throw new Exception("新增受訓心得報告失敗!");
            }
            catch (Exception ex)
            {
                throw new Exception("新增受訓心得報告失敗! system errorMessages ：" + ex.Message);
            }
        }

        /// <summary>
        ///  編輯 送出簽核
        /// </summary>
        /// <param name="modelList"></param>
        public void EditData(List<RinnaiForms> modelList)
        {
            var trainList = modelList.Cast<TrainViewModel>().ToList();
            var model = trainList.First();
            // create SignForm_Main data
            var manipulationConditions = new List<MultiConditions>();
            var strSQL = string.Empty;
            Conditions dic = null;

            var employeeData = _rootRepo.QueryForEmployeeByEmpID(model.SID);
            model.DepartmentID_FK = employeeData != null ? employeeData["DepartmentID_FK"].ToString() : String.Empty;

            var employeeData1 = _rootRepo.QueryForEmployeeByADAccount(model.Creator);
            model.ApplyID_FK = employeeData1 != null ? employeeData1["EmployeeID"].ToString() : String.Empty;
            model.ApplyName = employeeData1 != null ? employeeData1["EmployeeName"].ToString() : String.Empty;

            var status = "2";
            var currentDateTime = DateTime.Now.FormatDatetime();
            var procedureData = _rootRepo.QueryForSignProcedureByRuleID(model.RuleID_FK);
            var remainder = procedureData != null ? Int32.Parse(procedureData["SignLevel"].ToString()) : -1;

            //判斷 FlowRule
            if (!_pwfRepo.IsFollowFlowRule(model.RuleID_FK, model.DepartmentID_FK))
            {
                throw new Exception("不符合簽核規則，無法編輯!");
            }

            var upperDeptData = _pwfRepo.FindUpperDeptData(model.DepartmentID_FK, model.SID);
            var chiefID = upperDeptData.Keys.Single();
            var currentSignLevelDeptID = upperDeptData.Values.Single();

            // Update SignForm_Main 簽核主檔
            string strCatelog = RepositoryFactory.PotalConn["Catelog"];
            strSQL = String.Format(
                    @"UPDATE {0}..signform_main
                    SET    employeeid_fk = @EmployeeID_FK,
	                       senddate = @SendDate,
	                       currentsignleveldeptid_fk = @CurrentSignLevelDeptID_FK,
	                       finalstatus = @FinalStatus,
	                       remainder = @remainder,
	                       modifier = @Modifier,
	                       modifydate = @ModifyDate
                    WHERE  signdocid = @SignDocID ", strCatelog);
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

            // create SignForm_Detail data  簽核明細檔(歷程)
            strSQL = String.Format(
                    @"INSERT INTO {0}..signform_detail
			                    (signdocid_fk,
			                     chiefid_fk,
			                     status,
			                     creator,
			                     createdate)
                    VALUES      (@SignDocID_FK,
			                     @ChiefID_FK,
			                     @Status,
			                     @Creator,
			                     @CreateDate) ", strCatelog);
            dic = new Conditions()
			{
				{"@SignDocID_FK", model.SignDocID_FK},
				{"@ChiefID_FK", chiefID},
				{"@Status", status},
				{"@Creator", model.Modifier},
				{"@CreateDate", currentDateTime},
			};
            manipulationConditions.Add(new MultiConditions() { { strSQL, dic } });

            // add log 簽核明現況檔
            strSQL = String.Format(
                    @"INSERT INTO {0}..SIGNFORM_LOG
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
			                     @LogDatetime ) ", strCatelog);
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

            try
            {
                if (!_dc.ExecuteMultAndCheck(manipulationConditions)) { throw new Exception("編輯受訓心得報告失敗!"); }
            }
            catch (Exception ex)
            {
                throw new Exception("編輯受訓心得報告失敗!" + ex.Message);
            }
        }

        /// <summary>
        /// 找出課程編號
        /// </summary>
        /// <param name="SignDocID"></param>
        /// <returns></returns>
        public string Find_CLID(string SignDocID)
        {
            string Find_CLID = string.Empty;
            string strSQL = @"select CLID from FORM_SIGN  WHERE SignDocID='" + SignDocID + "'";
            temp = DBClass.Create_Table_T(strSQL, "GET");
            if (temp.Rows.Count > 0)
            {
                Find_CLID = temp.Rows[0]["CLID"].ToString();
            }
            return Find_CLID;
        }

        /// <summary>
        /// 找出填單學員工號
        /// </summary>
        /// <param name="SignDocID"></param>
        /// <returns></returns>
        public string Find_SID(string SignDocID)
        {
            string Find_SID = string.Empty;
            string strSQL = @"select SID from FORM_SIGN  WHERE SignDocID='" + SignDocID + "'";
            temp = DBClass.Create_Table_T(strSQL, "GET");
            if (temp.Rows.Count > 0)
            {
                Find_SID = temp.Rows[0]["SID"].ToString();
            }
            return Find_SID;
        }
    }
}