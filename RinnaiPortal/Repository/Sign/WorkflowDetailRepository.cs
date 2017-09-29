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
	public class WorkflowDetailRepository : ISignRepository
	{
		private DB _dc { get; set; }
		private RootRepository _rootRepo { get; set; }
		public WorkflowDetailRepository(DB dc, RootRepository rootRepo)
		{
			_dc = dc;
			_rootRepo = rootRepo;
		}

		//列表
		public Pagination GetPagination(SignListParms slParms, PaggerParms pParms)
		{
			string strSQL =
@"SELECT sn, 
	   CASE 
		 WHEN signdocid_fk <> '' THEN signdocid_fk 
		 ELSE detailsigndocid_fk 
	   END AS SignDocID_FK, 
	   formtype, 
	   senddate, 
	   departmentname, 
	   finalstatus, 
	   employeename, 
	   remark, 
	   status, 
	   logdatetime 
FROM   signform_log slog 
	   LEFT OUTER JOIN signtype type 
					ON slog.formid_fk = type.formid 
	   LEFT OUTER JOIN department dept 
					ON slog.currentsignleveldeptid_fk = dept.departmentid 
	   LEFT OUTER JOIN employee emp 
					ON slog.chiefid_fk = emp.employeeid 
 {0}";

			string strConditions = " where slog.SignDocID_FK = @SignDocID_FK or slog.DetailSignDocID_FK = @SignDocID_FK ";
			var employeeData = _rootRepo.QueryForEmployeeByADAccount(slParms.Member.ADAccount);
			var paginationParms = new PaginationParms()
			{
				QueryString = String.Format(strSQL, strConditions),
				QueryConditions = new Conditions()
				{
					{ "@SignDocID_FK", slParms.SignDocID},
				},
				PageIndex = pParms.PageIndex,
				PageSize = pParms.PageSize
			};
			pParms.OrderField = "CreateDate".Equals(pParms.OrderField) ? "sn" : pParms.OrderField;
			string orderExpression = String.Format("{0}{1}", pParms.Descending ? "-" : "", pParms.OrderField);

			var allowColumns = new List<string> 
			{ 
				"FormType", "SendDate", "DepartmentName", "FinalStatus", "EmployeeName", "Remark", "Status", "LogDatetime", "sn"
			};

			//根據 SQL取得　Pagination
			return _dc.QueryForPagination(paginationParms, orderExpression, allowColumns);
		}



	}
}