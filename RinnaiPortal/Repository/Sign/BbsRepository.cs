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
	public class BbsRepository : ISignRepository
	{
        DataTable temp = new DataTable();
		private DB _dc { get; set; }
		private RootRepository _rootRepo { get; set; }
		public BbsRepository(DB dc, RootRepository rootRepo)
		{
			_dc = dc;
			_rootRepo = rootRepo;
		}

		//列表
		public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
		{
			string strSQL = @"SELECT * FROM BbsContent bbs {0}";

			string strConditions = string.Empty;
			Conditions conditionsDic = null;
			//組成查詢條件 SQL
			if (!String.IsNullOrWhiteSpace(slParms.QueryText))
			{
				//conditionsDic = new Conditions() { { "@queryText", String.Format("{0}", slParms.QueryText.Trim()) } };
                strConditions = String.Format("where bbs.bbs_title like '%" + slParms.QueryText.Trim() + "%' ");
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
				"bbs_id", "bbs_title", "bbs_content", "bbs_http", "bbs_photo", "bbs_file", "startdatetime", "enddatetime", "Creator", "CreateDate", "Modifier", "ModifyDate",
			};
			//根據 SQL取得　Pagination
			return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
		}

		//是否存在部門代碼
		public bool IsExistBbsID(string id)
		{
            var conditions = new Conditions() { { "@bbs_id", id } };
            var sql = @"select * from BbsContent where bbs_id = @bbs_id";
			var rows = _dc.QueryForRowsCount(sql, conditions);

			return (rows > 0) ? true : false;
		}

		//新增
		public void CreateData(BbsViewModel model)
		{
			//model 不得為 null
			if (model == null) { throw new Exception("請檢查輸入的資料!"); }

			//判斷有無此部門代碼
            if (IsExistBbsID(model.bbs_id.ToString())) { throw new Exception("新增失敗，已經存在相同代碼!"); }

			var mainpulationConditions = new Conditions()
			{
                {"@bbs_title", model.txt_Title },
				{"@bbs_content", model.txt_Content},
				{"@startdatetime", model.DefaultStartDateTime},
				{"@enddatetime", model.DefaultEndDateTime},
				{"@bbs_http", model.txt_Http},
                {"@bbs_photo", model.PhotoName},
                {"@bbs_file", model.UpName},
				{"@Creator", model.Creator},
				{"@CreateDate", model.CreateDate},
			};

			try
			{
				_dc.ExecuteAndCheck(
@"INSERT INTO BbsContent 
			(bbs_title, 
			 bbs_content, 			 
			 startdatetime, 
			 enddatetime, 
			 bbs_http,
             bbs_photo, 
             bbs_file,			 
			 creator, 
			 createdate) 
VALUES      (@bbs_title, 
			 @bbs_content, 
			 @startdatetime, 
			 @enddatetime, 
			 @bbs_http,
             @bbs_photo,
             @bbs_file,			 
			 @Creator, 
			 @CreateDate) ", mainpulationConditions);
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}



		//編輯頁面 > 取得部門資料
        public BbsViewModel GetBbsData(string bbsID)
		{
            BbsViewModel model = null;
            string strSQL = @"Select * from BbsContent where bbs_id = @bbs_id";
            var strCondition = new Conditions() { { "@bbs_id", bbsID } };

			var result = _dc.QueryForDataRow(strSQL, strCondition);
			if (result == null) { return null; }

			model = new BbsViewModel()
			{
                bbs_id = int.Parse(result["bbs_id"].ToString()),
                txt_Title = result["bbs_title"].ToString(),
                txt_Content = result["bbs_content"].ToString(),
                DefaultStartDateTime = !result["StartDateTime"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["StartDateTime"].ToString()) : (DateTime?)null,             
                DefaultEndDateTime = !result["EndDateTime"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["EndDateTime"].ToString()) : (DateTime?)null,
                txt_Http = result["bbs_http"].ToString(),
                PhotoName = result["bbs_photo"].ToString(),
                UpName = result["bbs_file"].ToString(),
                //DepartmentLevel = Int32.Parse(result["DepartmentLevel"].ToString()),
                //FilingEmployeeID_FK = result["FilingEmployeeID_FK"].ToString(),
                //Disabled = "True".Equals(result["Disabled"].ToString()),
                //DisabledDate = !result["DisabledDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["DisabledDate"].ToString()) : (DateTime?)null,
				Modifier = result["Modifier"].ToString(),
				ModifyDate = !result["ModifyDate"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ModifyDate"].ToString()) : DateTime.Parse(result["CreateDate"].ToString()),
				TimeStamp = (result["ModifyDate"].IsDBNullOrWhiteSpace() ? result["CreateDate"].ToString() : result["ModifyDate"].ToString()).ToDateFormateString(),

				EmployeeDic = _rootRepo.GetEmployee(),
				DepartmentDic = _rootRepo.GetDepartment(),
			};

			return model;
		}


		//編輯
		public void EditData(BbsViewModel model)
		{
            var orgModel = GetBbsData(model.bbs_id.ToString());
			//model 不得為 null
			if (model == null || orgModel == null) { throw new Exception("請檢查輸入的資料!"); }

			if (!model.TimeStamp.Equals(orgModel.ModifyDate.FormatDatetimeNullable())) { throw new Exception("此筆資料已被他人更新!請重新確認!"); }

			var mainpulationConditions = new Conditions()
			{
				{ "@bbs_id", model.bbs_id },
                { "@bbs_title", model.txt_Title },
				{ "@bbs_content", model.txt_Content },
				{ "@startdatetime", model.DefaultStartDateTime },
				{ "@enddatetime", model.DefaultEndDateTime },
				{ "@bbs_http", model.txt_Http },
                { "@bbs_photo", model.PhotoName },
                { "@bbs_file", model.UpName},
				{"@Modifier", model.Modifier}
			};

			try
			{
                //bbs_photo = @bbs_photo, 
		        //bbs_file = @bbs_file,  
				_dc.ExecuteAndCheck(
                                    @"Update BbsContent Set 
		                                     bbs_title = @bbs_title,
		                                     bbs_content = @bbs_content, 
		                                     startdatetime = @startdatetime, 
		                                     enddatetime = @enddatetime, 
		                                     bbs_http = @bbs_http, 
                                             bbs_photo = @bbs_photo,
                                             bbs_file = @bbs_file, 
		                                     Modifier = @Modifier,
		                                     ModifyDate = getDate()
                                     Where bbs_id = @bbs_id", mainpulationConditions);
			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

        //簽核作業頁面 > 取得表單資料
        public BbsViewModel GetBbs(string bbs_id)
        {
            BbsViewModel model = null;
            string strSQL = @"Select * from BbsContent Where bbs_id = @bbs_id";

            var strCondition = new Conditions() { { "@bbs_id", bbs_id } };

            var result = _dc.QueryForDataRow(strSQL, strCondition);
            if (result == null) { return null; }

            model = new BbsViewModel()
            {
                bbs_id = Int32.Parse(result["bbs_id"].ToString()),
                txt_Title = result["bbs_title"].ToString(),
                txt_Content = result["bbs_content"].ToString(),
                DefaultStartDateTime = !result["StartDateTime"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["StartDateTime"].ToString()) : (DateTime?)null,             
                DefaultEndDateTime = !result["EndDateTime"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["EndDateTime"].ToString()) : (DateTime?)null,
                txt_Http = result["bbs_http"].ToString(),
                PhotoName = result["bbs_photo"].ToString(),
                UpName = result["bbs_file"].ToString(),
                Modifier = result["Modifier"].ToString(),
                Creator = result["Creator"].ToString()
                //FormID_FK = Int32.Parse(result["FormID_FK"].ToString()),
                //SignDocID_FK = result["SignDocID_FK"].ToString(),
                //ApplyID_FK = result["ApplyID_FK"].ToString(),
                //ApplyName = result["ApplayName"].ToString(),
                //ApplyDateTime = DateTime.Parse(result["ApplyDateTime"].ToString()),
                //EmployeeID_FK = result["EmployeeID_FK"].ToString(),
                //EmployeeName = result["EmployeeName"].ToString(),
                //DepartmentID_FK = result["DepartmentID_FK"].ToString(),
                //DepartmentName = result["DepartmentName"].ToString(),
                //PeriodType = Int32.Parse(result["PeriodType"].ToString()),
                //PunchName = result["PunchName"].ToString(),
                //ForgotPunchInDateTime = !result["ForgotPunchInDateTime"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ForgotPunchInDateTime"].ToString()) : (DateTime?)null,
                //ForgotPunchOutDateTime = !result["ForgotPunchOutDateTime"].IsDBNullOrWhiteSpace() ? DateTime.Parse(result["ForgotPunchOutDateTime"].ToString()) : (DateTime?)null,

                //Note = result["Note"].ToString()
            };

            return model;
        }

        public string Find_Photo(string CID)
        {
            string File_Photo = string.Empty;
            string strSQL = @"select bbs_photo from BbsContent  WHERE bbs_id='" + CID + "'";
            temp = DBClass.Create_Table(strSQL, "GET");
            if (temp.Rows.Count > 0)
            {
                File_Photo = temp.Rows[0]["bbs_photo"].ToString();
            }
            return File_Photo;
        }

        public string Find_Url(string CID)
        {
            string File_Photo = string.Empty;
            string strSQL = @"select bbs_file from BbsContent  WHERE bbs_id='" + CID + "'";
            temp = DBClass.Create_Table(strSQL, "GET");
            if (temp.Rows.Count > 0)
            {
                File_Photo = temp.Rows[0]["bbs_file"].ToString();
            }
            return File_Photo;
        }
        //刪除公告
        public void DelData(string bbsID)
        {
            var mainpulationConditions = new Conditions()
		    {
			    {"@bbs_id", bbsID }
		    };

            try
            {
                _dc.ExecuteAndCheck(@"Delete BbsContent Where bbs_id = @bbs_id ", mainpulationConditions);

            }
            catch (Exception ex)
            {
                throw new Exception("刪除失敗!" + ex.Message);
            }
        }
	}
}