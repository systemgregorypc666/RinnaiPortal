using DBTools;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository.Sign;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Repository
{
	public class ProcedureRepository : ISignRepository
	{
		private DB _dc { get; set; }
		public ProcedureRepository(DB dc)
		{
			_dc = dc;
		}

		//列表
		public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
		{
			string strSQL = @"SELECT * FROM SignProcedure {0}";

			string strConditions = string.Empty;
			Conditions conditionsDic = null;
			//組成查詢條件 SQL
			if (!String.IsNullOrWhiteSpace(slParms.QueryText))
			{
				conditionsDic = new Conditions() { { "@queryText", String.Format("{0}%", slParms.QueryText.Trim()) } };
				strConditions = String.Format("where SignID like @queryText or SignLevel like @queryText or MaxLevel like @queryText");
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
				"SignID", "SignLevel", "MaxLevel", "Disabled", "DisabledDate", "Creator", "CreateDate", "Modifier", "ModifyDate"
			};
			//根據 SQL取得　Pagination
			return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
		}

		//是否存在簽核代碼
		public bool IsExistSignID(string id)
		{
			var conditions = new Conditions() { { "@SignID", id } };
			var sql = @"select * from SignProcedure where SignID = @SignID";
			var rows = _dc.QueryForRowsCount(sql, conditions);

			return (rows > 0) ? true : false;
		}

		//新增
		public void CreateData(ProcedureViewModel model)
		{
			//model 不得為 null
			if (model == null) { throw new Exception("請檢查輸入的資料!"); }

			//判斷有無此簽核代碼
			if (IsExistSignID(model.SignID)) { throw new Exception("新增失敗，已經存在相同簽核層級代碼!"); }

			var mainpulationConditions = new Conditions()
			{
				{"@SignID", model.SignID },
				{"@SignLevel", model.SignLevel},
				{"@MaxLevel", model.MaxLevel},
				{"@Disabled", model.Disabled},
				{"@DisabledDate", model.DisabledDate},
				{"@Creator", model.Creator},
				{"@CreateDate", model.CreateDate},
			};
			try
			{
				_dc.ExecuteAndCheck(
@"INSERT INTO signprocedure 
			(signid, 
			 signlevel, 
			 maxlevel, 
			 disabled, 
			 disableddate, 
			 creator, 
			 createdate) 
VALUES      (@SignID, 
			 @SignLevel, 
			 @MaxLevel, 
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


		//編輯頁面 > 取得簽核代碼資料
		public ProcedureViewModel GetProcedureData(string signID)
		{
			ProcedureViewModel model = null;
			string strSQL = @"Select * from SignProcedure where SignID = @SignID";
			var strCondition = new Conditions() { { "@SignID", signID } };

			var result = _dc.QueryForDataRow(strSQL, strCondition);
			if (result == null) { return null; }

			model = new ProcedureViewModel()
			{
				SignID = result["SignID"].ToString(),
				SignLevel = Int32.Parse(result["SignLevel"].ToString()),
				MaxLevel = Int32.Parse(result["MaxLevel"].ToString()),
				Disabled = "True".Equals(result["Disabled"].ToString()),
				DisabledDate = !result["DisabledDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["DisabledDate"].ToString()) : (DateTime?)null,
				Modifier = result["Modifier"].ToString(),
				ModifyDate = !result["ModifyDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ModifyDate"].ToString()) : DateTime.Parse(result["CreateDate"].ToString()),
				TimeStamp = (result["ModifyDate"].IsDBNullOrWhiteSpace() ? result["CreateDate"].ToString() : result["ModifyDate"].ToString()).ToDateFormateString(),
			};

			return model;
		}

		//編輯
		public void EditData(ProcedureViewModel model)
		{
			var orgModel = GetProcedureData(model.SignID);
			//model 不得為 null
			if (model == null || orgModel == null) { throw new Exception("請檢查輸入的資料!"); }

			if (!model.TimeStamp.Equals(orgModel.ModifyDate.FormatDatetimeNullable())) { throw new Exception("此筆資料已被他人更新!請重新確認!"); }

			var mainpulationConditions = new Conditions()
			{
				{ "@SignID", model.SignID },
				{ "@SignLevel", model.SignLevel },
				{ "@MaxLevel", model.MaxLevel },
				{"@Disabled", model.Disabled},
				{"@DisabledDate", model.DisabledDate},
				{"@Modifier", model.Modifier}
			};
			try
			{
				 _dc.ExecuteAndCheck(
@"Update SignProcedure Set 
		 SignID = @SignID,
		 SignLevel = @SignLevel, 
		 MaxLevel = @MaxLevel, 
		 Disabled = @Disabled, 
		 DisabledDate = Case When @DisabledDate = '' then null else @DisabledDate end, 
		 Modifier = @Modifier,
		 ModifyDate = getDate()
 Where SignID = @SignID", mainpulationConditions);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}