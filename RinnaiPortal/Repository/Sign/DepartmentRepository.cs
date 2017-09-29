using DBTools;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository.Sign;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using RinnaiPortal.FactoryMethod;

namespace RinnaiPortal.Repository
{
	public class DepartmentRepository : ISignRepository
	{
		private DB _dc { get; set; }
		private RootRepository _rootRepo { get; set; }
		public DepartmentRepository(DB dc, RootRepository rootRepo)
		{
			_dc = dc;
			_rootRepo = rootRepo;
		}

		//列表
		public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
		{
			string strSQL =
@"SELECT dpt.*, emp.EmployeeName as ChiefName, emp2.EmployeeName as FilingEmployeeName, dpt2.DepartmentName as upperDepartmentName FROM Department dpt 
left outer join Employee emp on dpt.chiefID_FK = emp.EmployeeID
left outer join Employee emp2 on dpt.FilingEmployeeID_FK = emp2.EmployeeID
left outer join Department dpt2 on dpt.upperDepartmentID = dpt2.DepartmentID 
 {0}";

			string strConditions = string.Empty;
			Conditions conditionsDic = null;
			//組成查詢條件 SQL
			if (!String.IsNullOrWhiteSpace(slParms.QueryText))
			{
				conditionsDic = new Conditions() { { "@queryText", String.Format("{0}%", slParms.QueryText.Trim()) } };
				strConditions = String.Format("where dpt.DepartmentID like @queryText or dpt.DepartmentName like @queryText or emp.EmployeeName like @queryText");
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
				"DepartmentID", "DepartmentName", "ChiefID_FK", "ChiefName", "UpperDepartmentID", "UpperDepartmentName", "DepartmentLevel", "FilingEmployeeID_FK",
				"FilingEmployeeName", "Disabled", "DisabledDate", "Creator", "CreateDate", "Modifier", "ModifyDate",
			};
			//根據 SQL取得　Pagination
			return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
		}

		//是否存在部門代碼
		public bool IsExistDepartmentID(string id)
		{
			var conditions = new Conditions() { { "@DepartmentID", id } };
			var sql = @"select * from Department where DepartmentID = @DepartmentID";
			var rows = _dc.QueryForRowsCount(sql, conditions);

			return (rows > 0) ? true : false;
		}

		//新增
		public void CreateData(DepartmentViewModel model)
		{
			//model 不得為 null
			if (model == null) { throw new Exception("請檢查輸入的資料!"); }

			//判斷有無此部門代碼
			if (IsExistDepartmentID(model.DepartmentID)) { throw new Exception("新增失敗，已經存在相同部門代碼!"); }

			var mainpulationConditions = new Conditions()
			{
				{"@DepartmentID", model.DepartmentID },
				{"@DepartmentName", model.DepartmentName},
				{"@ChiefID_FK", model.ChiefID_FK},
				{"@UpperDepartmentID", model.UpperDepartmentID},
				{"@FilingEmployeeID_FK", model.FilingEmployeeID_FK},
				{"@DepartmentLevel", model.DepartmentLevel},
				{"@Disabled", model.Disabled},
				{"@DisabledDate", model.DisabledDate},
				{"@Creator", model.Creator},
				{"@CreateDate", model.CreateDate},
			};

			try
			{
				_dc.ExecuteAndCheck(
@"INSERT INTO department 
			(departmentid, 
			 departmentname, 
			 chiefid_fk, 
			 upperdepartmentid, 
			 filingemployeeid_fk, 
			 departmentlevel, 
			 disabled, 
			 disableddate, 
			 creator, 
			 createdate) 
VALUES      (@DepartmentID, 
			 @DepartmentName, 
			 @ChiefID_FK, 
			 @UpperDepartmentID, 
			 @FilingEmployeeID_FK, 
			 @DepartmentLevel, 
			 @Disabled, 
			 CASE 
			   WHEN @DisabledDate = '' THEN NULL 
			   ELSE @DisabledDate 
			 END, 
			 @Creator, 
			 @CreateDate) ", mainpulationConditions);
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}



		//編輯頁面 > 取得部門資料
		public DepartmentViewModel GetDepartmentData(string departmentID)
		{
			DepartmentViewModel model = null;
			string strSQL = @"Select * from Department where DepartmentID = @DepartmentID";
			var strCondition = new Conditions() { { "@DepartmentID", departmentID } };

			var result = _dc.QueryForDataRow(strSQL, strCondition);
			if (result == null) { return null; }

			model = new DepartmentViewModel()
			{
				DepartmentID = result["DepartmentID"].ToString(),
				DepartmentName = result["DepartmentName"].ToString(),
				ChiefID_FK = result["ChiefID_FK"].ToString(),
				UpperDepartmentID = result["UpperDepartmentID"].ToString(),
				DepartmentLevel = Int32.Parse(result["DepartmentLevel"].ToString()),
				FilingEmployeeID_FK = result["FilingEmployeeID_FK"].ToString(),
				Disabled = "True".Equals(result["Disabled"].ToString()),
				DisabledDate = !result["DisabledDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["DisabledDate"].ToString()) : (DateTime?)null,
				Modifier = result["Modifier"].ToString(),
				ModifyDate = !result["ModifyDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ModifyDate"].ToString()) : DateTime.Parse(result["CreateDate"].ToString()),
				TimeStamp = (result["ModifyDate"].IsDBNullOrWhiteSpace() ? result["CreateDate"].ToString() : result["ModifyDate"].ToString()).ToDateFormateString(),

				EmployeeDic = _rootRepo.GetEmployee(),
				DepartmentDic = _rootRepo.GetDepartment(),
			};

			return model;
		}


		//編輯
		public void EditData(DepartmentViewModel model)
		{
			var orgModel = GetDepartmentData(model.DepartmentID);
			//model 不得為 null
			if (model == null || orgModel == null) { throw new Exception("請檢查輸入的資料!"); }

			if (!model.TimeStamp.Equals(orgModel.ModifyDate.FormatDatetimeNullable())) { throw new Exception("此筆資料已被他人更新!請重新確認!"); }

			var mainpulationConditions = new Conditions()
			{
				{ "@DepartmentID", model.DepartmentID },
				{ "@DepartmentName", model.DepartmentName },
				{ "@ChiefID_FK", model.ChiefID_FK },
				{ "@UpperDepartmentID", model.UpperDepartmentID },
				{ "@DepartmentLevel", model.DepartmentLevel },
				{ "@FilingEmployeeID_FK", model.FilingEmployeeID_FK },
				{"@Disabled", model.Disabled},
				{"@DisabledDate", model.DisabledDate},
				{"@Modifier", model.Modifier}
			};

			try
			{
				_dc.ExecuteAndCheck(
@"Update Department Set 
		 DepartmentID = @DepartmentID,
		 DepartmentName = @DepartmentName, 
		 ChiefID_FK = @ChiefID_FK, 
		 UpperDepartmentID = @UpperDepartmentID, 
		 DepartmentLevel = @DepartmentLevel, 
		 FilingEmployeeID_FK = @FilingEmployeeID_FK, 
		 Disabled = @Disabled, 
		 DisabledDate = Case When @DisabledDate = '' then null else @DisabledDate end, 
		 Modifier = @Modifier,
		 ModifyDate = getDate()
 Where DepartmentID = @DepartmentID", mainpulationConditions);
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}
	}
}