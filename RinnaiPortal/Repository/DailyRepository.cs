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
    public class DailyRepository : ISignRepository
    {
        private DB _dc { get; set; }
        private RootRepository _rootRepo { get; set; }
        public DailyRepository(DB dc, RootRepository rootRepo)
		{
			_dc = dc;
			_rootRepo = rootRepo;
		}
        //列表
        public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
        {
            //string DataSource = RepositoryFactory.SmartManConn["DataSource"] + "." + RepositoryFactory.SmartManConn["Catelog"] ;

            string strSQL = String.Format(
                                @"SELECT * from (SELECT     AA.EMPLOYECD AS USERID, AA.DUTYDATE, MAX(AA.PAYYYYYMM) AS PAYYYYYMM, ISNULL(MAX(AA.BEGINTIME),'') AS BeginTime, ISNULL(MAX(AA.ENDTIME),'') AS EndTime, 
                                                            SUM(AA.OverWorkHours) AS OverWorkHours, SUM(AA.RecreateDays) AS RecreateDays, SUM(AA.OffWork1) AS OffWork1, SUM(AA.OffWork2) AS OffWork2, 
                                                            SUM(AA.OFFWORK3) AS OffWork3, SUM(AA.OffHours) AS OffHours, SUM(AA.OffWork5M) AS OffWork5M, SUM(AA.OffWork6M) AS OffWork6M, SUM(AA.OffWork8) 
                                                            AS OffWork8, SUM(AA.OffWork9) AS OffWork9, SUM(AA.OffWorkHours) AS OffWorkHours, SUM(AA.OffWork14) AS OffWork14, SUM(AA.MEALDELAY) AS MealDelay, 
                                                            SUM(AA.LOSTTIMES) AS LostTimes, SUM(AA.AddHours) AS AddHours, SUM(AA.OverWork1) AS OverWork1, SUM(AA.OverWork2) AS OverWork2, SUM(AA.OverWork3) 
                                                            AS OverWork3, SUM(AA.OverWork4) AS OverWork4, MAX(Department.CODENAME) AS Department_ID, 
                                                            ISNULL(MAX(AA.RemarkOff),'') AS RemarkOff
                                    FROM         (SELECT     DO.EMPLOYECD, DO.DUTYDATE, DO.PAYYYYYMM, DO.BEGINTIME, DO.ENDTIME, 0 AS OverWorkHours, 0 AS RecreateDays, 0 AS OffWork1, 
                                                                                    0 AS OffWork2, CASE WHEN DO.DUTYDATE = CAST(CONVERT(varchar, getdate(), 112) AS DECIMAL) THEN 0 ELSE F.OffWork3 END AS OFFWORK3, 
                                                                                    0 AS OffHours, F.OffWork5M, F.OffWork6M, 0 AS OffWork8, 0 AS OffWork9, 0 AS OffWorkHours, 0 AS OffWork14, DO.MEALDELAY, 
                                                                                    CASE WHEN DO.DUTYDATE = CAST(CONVERT(VARCHAR, GETDATE(), 112) AS DECIMAL) THEN 0 ELSE DO.LOSTTIMES END AS LOSTTIMES, 0 AS AddHours,
                                                                                    0 AS OverWork1, 0 AS OverWork2, 0 AS OverWork3, 0 AS OverWork4, DO.RemarkOff
                                                            FROM          ITEIP.HRIS.dbo.DAILYONOFF AS DO INNER JOIN
                                                                                    ITEIP.HRIS.dbo.EMPLOYEE AS EM ON EM.EMPLOYECD = DO.EMPLOYECD AND DO.EMPLOYECD = EM.EMPLOYECD LEFT OUTER JOIN
                                                                                    ITEIP.HRIS.dbo.Dailyonoff_View AS F ON F.Employecd = DO.EMPLOYECD AND F.DutyDate = DO.DUTYDATE
                                                            WHERE      (LEFT(DO.DUTYDATE, 4) BETWEEN CAST(YEAR(GETDATE()) - 1 AS DECIMAL) AND CAST(YEAR(GETDATE()) + 1 AS DECIMAL)) AND 
                                                                                    (EM.EMPLOYECD IN
                                                                                        (SELECT     EMPLOYECD
                                                                                        FROM          ITEIP.HRIS.dbo.EMPLOYEE AS Sub
                                                                                        WHERE      (1 = 1))) AND (DO.COMPANYCD = 'A') AND (EM.HIRETYPE IN ('A', 'B', 'C', 'D', 'E', 'F', ''))
                                                            UNION ALL
                                                            SELECT     EMPLOYECD, DUTYDATE, '' AS PAYYYYYMM, '' AS BeginTime, '' AS EndTime, 0 AS OverWorkHours, ISNULL(SUM(RECREATEDAYS), 0) AS RecreateDays, 
                                                                                    ISNULL(SUM(OFFWORK1), 0) AS OffWork1, ISNULL(SUM(OFFWORK2), 0) AS OffWork2, 0 AS OffWork3, ISNULL(SUM(OFFHOURS), 0) AS OffHours, 
                                                                                    0 AS Offwork5M, 0 AS Offwork6M, ISNULL(SUM(OFFWORK8), 0) AS OffWork8, ISNULL(SUM(OFFWORK9), 0) AS OffWork9, ISNULL(SUM(OFFWORKHOURS), 0)
                                                                                    AS OffWorkHours, ISNULL(SUM(OffWork14), 0) AS OffWork14, 0 AS MealDelay, 0 AS LostTimes, 0 AS AddHours, 0 AS OverWork1, 0 AS OverWork2, 
                                                                                    0 AS OverWork3, 0 AS OverWork4, '' AS REMARKOFF
                                                            FROM         ITEIP.HRIS.dbo.DAILYOFF
                                                            WHERE     (LEFT(DUTYDATE, 4) BETWEEN CAST(YEAR(GETDATE()) - 1 AS DECIMAL) AND CAST(YEAR(GETDATE()) + 1 AS DECIMAL))
                                                            GROUP BY EMPLOYECD, DUTYDATE
                                                            UNION ALL
                                                            SELECT     EMPLOYECD, DUTYDATE, '' AS PAYYYYYMM, '' AS BeginTime, '' AS EndTime, ISNULL(SUM(OVERWORKHOURS), 0) AS OverWorkHours, 
                                                                                    0 AS RecreateDays, 0 AS OffWork1, 0 AS OffWork2, 0 AS OffWork3, 0 AS OffHours, 0 AS Offwork5M, 0 AS Offwork6M, 0 AS OffWork8, 0 AS OffWork9, 
                                                                                    0 AS OffWorkHours, 0 AS OffWork14, 0 AS MealDelay, 0 AS LostTimes, ISNULL(SUM(ADDHOURS), 0) AS AddHours, ISNULL(SUM(OVERWORK1), 0) 
                                                                                    AS OverWork1, ISNULL(SUM(OVERWORK2), 0) AS OverWork2, ISNULL(SUM(OVERWORK3), 0) AS OverWork3, ISNULL(SUM(OVERWORK4), 0) 
                                                                                    AS OverWork4, '' AS REMARKOFF
                                                            FROM         ITEIP.HRIS.dbo.DAILYON
                                                            WHERE     (LEFT(DUTYDATE, 4) BETWEEN CAST(YEAR(GETDATE()) - 1 AS DECIMAL) AND CAST(YEAR(GETDATE()) + 1 AS DECIMAL))
                                                            GROUP BY EMPLOYECD, DUTYDATE) AS AA 
					                                        INNER JOIN ITEIP.HRIS.dbo.Employee AS EMPLOYEE ON AA.EMPLOYECD = Employee.EMPLOYECD 
					                                        INNER JOIN ITEIP.HRIS.dbo.CODEDTL AS Department ON Employee.UNITCD = Department.CODECD and Department.TYPECD = 'UNIT'
							
                                    WHERE     (EMPLOYEE.QUITDATE='0') GROUP BY AA.EMPLOYECD, AA.DUTYDATE ) as Daily
                                        where USERID=@USERID and PAYYYYYMM=@PAYYYYYMM");

            var conditionsDic = new Conditions()
			{
				{ "@USERID", slParms.EmployeeID_FK},
                { "@PAYYYYYMM", slParms.payYYYYMM}
			};

            int PageCount=0;
            if (slParms.PageName=="Default")
            {
                PageCount = 6;
            }
            else
            {
                PageCount = 40;
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
				"USERID", "DUTYDATE", "PAYYYYYMM", "BeginTime", "EndTime", "OverWorkHours", "RecreateDays", "OffWork1", "OffWork2", "OffWork3", "OffHours", "OffWork5M", "OffWork6M", "OffWork8", "OffWork9", "OffWorkHours", "OffWork14", "MealDelay", "LostTimes", "AddHours", "OverWork1", "OverWork2", "OverWork3", "OverWork4", "Department_ID", "RemarkOff"
			};
            //根據 SQL取得　Pagination
            return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
        }
    }
}