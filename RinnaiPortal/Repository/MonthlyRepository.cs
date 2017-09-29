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

namespace RinnaiPortal.Repository
{
    public class MonthlyRepository : ISignRepository
    {
        private DB _dc { get; set; }
        private RootRepository _rootRepo { get; set; }
        public MonthlyRepository(DB dc, RootRepository rootRepo)
		{
			_dc = dc;
			_rootRepo = rootRepo;
		}
        //列表
        public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
        {
            string DataSource = RepositoryFactory.SmartManConn["DataSource"] + "." + RepositoryFactory.SmartManConn["Catelog"] ;

            string strSQL = String.Format(
                                @"SELECT     C.CODENAME DeptName,B.CostDepartmentID DeptID, B.EmployeeID, B.EmployeeName, A.PAYYYYYMM, A.OVERWORK1, A.OVERWORK2, A.OVERWORK3, A.OVERWORK4, A.OVERWORK5, A.RECREATEDAYS, 
                                          A.OFFWORK1, A.OFFWORK2,
                                              (SELECT     CASE COUNT(*) WHEN 0 THEN 0 ELSE SUM(C.OFFWORK3) END AS EXPR2
                                                FROM          {0}.dbo.DAILYOFF AS C
                                                WHERE      (B.EmployeeID = EMPLOYECD) AND (PAYYYYYMM = A.PAYYYYYMM)) AS OFFWORK3, A.OFFWORK5M, A.OFFWORK9, A.OFFWORK6M, A.OFFHOURS, 
                                          ISNULL(A.ADDHOURS,0) as ADDHOURS, A.OVERWORKHOURS, A.OFFWORKHOURS, A.LOSTTIMES, A.OffWork14, A.MEALDELAY,
                                              (SELECT     CASE COUNT(*) WHEN 0 THEN 0 ELSE SUM(C.MEALDELAY2) END AS Expr1
                                                FROM          {0}.dbo.DAILYON AS C
                                                WHERE      (B.EmployeeID = EMPLOYECD) AND (PAYYYYYMM = A.PAYYYYYMM)) AS mealdelay2,ISNULL(E.ADDOFFHOURS,0) as ADDOFFHOURS
                                FROM  {0}.dbo.MONTHLYONOFF AS A 
                                INNER JOIN dbo.Employee AS B ON A.EMPLOYECD = B.EmployeeID
                                LEFT OUTER JOIN {0}.dbo.CODEDTL AS C ON B.CostDepartmentID=C.CODECD and C.TYPECD = 'UNIT'
                                LEFT OUTER JOIN {0}.dbo.EMPLOYEE AS E ON A.EMPLOYECD = E.EMPLOYECD AND A.PAYYYYYMM =
																							                                (SELECT     PAYYYYYMM
																								                            FROM          (SELECT     TOP (1) STR(SYSTEMDATE) + SYSTEMTIME AS DAYTIME, (CASE RIGHT(PAYYYYYMM, 2) 
																																			                                WHEN 1 THEN PAYYYYYMM ELSE (PAYYYYYMM - 1) END) AS PAYYYYYMM
																														                                    FROM          {0}.dbo.RUNSALARY
																														                                    WHERE      (RUNTYPE = 'RUNM')
																														                                    ORDER BY DAYTIME DESC) AS AA)
                                WHERE     (B.Disabled ='false') AND (LEFT(A.PAYYYYYMM, 4) BETWEEN CAST(YEAR(GETDATE()) - 1 AS char) AND CAST(YEAR(GETDATE()) + 1 AS char)) and B.EmployeeID=@EmployeeID ", DataSource);

            var conditionsDic = new Conditions()
			{
				{ "@EmployeeID", slParms.EmployeeID_FK}
			};

            int PageCount=0;
            if (slParms.PageName=="Default")
            {
                PageCount = 3;
            }
            else
            {
                PageCount = 24;
            }
            var paginationParms = new PaginationParms()
            {
                QueryString = strSQL,
                QueryConditions = conditionsDic,
                //PageIndex = pParms.PageIndex,
                PageSize = PageCount
            };

            //pParms.OrderField = "CreateDate".Equals(pParms.OrderField) ? "PAYYYYYMM" : pParms.OrderField;
            //string orderExpression = String.Format("{0}{1}", pParms.Descending ? "-" : "", pParms.OrderField);
            string orderExpression = String.Format("{0}{1}", "-", "PAYYYYYMM");
            var allowColumns = new List<string>
			{
				"DeptName", "DeptID", "EmployeeID", "EmployeeName", "PAYYYYYMM", "OVERWORK1", "OVERWORK2", "OVERWORK3", "OVERWORK4", "OVERWORK5", "RECREATEDAYS", "OFFWORK1", "OFFWORK2", "OFFWORK3", "OFFWORK5M", "OFFWORK9", "OFFWORK6M", "OFFHOURS", "ADDHOURS", "OVERWORKHOURS", "OFFWORKHOURS", "LOSTTIMES", "OffWork14", "MEALDELAY", "mealdelay2", "ADDOFFHOURS"
			};
            //根據 SQL取得　Pagination
            return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
        }
    }
}