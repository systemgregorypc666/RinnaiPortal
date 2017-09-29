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
    public class BbsListRepository : ISignRepository
    {
        private DB _dc { get; set; }
        private RootRepository _rootRepo { get; set; }
        public BbsListRepository(DB dc, RootRepository rootRepo)
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
            //if (!String.IsNullOrWhiteSpace(slParms.QueryText))
            //{
            //    //conditionsDic = new Conditions() { { "@queryText", String.Format("{0}", slParms.QueryText.Trim()) } };
            //    strConditions = String.Format("where bbs.bbs_title like '%" + slParms.QueryText.Trim() + "%' ");
            //}

            //依照公佈開始時間至結束時間篩選
            strConditions = String.Format("where Convert(varchar(10),Getdate(),112) between Convert(varchar(10),startdatetime,112) and Convert(varchar(10),enddatetime,112) ");
            string orderExpression = String.Format("{0}{1}", "-", "StartDateTime");
            //string orderExpression = "startdatetime";
            //string strOrder = " order by startdatetime Desc";
            var paginationParms = new PaginationParms()
            {
                QueryString = String.Format(strSQL, strConditions),
                QueryConditions = conditionsDic,
                PageIndex = pParms.PageIndex,
                PageSize = 5
            };
            var allowColumns = new List<string>
			{
				"bbs_id", "bbs_title", "bbs_content", "bbs_http", "bbs_photo", "bbs_file", "startdatetime", "enddatetime", "Creator", "CreateDate", "Modifier", "ModifyDate",
			};
            //根據 SQL取得　Pagination
            return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
        }
    }
}