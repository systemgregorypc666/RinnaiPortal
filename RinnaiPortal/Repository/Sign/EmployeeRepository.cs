using DBTools;
using RinnaiPortal.Extensions;
using RinnaiPortal.Interface;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign;
using System;
using System.Collections.Generic;

namespace RinnaiPortal.Repository
{
    public class EmployeeRepository : ISignRepository
    {
        private DB _dc { get; set; }
        private RootRepository _rootRepo { get; set; }

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="rootRepo"></param>
        public EmployeeRepository(DB dc, RootRepository rootRepo)
        {
            _dc = dc;
            _rootRepo = rootRepo;
        }

        /// <summary>
        /// 分頁列表順帶資料
        /// </summary>
        /// <param name="slParms"></param>
        /// <param name="pParms"></param>
        /// <returns></returns>
        public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
        {
            string strSQL =
            @"SELECT emp.*,
                dep.departmentname AS DepartmentName,
                emp2.employeename  AS AgentName
                FROM   employee emp
                LEFT OUTER JOIN department dep
                ON emp.departmentid_fk = dep.departmentid
                LEFT OUTER JOIN employee emp2
                ON emp.agentid = emp2.employeeid
                {0}";

            string strConditions = string.Empty;
            Conditions conditionsDic = null;
            //組成查詢條件 SQL
            if (!String.IsNullOrWhiteSpace(slParms.QueryText))
            {
                conditionsDic = new Conditions()
				{
					{ "@queryText", String.Format("{0}%", slParms.QueryText.Trim())}
				};
                strConditions = String.Format("where emp.EmployeeID like @queryText or emp.EmployeeName like @queryText or emp.DepartmentID_FK like @queryText");
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
				"EmployeeID", "EmployeeName", "DepartmentID_FK", "DepartmentName", "AgentID", "AgentName", "Disabled", "DisabledDate",
				"ADAccount", "Creator", "CreateDate", "Modifier", "ModifyDate"
			};
            //根據 SQL取得　Pagination
            return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
        }

        /// <summary>
        /// 檢查是否存在員工編號
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsExistEmployeeID(string id)
        {
            var conditions = new Conditions() { { "@EmployeeID", id } };
            var sql = @"select * from Employee where EmployeeID = @EmployeeID";
            var rows = _dc.QueryForRowsCount(sql, conditions);

            return (rows > 0) ? true : false;
        }

        /// <summary>
        /// 新增一筆員工資料
        /// </summary>
        /// <param name="model"></param>
        public void CreateData(EmployeeViewModel model)
        {
            //model 不得為 null
            if (model == null) { new Exception("請檢查輸入的資料!"); }

            //判斷有無此員工編號
            if (IsExistEmployeeID(model.EmployeeID)) { throw new Exception("新增失敗，已經存在相同員工編號!"); }

            var mainpulationConditions = new Conditions()
			{
				{"@EmployeeID", model.EmployeeID },
				{"@EmployeeName", model.EmployeeName},
				{"@DepartmentID_FK", model.DepartmentID_FK},
                {"@CostDepartmentID", model.CostDepartmentID},
				{"@AgentID", model.AgentID},
				{"@Disabled", model.Disabled},
				{"@DisabledDate", model.DisabledDate},
				{"@ADAccount", model.ADAccount},
				{"@Creator", model.Creator},
				{"@CreateDate", model.CreateDate},
				{"@AccessType", model.AccessType},
				{"@NationalType", model.NationalType},
                {"@SexType", model.SexType},
			};
            try
            {
                _dc.ExecuteAndCheck(
                @"INSERT INTO employee
                (
                    employeeid,
                    employeename,
                    departmentid_fk,
                    CostDepartmentID,
                    agentid,
                    disabled,
                    disableddate,
                    adaccount,
                    creator,
                    createdate,
                    accesstype,
                    nationaltype,SexType
)
                VALUES
                (
                    @EmployeeID,
                    @EmployeeName,
                    @DepartmentID_FK,
                    @CostDepartmentID,
                    @AgentID,
                    @Disabled,
                    CASE
                    WHEN @DisabledDate = '' THEN NULL
                    ELSE @DisabledDate
                    END,
                    @ADAccount,
                    @Creator,
                    @CreateDate,
                    @AccessType,
                    @NationalType,@SexType
                ) ",
                mainpulationConditions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得員工資料
        /// </summary>
        /// <param name="EmployeeID"></param>
        /// <returns></returns>
        public EmployeeViewModel GetEmployeeDataByID(string EmployeeID)
        {
            EmployeeViewModel model = null;
            string strSQL = @"Select * from Employee where EmployeeID = @EmployeeID";
            var strCondition = new Conditions() { { "@EmployeeID", EmployeeID } };

            var result = _dc.QueryForDataRow(strSQL, strCondition);
            if (result == null) { return null; }

            model = new EmployeeViewModel()
            {
                EmployeeID = result["EmployeeID"].ToString(),
                EmployeeName = result["EmployeeName"].ToString(),
                DepartmentID_FK = result["DepartmentID_FK"].ToString(),
                CostDepartmentID = result["CostDepartmentID"].ToString(),
                AgentID = result["AgentID"].ToString(),
                Disabled = "True".Equals(result["Disabled"].ToString()),
                DisabledDate = !result["DisabledDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["DisabledDate"].ToString()) : (DateTime?)null,
                Modifier = result["Modifier"].ToString(),
                ModifyDate = !result["ModifyDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ModifyDate"].ToString()) : DateTime.Parse(result["CreateDate"].ToString()),
                AccessType = result["AccessType"].ToString(),
                NationalType = result["NationalType"].ToString().ToUpper(),
                SexType = result["SexType"].ToString().ToUpper(),
                ADAccount = result["ADAccount"].ToString(),
                TimeStamp = (result["ModifyDate"].IsDBNullOrWhiteSpace() ? result["CreateDate"].ToString() : result["ModifyDate"].ToString()).ToDateFormateString(),
                //取得代理人員資料
                AgentNameDic = _rootRepo.GetEmployee(),
                //取得部門資料
                DepartmentDic = _rootRepo.GetDepartment(),
            };
            return model;
        }


        /// <summary>
        /// 取得員工資料 根據AADAccount
        /// </summary>
        /// <param name="EmployeeID"></param>
        /// <returns></returns>
        public EmployeeViewModel GetEmployeeDataByADAccount(string ADAccount)
        {
            EmployeeViewModel model = null;
            string strSQL = @"Select * from Employee where ADAccount = @ADAccount";
            var strCondition = new Conditions() { { "@ADAccount", ADAccount } };

            var result = _dc.QueryForDataRow(strSQL, strCondition);
            if (result == null) { return null; }

            model = new EmployeeViewModel()
            {
                EmployeeID = result["EmployeeID"].ToString(),
                EmployeeName = result["EmployeeName"].ToString(),
                DepartmentID_FK = result["DepartmentID_FK"].ToString(),
                CostDepartmentID = result["CostDepartmentID"].ToString(),
                AgentID = result["AgentID"].ToString(),
                Disabled = "True".Equals(result["Disabled"].ToString()),
                DisabledDate = !result["DisabledDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["DisabledDate"].ToString()) : (DateTime?)null,
                Modifier = result["Modifier"].ToString(),
                ModifyDate = !result["ModifyDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ModifyDate"].ToString()) : DateTime.Parse(result["CreateDate"].ToString()),
                AccessType = result["AccessType"].ToString(),
                NationalType = result["NationalType"].ToString().ToUpper(),
                SexType = result["SexType"].ToString().ToUpper(),
                ADAccount = result["ADAccount"].ToString(),
                TimeStamp = (result["ModifyDate"].IsDBNullOrWhiteSpace() ? result["CreateDate"].ToString() : result["ModifyDate"].ToString()).ToDateFormateString(),
                //取得代理人員資料
                AgentNameDic = _rootRepo.GetEmployee(),
                //取得部門資料
                DepartmentDic = _rootRepo.GetDepartment(),
            };
            return model;
        }

        /// <summary>
        /// 編輯員工資料
        /// </summary>
        /// <param name="model"></param>
        public void EditData(EmployeeViewModel model)
        {
            var orgModel = GetEmployeeDataByID(model.EmployeeID);
            //model 不得為 null
            if (model == null || orgModel == null) { new Exception("請檢查輸入的資料!"); }

            if (!model.TimeStamp.Equals(orgModel.ModifyDate.FormatDatetimeNullable())) { new Exception("此筆資料已被他人更新!請重新確認!"); }

            var mainpulationConditions = new Conditions()
			{
				{"@EmployeeID", model.EmployeeID },
				{"@EmployeeName", model.EmployeeName},
				{"@DepartmentID_FK", model.DepartmentID_FK},
                {"@CostDepartmentID", model.CostDepartmentID},
				{"@AgentID", model.AgentID},
				{"@Disabled", model.Disabled},
				{"@DisabledDate", model.DisabledDate},
				{"@Modifier", model.Modifier},
				{"@AccessType", model.AccessType},
				{"@NationalType", model.NationalType.ToUpper()},
                {"@SexType", model.SexType.ToUpper()},
				{"@ADAccount", model.ADAccount},
			};
            try
            {
                _dc.ExecuteAndCheck(
                @"Update Employee
                Set
                    EmployeeID = @EmployeeID,
                    EmployeeName = @EmployeeName,
                    DepartmentID_FK = @DepartmentID_FK,
                    CostDepartmentID = @CostDepartmentID,
                    AgentID = @AgentID,
                    Disabled = @Disabled,
                    DisabledDate = Case When @DisabledDate = '' then null else @DisabledDate end,
                    Modifier = @Modifier,
                    ModifyDate = getDate(),
                    AccessType = @AccessType,
                    NationalType = @NationalType,
                    SexType = @SexType,
                    ADAccount = @ADAccount
                    Where EmployeeID = @EmployeeID",
                mainpulationConditions);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 國家類別
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GenarationNationalType()
        {
            return new Dictionary<string, string>()
			{
				{"","請選擇"},
				{"TAIWAN","台灣"},
				{"JAPAN","日本"},
				{"VIETNAM","越南"},
				{"INDONESIA","印尼"},
			};
        }

        /// <summary>
        /// 性別類別
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GenarationSexType()
        {
            return new Dictionary<string, string>()
			{
				{"","請選擇"},
				{"M","男"},
				{"F","女"},
			};
        }
    }
}