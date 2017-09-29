using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RinnaiPortal.ViewModel;
using RinnaiPortal.ViewModel.Sign;
using RinnaiPortal.Interface;
using RinnaiPortal.Tools;
using DBTools;
using RinnaiPortal.Extensions;
using RinnaiPortal.FactoryMethod;

namespace RinnaiPortal.Repository.Sign
{
	public class GroupRepository : ISignRepository
	{
		private DB _dc { get; set; }
		private RootRepository _rootRepo { get; set; }
		public GroupRepository(DB dc, RootRepository rootRepo)
		{
			_dc = dc;
			_rootRepo = rootRepo;
		}

		//列表
		public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
		{
			string strSQL =
@"SELECT * FROM   sysGroup {0}";

			string strConditions = string.Empty;
			Conditions conditionsDic = null;
			//組成查詢條件 SQL
			if (!String.IsNullOrWhiteSpace(slParms.QueryText))
			{
				conditionsDic = new Conditions
				{
					{ "@queryText", String.Format("{0}%", slParms.QueryText.Trim())}
				};
				strConditions = String.Format("where GroupType like @queryText or GroupName like @queryText ");
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
				"GroupType", "GroupName", "Resource", "Creator", "CreateDate", "Modifier", "ModifyDate"
			};
			//根據 SQL取得　Pagination
			return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
		}

		//是否存在群組代碼
		public bool IsExistGroupType(string groupType)
		{
			var conditions = new Conditions() { { "@GroupType", groupType } };
			var sql = @"select * from sysGroup where GroupType = @GroupType";
			var rows = _dc.QueryForRowsCount(sql, conditions);

			return (rows > 0) ? true : false;
		}

		//新增
		public void CreateData(GroupViewModel model)
		{
			//model 不得為 null
			if (model == null) { throw new Exception("請檢查輸入的資料!"); }

			//判斷有無此群組代碼
			if (IsExistGroupType(model.GroupType)) { throw new Exception("新增失敗，已經存在相同群組代碼!"); }

			var mainpulationConditions = new Conditions()
			{
				{"@GroupType", model.GroupType },
				{"@GroupName", model.GroupName},
				{"@Resource", model.Resource},
				{"@Creator", model.Creator},
				{"@CreateDate", model.CreateDate},
			};

			try
			{
				_dc.ExecuteAndCheck(
@"INSERT INTO SysGroup 
			(GroupType, 
				GroupName, 
				Resource, 
				creator, 
				createdate) 
VALUES      (@GroupType, 
				@GroupName, 
				@Resource, 
				@Creator, 
				@CreateDate) ", mainpulationConditions);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		//編輯頁面 > 取得群組資料
		public GroupViewModel GetGroupData(string groupType)
		{
			GroupViewModel model = null;
			string strSQL = @"Select * from sysGroup where GroupType = @GroupType";
			var strCondition = new Conditions() { { "@GroupType", groupType } };

			var result = _dc.QueryForDataRow(strSQL, strCondition);
			if (result == null) { return null; }

			model = new GroupViewModel()
			{
				GroupType = result["GroupType"].ToString(),
				GroupName = result["GroupName"].ToString(),
				Resource = result["Resource"].ToString(),
				Modifier = result["Modifier"].ToString(),
				ModifyDate = !result["ModifyDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ModifyDate"].ToString()) : DateTime.Parse(result["CreateDate"].ToString()),
				TimeStamp = (result["ModifyDate"].IsDBNullOrWhiteSpace() ? result["CreateDate"].ToString() : result["ModifyDate"].ToString()).ToDateFormateString(),
			};

			return model;
		}


		//編輯
		public void EditData(GroupViewModel model)
		{
			var orgModel = GetGroupData(model.GroupType);
			//model 不得為 null
			if (model == null || orgModel == null) { throw new Exception("請檢查輸入的資料!");}

			if (!model.TimeStamp.Equals(orgModel.ModifyDate.FormatDatetimeNullable())) { throw new Exception("此筆資料已被他人更新!請重新確認!"); }

			var mainpulationConditions = new Conditions()
			{
				{"@GroupType", model.GroupType },
				{ "@GroupName", model.GroupName },
				{ "@Resource", model.Resource },
				{"@Modifier", model.Modifier}
			};
			try
			{
				 _dc.ExecuteAndCheck(
@"Update SysGroup Set 
		 GroupName = @GroupName,
		 Resource = @Resource, 
		 Modifier = @Modifier,
		 ModifyDate = getDate()
 Where GroupType = @GroupType", mainpulationConditions);

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}