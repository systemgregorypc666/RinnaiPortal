using DBTools;
using RinnaiPortal.Area.Manage;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Linq;

namespace RinnaiPortal.Repository.Manage
{
    public class MealTaxiRepository : IManageRepository
    {
        public readonly DateTime UTC_START_DATETIME = new DateTime(2017, 1, 1);
        private DB _dc { get; set; }
        private RootRepository _rootRepo = RepositoryFactory.CreateRootRepo();

        public MealTaxiRepository(DB dc)
        {
            _dc = dc;
        }

        //列表
        public Pagination GetPagination(ManageListParms mlParms, PaggerParms pParms)
        {
            //組出SQL查詢語句
            string strSQL =
                            @"select distinct * from (select TOP 18000 *  from ( SELECT  overtime.employeeid_fk,
                                emp.employeename,
                                right(overtime.supportdeptid_fk,4) supportdeptid_fk,
                                dept.departmentname,
                                overtime.startdatetime,
                                overtime.enddatetime,
                                overtime.paytypekey,
                                overtime.mealorderkey,
                                emp.nationaltype
                            FROM   overtimeform overtime
                                LEFT OUTER JOIN employee emp
                                ON overtime.employeeid_fk = emp.employeeid
                                LEFT OUTER JOIN department dept
                                ON overtime.supportdeptid_fk = dept.departmentid
                                LEFT OUTER JOIN SignForm_Main Sign_M
                                ON overtime.SignDocID_FK=Sign_M.SignDocID
                                {0} ) as aa) as overtime
                                       ";
            //設定Where條件
            string strConditions = String.Format(
                    @"WHERE  overtime.startdatetime >= @StartDatetime
                    AND overtime.enddatetime < @EndDatetime and Sign_M.FinalStatus <>'5' and emp.CostDepartmentID not like'39%' ");
            //設定參數話查詢欄位
            var conditionsDic = new Conditions()
			{
				{ "@StartDatetime", mlParms.StartDateTime.HasValue ? mlParms.StartDateTime.Value : UTC_START_DATETIME },
				{ "@EndDatetime", mlParms.EndDateTime.HasValue ? mlParms.EndDateTime.Value : DateTime.Now }
			};
            //設定資料所需參數(PaginationParms為資料查詢模型)
            var paginationParms = new PaginationParms()
            {
                QueryString = String.Format(strSQL, strConditions),
                QueryConditions = conditionsDic,
                PageIndex = pParms.PageIndex,
                PageSize = pParms.PageSize
            };

            pParms.OrderField = "CreateDate".Equals(pParms.OrderField) ? "NationalType" : pParms.OrderField;
            string orderExpression = String.Format("{0}{1}", pParms.Descending ? "-" : "", pParms.OrderField);

            var allowColumns = new List<string>
			{
				"EmployeeID_FK", "EmployeeName", "SupportDeptID_FK", "DepartmentName", "StartDatetime", "EndDatetime", "PayTypeKey", "MealOrderKey", "NationalType"
			};

            //根據 SQL取得　Pagination
            return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
        }


        public DataTable GetMealSummary(ManageListParms parms)
        {
            #region #0002 增加避免重複判斷，避免加班餐車資料統計數量與列表不符情況 假日則不判斷，申請次就請幾次伙食

            string holidayCondition = string.Empty;
            {
                string sqlholidayday = @"select * from HOLIDAY where holdate = @StartDatetime ";
                DB smartDb = ConnectionFactory.GetSmartManDC();
                var conditionsHolidayday = new Conditions()
                    {
                        { "@StartDatetime", parms.StartDateTime.HasValue ? parms.StartDateTime.Value.ToString("yyyyMMdd") : UTC_START_DATETIME.ToString("yyyyMMdd") }
                    };
                var isHoliday = smartDb.QueryForDataTable(sqlholidayday, conditionsHolidayday);
                //不等於null就是假日，則申請幾筆顯示幾筆 2017-08-18 先將isHoliday設定為null 假日與平日一樣條件 有時間要將假日新增一個判斷時間若重複就篩掉 不同時段就累積加筆數
                isHoliday = null;
                if (isHoliday != null)
                {
                    holidayCondition = " FROM   overtimeform overtime ";
                }
                else
                {
                    holidayCondition = @"
                    from
                    (
                        select distinct o.supportdeptid_fk,o.startdatetime, o.enddatetime,o.mealorderkey,o.EmployeeID_FK
                        from OvertimeForm o
                        LEFT OUTER JOIN SignForm_Main Sign_M
                        ON o.SignDocID_FK = Sign_M.SignDocID
                        where startdatetime >= @StartDatetime and EndDateTime < @EndDatetime
                        and Sign_M.FinalStatus <> '5'
                    ) as overtime
                    ";
                }
            }

            #endregion #0002 增加避免重複判斷，避免加班餐車資料統計數量與列表不符情況 假日則不判斷，申請次就請幾次伙食

            string strSQL =
            @"
               --Select--
                SELECT right(SupportDeptId,4) as SupportDeptId,supportdeptname SupportDeptName,
                Sum(carnivore) Carnivore,
                Sum(vegan)     Vegan,
                Sum(none)      None,
                Sum(total)-Sum(none)     Total

               --select case 將table符合條件的給1--
                FROM   (
                SELECT dept.departmentname SupportDeptName, dept.DepartmentID SupportDeptId,
                    CASE
                    WHEN overtime.mealorderkey = 'None' THEN 1
                    ELSE 0
                    END                 AS None,
                    CASE
                    WHEN overtime.mealorderkey = 'Carnivore' THEN 1
                    ELSE 0
                    END                 AS Carnivore,
                    CASE
                    WHEN overtime.mealorderkey = 'Vegan' THEN 1
                    ELSE 0
                    END                 AS Vegan,
                    1                   AS Total "
                    +
                //2017-07-24 #0002 增加子查詢 過濾掉重複的員工-
                    holidayCondition
                    +
            @"
                --Join Table--
                LEFT OUTER JOIN department dept
                ON overtime.supportdeptid_fk = dept.departmentid
                Left OUTER JOIN {1}.dbo.DUTYWORK dtw
                ON overtime.employeeid_fk = dtw.EMPLOYECD and CONVERT(varchar(12) , overtime.startdatetime, 112 )=dtw.WORKDATE
                {0}
                ) AS MealSummary
                GROUP  BY SupportDeptId,supportdeptname order by right(SupportDeptId,4) ";

            string table = RepositoryFactory.SmartManConn["DataSource"] + "." + RepositoryFactory.SmartManConn["Catelog"];
            string strConditions = String.Format(
                @"WHERE  overtime.startdatetime >= @StartDatetime
                AND overtime.enddatetime < @EndDatetime
                AND (
                (dtw.HOLIDAY='N' AND REPLACE(SUBSTRING(CONVERT(CHAR(16),startdatetime,120),12,5),':','')>=1700)
                OR (dtw.HOLIDAY='Y' and REPLACE(SUBSTRING(CONVERT(CHAR(16),enddatetime,120),12,5),':','')>=0800)
                       )
                AND dept.departmentid NOT LIKE'39%'
                AND dept.departmentid NOT like'9239%'
                AND dept.departmentid NOT LIKE'9339%' ");

            var conditionsDic = new Conditions()
			{
				{ "@StartDatetime", parms.StartDateTime.HasValue ? parms.StartDateTime.Value : UTC_START_DATETIME },
				{ "@EndDatetime", parms.EndDateTime.HasValue ? parms.EndDateTime.Value : DateTime.Now }
			};

            //組成查詢條件 SQL
            strSQL = String.Format(strSQL, strConditions, table);
            return _dc.QueryForDataTable(strSQL, conditionsDic);
        }

        public DataTable GetTaxiSummary(ManageListParms parms)
        {
            string strSQL =
@"SELECT DISTINCT overtime.enddatetime    AS EndDatetime,
				Count(emp.nationaltype) AS Total
FROM   overtimeform overtime
	   LEFT OUTER JOIN employee emp
					ON overtime.employeeid_fk = emp.employeeid
	   LEFT OUTER JOIN department dept
					ON overtime.supportdeptid_fk = dept.departmentid
       LEFT OUTER JOIN SignForm_Main Sign_M
					ON overtime.SignDocID_FK=Sign_M.SignDocID
{0}
GROUP  BY overtime.enddatetime ";

            string strConditions = String.Format(
@"WHERE  overtime.startdatetime >= @StartDatetime
	   AND overtime.enddatetime < @EndDatetime
	   AND (emp.nationaltype NOT IN ( 'Taiwan', 'Japan' )  and emp.SexType <>'F') and Sign_M.FinalStatus <>'5' and emp.CostDepartmentID not like'39% '
");

            var conditionsDic = new Conditions()
			{
				{ "@StartDatetime", parms.StartDateTime.HasValue ? parms.StartDateTime.Value : UTC_START_DATETIME },
				{ "@EndDatetime", parms.EndDateTime.HasValue ? parms.EndDateTime.Value : DateTime.Now }
			};

            //組成查詢條件 SQL
            strSQL = String.Format(strSQL, strConditions);

            return _dc.QueryForDataTable(strSQL, conditionsDic);
        }
    }
}