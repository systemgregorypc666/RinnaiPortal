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
	public class TypeRepository : ISignRepository
	{
		private DB _dc { get; set; }
		private RootRepository _rootRepo { get; set; }
		public TypeRepository(DB dc, RootRepository rootRepo)
		{
			_dc = dc;
			_rootRepo = rootRepo;
		}

		//列表
		public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
		{
			string strSQL =
@"SELECT typ.*, 
	   dpt.departmentname AS FilingDepartmentName 
FROM   signtype typ 
	   LEFT OUTER JOIN department dpt 
					ON typ.filingdepartmentid_fk = dpt.departmentid 
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
				strConditions = String.Format("where FormType like @queryText or FormID like @queryText or SignID_FK like @queryText");
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
				"FormID", "FormType", "SignID_FK", "FilingDepartmentID_FK", "Creator", "CreateDate", "Modifier", "ModifyDate"
			};
			//根據 SQL取得　Pagination
			return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
		}

		//是否存在表單類型
		public bool IsExistFormType(string type)
		{
			var conditions = new Conditions()
			{
				{"@FormType", type}
			};
			var sql = "select * from SignType where FormType = @FormType";
			var rows = _dc.QueryForRowsCount(sql, conditions);

			return (rows > 0) ? true : false;
		}

		//新增
		public void CreateData(TypeViewModel model)
		{
			//model 不得為 null
			if (model == null) { throw new Exception("請檢查輸入的資料!"); }

			//判斷有無此表單類型
			if (IsExistFormType(model.FormType)) { throw new Exception("新增失敗，已經存在相同表單類型!"); }

			var mainpulationConditions = new Conditions()
			{
				{"@FormType", model.FormType },
				{"@SignID_FK", model.SignID_FK},
				{"@FilingDepartmentID_FK", model.FilingDepartmentID_FK},
				{"@Creator", model.Creator},
				{"@CreateDate", model.CreateDate},
			};

			try
			{
				_dc.ExecuteAndCheck(
@"INSERT INTO signtype 
			(formtype, 
			 signid_fk, 
			 filingdepartmentid_fk, 
			 creator, 
			 createdate) 
VALUES      (@FormType, 
			 @SignID_FK, 
			 @FilingDepartmentID_FK, 
			 @Creator, 
			 @CreateDate) ", mainpulationConditions);

			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

		//編輯頁面 > 取得表單簽核資料類型
		public TypeViewModel GetTypeData(int formID)
		{
			TypeViewModel model = null;
			string strSQL = @"Select * from SignType where FormID = @FormID";
			var strCondition = new Conditions() { {"@FormID", formID}};

			var result = _dc.QueryForDataRow(strSQL, strCondition);
			if (result == null){return null;}

			model = new TypeViewModel()
			{
				FormID = Int32.Parse(result["FormID"].ToString()),
				FormType = result["FormType"].ToString(),
				SignID_FK = result["SignID_FK"].ToString(),
				FilingDepartmentID_FK = result["FilingDepartmentID_FK"].ToString(),
				Modifier = result["Modifier"].ToString(),
				ModifyDate = !result["ModifyDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ModifyDate"].ToString()) : DateTime.Parse(result["CreateDate"].ToString()),
				TimeStamp = (result["ModifyDate"].IsDBNullOrWhiteSpace() ? result["CreateDate"].ToString() : result["ModifyDate"].ToString()).ToDateFormateString(),
				DepartmentDic = _rootRepo.GetDepartment(),
				SignIDDic = _rootRepo.GetSignProcedure(),
			};

			return model;
		}

		//編輯
		public void EditData(TypeViewModel model)
		{
			var orgModel = GetTypeData(model.FormID);
			//model 不得為 null
			if (model == null || orgModel == null) { throw new Exception("請檢查輸入的資料!"); }

			if (!model.TimeStamp.Equals(orgModel.ModifyDate.FormatDatetimeNullable())) { throw new Exception("此筆資料已被他人更新!請重新確認!"); }

			var mainpulationConditions = new Conditions()
			{
				{"@FormID", model.FormID },
				{"@FormType", model.FormType },
				{"@FilingDepartmentID_FK", model.FilingDepartmentID_FK},
				{"@SignID_FK", model.SignID_FK},
				{"@Modifier", model.Modifier}
			};

			try
			{
				_dc.ExecuteAndCheck(
@"Update SignType Set 
		 FormType = @FormType,
		 FilingDepartmentID_FK = @FilingDepartmentID_FK, 
		 SignID_FK = @SignID_FK,        
		 Modifier = @Modifier,
		 ModifyDate = getDate()
 Where FormID = @FormID", mainpulationConditions);
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}


	}
}