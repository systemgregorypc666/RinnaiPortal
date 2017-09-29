using DBTools;
using RinnaiPortal.Repository.Sign;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.Tools;
using RinnaiPortal.Interface;
using RinnaiPortal.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using RinnaiPortal.FactoryMethod;

namespace RinnaiPortal.Repository
{
	public class AgentRepository : ISignRepository
	{
		private DB _dc { get; set; }
		private RootRepository _rootRepo { get; set; }
		public AgentRepository(DB dc, RootRepository rootRepo)
		{
			_dc = dc;
			_rootRepo = rootRepo;
		}
		//列表
		public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
		{
			string strSQL =
@"SELECT agt.*,emp.employeeName FROM SignAgent agt
left outer join Employee emp
on agt.EmployeeID_FK = emp.EmployeeID
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
				strConditions = String.Format("where agt.EmployeeID_FK like @queryText or agt.SN like @queryText");
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
				"SN", "EmployeeID_FK", "EmployeeName","BeginDate", "EndDate", "Creator", "CreateDate", "Modifier", "ModifyDate",
			};
			//根據 SQL取得　Pagination
			return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
		}

		//新增
		public void CreateData(AgentViewModel model)
		{
			//model 不得為 null
			if (model == null) { throw new Exception("請檢查輸入的資料!"); }

			var mainpulationConditions = new Conditions()
			{
				{"@EmployeeID_FK", model.EmployeeID_FK },
				{"@BeginDate", model.BeginDate},
				{"@EndDate", model.EndDate},
				{"@Creator", model.Creator},
				{"@CreateDate", model.CreateDate},
			};

			try
			{
				_dc.ExecuteAndCheck(
@"Insert into SignAgent (EmployeeID_FK, BeginDate, EndDate, Creator, CreateDate)
	Values (@EmployeeID_FK, @BeginDate, @EndDate, @Creator, @CreateDate)", mainpulationConditions);
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}


		public AgentViewModel GetAgentData(string sn)
		{
			AgentViewModel model = null;
			string strSQL = @"Select * from SignAgent where SN = @SN";
			var strCondition = new Conditions() { { "@SN", sn } };

			var result = _dc.QueryForDataRow(strSQL, strCondition);
			if (result == null) { return null; }

			model = new AgentViewModel()
			{
				SN = Int32.Parse(result["SN"].ToString()),
				EmployeeID_FK = result["EmployeeID_FK"].ToString(),
				BeginDate = DateTime.Parse(result["BeginDate"].ToString()),
				EndDate = DateTime.Parse(result["EndDate"].ToString()),
				Modifier = result["Modifier"].ToString(),
				ModifyDate = !result["ModifyDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ModifyDate"].ToString()) : DateTime.Parse(result["CreateDate"].ToString()),
				TimeStamp = (result["ModifyDate"].IsDBNullOrWhiteSpace() ? result["CreateDate"].ToString() : result["ModifyDate"].ToString()).ToDateFormateString(),
				EmployeeDic = _rootRepo.GetEmployee(),
			};

			return model;
		}

		//編輯
		public void EditData(AgentViewModel model)
		{
			var orgModel = GetAgentData(model.SN.ToString());
			//model 不得為 null
			if (model == null || orgModel == null) { throw new Exception("請檢查輸入的資料!"); }

			if (!model.TimeStamp.Equals(orgModel.ModifyDate.FormatDatetimeNullable())) { throw new Exception("此筆資料已被他人更新!請重新確認!"); }

			var mainpulationConditions = new Conditions()
			{
				{"@SN", model.SN },
				{"@EmployeeID_FK", model.EmployeeID_FK },
				{"@BeginDate", model.BeginDate},
				{"@EndDate", model.EndDate},
				{"@Modifier", model.Modifier}
			};

			try
			{
				_dc.ExecuteAndCheck(
@"Update SignAgent Set 
		 EmployeeID_FK = @EmployeeID_FK,
		 BeginDate = @BeginDate, 
		 EndDate = @EndDate, 
		 Modifier = @Modifier,
		 ModifyDate = getDate()
 Where SN = @SN", mainpulationConditions);

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}