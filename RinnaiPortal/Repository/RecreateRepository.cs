using DBTools;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository.Sign;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace RinnaiPortal.Repository
{
    public class RecreateRepository : ISignRepository
    {
        DataTable temp = new DataTable();
        private DB _dc { get; set; }
        private RootRepository _rootRepo { get; set; }
        public RecreateRepository(DB dc, RootRepository rootRepo)
		{
			_dc = dc;
			_rootRepo = rootRepo;
		}
        //列表
        public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
        {
            string DataSource = RepositoryFactory.SmartManConn["DataSource"] + "." + RepositoryFactory.SmartManConn["Catelog"] ;

            string strSQL = String.Format(
                                @"SELECT          EMPLOYECD, YEAR, COUNTBEGDATE, COUNTENDDATE, RECREATEDAYS, INCREASEDAYS, LASTYEARDAYS, 
                                    USEDDAYS, RECREATEDAYS + INCREASEDAYS + LASTYEARDAYS -
                                    (SELECT          ISNULL(SUM(RECREATEDAYS), 0) AS Expr1
                                      FROM               {0}.dbo.DAILYOFF AS D
                                      WHERE           (EMPLOYECD = R.EMPLOYECD) AND (DUTYDATE BETWEEN R.COUNTBEGDATE AND R.COUNTENDDATE)) AS USD
                                FROM              {0}.dbo.RECREATEDAY  AS R                               
                                WHERE YEAR >= YEAR(GETDATE()) and EMPLOYECD=@EMPLOYECD ", DataSource);

            var conditionsDic = new Conditions()
			{
				{ "@EMPLOYECD", slParms.EmployeeID_FK}
			};
        
            var paginationParms = new PaginationParms()
            {
                QueryString = strSQL,
                QueryConditions = conditionsDic,
                //PageIndex = pParms.PageIndex,
                PageSize = 2
            };

            //pParms.OrderField = "CreateDate".Equals(pParms.OrderField) ? "PAYYYYYMM" : pParms.OrderField;
            //string orderExpression = String.Format("{0}{1}", pParms.Descending ? "-" : "", pParms.OrderField);
            string orderExpression = String.Format("{0}{1}", "", "YEAR");
            var allowColumns = new List<string>
			{
				"EMPLOYECD", "YEAR", "COUNTBEGDATE", "COUNTENDDATE", "RECREATEDAYS", "USEDDAYS", "USD"
			};
            //根據 SQL取得　Pagination
            return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
        }

        
    }
}