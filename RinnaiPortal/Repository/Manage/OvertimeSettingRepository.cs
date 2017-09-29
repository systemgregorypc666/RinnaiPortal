using DBTools;
using RinnaiPortal.Area.Manage;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using RinnaiPortal.Repository;
using RinnaiPortal.Tools;
using RinnaiPortal.ViewModel.Sign.Forms;


namespace RinnaiPortal.Repository.Manage
{
    public class OvertimeSettingRepository : IManageRepository
    {
        private DB _dc { get; set; }
        private RootRepository _rootRepo = RepositoryFactory.CreateRootRepo();

        public OvertimeSettingRepository(DB dc)
        {
            _dc = dc;
        }

        //列表
        public Pagination GetPagination(ManageListParms mlParms, PaggerParms pParms)
        {
            string tableName = RepositoryFactory.SmartManConn["DataSource"] + "." + RepositoryFactory.SmartManConn["Catelog"] + ".dbo.DailyOnOff";

            //20170223 修改convert (nvarchar, startDateTime, 112) = onoff.RealOnDate 為 convert (nvarchar, startDateTime, 112) = onoff.DUTYDATE 
            //怕志元沒寫值進RealOnDate欄位
            string strSQL = String.Format(
@"SELECT overtime.*, onoff.*, emp.CostDepartmentID  FROM   overtimeform overtime 
left outer join {0} onoff
on overtime.employeeID_FK = onoff.employecd 
left outer join employee emp
on overtime.employeeID_FK = emp.employeeID 
where autoinsert = 'False' and signDocID_FK = @SignDocID_FK and convert (nvarchar, startDateTime, 112) = onoff.DUTYDATE
and convert (nvarchar, startDateTime, 112) = onoff.DUTYDATE ", tableName);

            var conditionsDic = new Conditions()
			{
				{ "@SignDocID_FK", mlParms.SignDocID },
			};

            var overtimeList = _rootRepo.QueryForOvertimeFormDataBySignDocID(mlParms.SignDocID);

            var paginationParms = new PaginationParms()
            {
                QueryString = strSQL,
                QueryConditions = conditionsDic,
                PageIndex = pParms.PageIndex,
                PageSize = overtimeList.Count
            };

            pParms.OrderField = "CreateDate".Equals(pParms.OrderField) ? "SignDocID_FK" : pParms.OrderField;
            string orderExpression = String.Format("{0}{1}", pParms.Descending ? "-" : "", pParms.OrderField);

            var allowColumns = new List<string>
			{
				"SignDocID_FK", "EmployeeID_FK", "ApplyID_FK", "ApplyDateTime", "EmployeeID_FK", "DepartmentID_FK", "StartDateTime", "EndDateTime", "SupportDeptID_FK", "PayTypeKey", "MealOrderKey", "AutoInsert", "IsHoliday", "TotalHours", "Note"
			};
            //根據 SQL取得　Pagination
            return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
        }

        //更正 autoinsert = true
        public MultiConditions GetAutoInserDML(string signDocID, string user, bool autoInsertValue = true)
        {
            var strSQL =
@"Update Overtimeform Set AutoInsert='True', Modifier = @Modifier, ModifyDate=GetDate() Where SignDocID_FK = @SignDocID ";

            var conditions = new Conditions()
			{
				{ "@SignDocID", signDocID },
				{ "@Modifier", user},
				{ "@AutoInsertValue", autoInsertValue},
			};
            return new MultiConditions() { { strSQL, conditions } };
        }


        public DataTable QueryOvertimeData(string docID)
        {
            string tableName = RepositoryFactory.SmartManConn["DataSource"] + "." + RepositoryFactory.SmartManConn["Catelog"] + ".dbo.DailyOnOff";
            var conditions = new Conditions() { { "@SignDocID_FK", docID } };

            string sql = String.Format(@"SELECT overtime.*, onoff.*, emp.CostDepartmentID  FROM   overtimeform overtime 
                                    left outer join {0} onoff
                                    on overtime.employeeID_FK = onoff.employecd 
                                    left outer join employee emp
                                    on overtime.employeeID_FK = emp.employeeID 
                                    where autoinsert = 'False' and signDocID_FK = @SignDocID_FK and convert (nvarchar, startDateTime, 112) = onoff.DUTYDATE
                                    and convert (nvarchar, startDateTime, 112) = onoff.RealOnDate ", tableName);

            var result = _dc.QueryForDataTable(sql, conditions);
            return result ?? null;
        }
    }
}