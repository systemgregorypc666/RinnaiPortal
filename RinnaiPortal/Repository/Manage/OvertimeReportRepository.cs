using DBTools;
using RinnaiPortal.Area.Manage;
using RinnaiPortal.FactoryMethod;
using RinnaiPortal.Interface;
using RinnaiPortal.Repository.SmartMan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RinnaiPortal.Repository.Manage
{
	public class OvertimeReportRepository : IManageRepository
	{
		public readonly DateTime UTC_START_DATETIME = new DateTime(2017, 1, 1);
		private DB _dc { get; set; }

		public OvertimeReportRepository(DB dc)
		{
			_dc = dc;
		}

		//列表
		public Pagination GetPagination(ManageListParms mlParms, PaggerParms pParms)
		{
			string strSQL =
@"SELECT main.signdocid SignDocID, 
       overtime.ApplyID_FK ApplyID,
	   emp2.employeename Applyname,
	   overtime.employeeid_fk EmployeeID, 
	   emp.employeename EmployeeName,  
	   main.senddate SendDate, 
	   main.finalstatus FinalStatus, 
	   main.remainder Remainder, 
	   detail.chiefid_fk ChiefID, 
	   chief.employeename ChiefName, 
	   detail.status Status, 
	   overtime.paytypekey PayType, 
	   sup.departmentname SupportDeptName, 
	   overtime.startdatetime StartDateTime,  
	   overtime.enddatetime EndDateTime, 
	   overtime.totalhours TotalHours,
	   overtime.isholiday IsHoliday, 
	   overtime.autoinsert AutoInsert
FROM   signform_main main 
	   LEFT OUTER JOIN signform_detail detail 
					ON main.signdocid = detail.signdocid_fk 
	   LEFT OUTER JOIN overtimeform overtime 
					ON main.signdocid = overtime.signdocid_fk 
	   LEFT OUTER JOIN employee emp 
					ON overtime.employeeid_fk = emp.employeeid 
       LEFT OUTER JOIN employee emp2 
					ON overtime.ApplyID_FK = emp2.employeeid
	   LEFT OUTER JOIN employee chief 
					ON detail.chiefid_fk = chief.employeeid 
	   LEFT OUTER JOIN department sup 
					ON overtime.supportdeptid_fk = sup.departmentid 
 {0}";

			string strConditions = String.Format(
@"WHERE overtime.startdatetime >= @StartDatetime 
	   AND overtime.enddatetime < @EndDatetime and finalStatus<>'5'");

			var conditionsDic = new Conditions()
			{
				{ "@StartDatetime", mlParms.StartDateTime.HasValue ? mlParms.StartDateTime.Value : UTC_START_DATETIME },
				{ "@EndDatetime", mlParms.EndDateTime.HasValue ? mlParms.EndDateTime.Value : DateTime.Now },
				{ "@FinalStatus", mlParms.FinalStatus}
			};

			if (!String.IsNullOrWhiteSpace(mlParms.FinalStatus))
			{
				strConditions += " And finalStatus = @FinalStatus";
			}
			else
			{
				strConditions += " And finalStatus <> '6'";
			}


			var paginationParms = new PaginationParms()
			{
				QueryString = String.Format(strSQL, strConditions),
				QueryConditions = conditionsDic,
				PageIndex = pParms.PageIndex,
				PageSize = pParms.PageSize
			};

			pParms.OrderField = "CreateDate".Equals(pParms.OrderField) ? "SignDocID" : pParms.OrderField;
			string orderExpression = String.Format("{0}{1}", pParms.Descending ? "-" : "", pParms.OrderField);

			var allowColumns = new List<string>
			{
				"SignDocID", "EmployeeID", "EmployeeName", "SendDate", "FinalStatus", "Remainder", "ChiefID", "ChiefName", "Status", "PayType", "StartDateTime", "EndDateTime", "SupportDeptName", "AutoInsert", "IsHoliday", "TotalHours"
			};
			//根據 SQL取得　Pagination
			return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
		}


	}
}